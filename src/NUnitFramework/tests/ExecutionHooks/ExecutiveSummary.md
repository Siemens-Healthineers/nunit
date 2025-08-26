# Introduction to NUnit Execution Hooks
This PR adds a new, lightweight extension mechanism that lets consumers register handlers 
to run immediately before and after test-relevant methods. It exposes these hooks via the `TestExecutionContext` 
so extensions/attributes can add handlers at method, class and assembly levels. 
From the consumers perspective the execution hooks can be accessed via `TestContext.CurrentContext`.

# Motivation
For higher level tests it is often necessary to execute user defined functionalities directly before and after test relevant methods, which are:

- Test methods,
- SetUp / TearDown methods,
- OneTimeSetUp / OneTimeTearDown methods,
- BeforeTest / AfterTest methods of ITestAction.

One example of such a use case is logging. We create extensive test protocols. In order to protocol that a specific method has started and finished, 
the user code needs to be notified about it.

# Delivery model
The feature is delivered in vertical slices. This PR (Slice 1) implements synchronous before/after hooks for **Test Methods**
and the plumbing to create, register and invoke them. Subsequent PRs will add other hook points, outcome calculation and async modes.

# Design Goals
- No public breaking changes to existing APIs — the hooks are additive and consumed via attributes/extensions.
- No runtime cost for tests not using hooks. Hooks are not created when not used.

# Major design decisions
- **TestExecutionContext is the access point**:   
Extensions apply hooks to the context so they are available during execution.

- **Command wrapper approach**:   
Test **method** execution is wrapped by a delegating TestCommand that invokes registered hooks and then delegates to the inner command.

- **Hook ordering**:
   - Before-hooks are invoked in registration order of the handlers.
   - After-hooks are invoked in the **reverse** order of registration of the handlers.
   - The original execution order of test relevant methods is preserved. 
   In addition, the hooks are tightly coupled to the methods which they are associated with and are executed immediately before and after the corresponding methods.

- **Hook scoping**:   
ExecutionHooks are attached to `TestExecutionContext` and created lazily. When test objects are nested the hook collections are cloned 
along with the context to avoid leakage across tests.

- **Exception behavior**:   
If a hook throws, the exception is propagated and fails the test. Tests verify propagation and that inner test execution semantics remain predictable.

- **Thread-safety**:   
In order to safe guard parallel test executions, Hook extension is designed to be thread-safe.

- **Performance**:     
Minimal runtime cost when unused. Hooks are lazily allocated thereby the code avoids creating hook structures when not needed 
so no performance penalty for users not using hooks.

- **Minimize intrusive changes**:   
Alignment with existing NUnit extension patterns, integration points like IWrapTestMethod / IWrapSetUpTearDown 
were considered to be used in order to minimize intrusive changes to NUnit Framework.

# Example usage
A hook can be created by deriving from `ExecutionHookAttribute` and overriding the relevant methods.
```C#
[AttributeUsage(AttributeTargets.Method)]
public sealed class ActivateAfterTestHooksAttribute : ExecutionHookAttribute
{
    public override void AfterTestHook(TestExecutionContext context)
    {
        // Do something after the test
    }
}
```
Usage in test code is straightforward. Just add the attribute to a test method:
```C#
[Test]
[ActivateAfterTestHook]
public void SomeTest()
{
}
```
Similarly, hooks can be applied at the class or assembly level. And multiple instances of a hook can be placed on the same element.

# Slice 1 - High level changes
- Introduced an `ExecutionHooks` (originally HookExtensions) subsystem that:
   - Exposes hook collections on `TestContext.CurrentContext`.
   - Allows attributes to add handlers to those hooks. `ExecutionHookAttribute` is provided as a base class for consumers to derive from and override hook methods.
   - Invokes registered handlers before and after test methods.
- Added TestHook abstractions to hold handlers. Thread-safe snapshots are used when invoking. For e.g. `BeforeHook` and `AfterHook`.
- Added a `HookDelegatingTestCommand` (delegating command) that wraps test execution and calls hook handlers at the correct times.

- **Key Tests**: 
Comprehensive tests cover hook creation (method/class/assembly), execution ordering, `ITestAction` interaction, and exception handling.

- **Code Improvements**: 
Numerous code-review-driven enhancements, including class sealing, thread-safety fixes, `EventHandler` usage, and optimizations to minimize allocations.

- **Pull request**: 
[PR-4986 NUnit Framework repo](https://github.com/nunit/nunit/pull/4986)

# Slice 2 - High level changes
- **New functionalities**:
   - Added hook extensions for:
      - [SetUp] / [TearDown]
      - [OneTimeSetUp] / [OneTimeTearDown]
      - BeforeTest() and AfterTest() of ITestAction.
   - Implemented reverse‑ordering for after-hooks using ForwardTestHook / ReverseTestHook classes.
   - Added tests covering:
      - Hook application at SetUp/TearDown and OneTimeSetUp/TearDown methods.
      - Combination of hooks with ITestAction.
      - Ensuring hooks run correctly in presence of SetupFixtures.

- **Improvements (on top of Slice 1)**:
   - Refined HookDelegatingTestCommand to only wrap commands when hooks are present (avoids overhead).
   - Added explicit support & tests for class/assembly‑level hooks.
   - Hardened thread‑safety.

- **Improvements in NUnit Framework**:
   - Added test filter for explicit tests in `TestBuilder` (Already part of NUnit Main). 

- **Code quality improvements**:
   - Fixed CA/style issues; sealed hook classes, removed unused setters.

- **Pull request**: [PR-21 Healthineers NUnit Fork](https://github.com/Siemens-Healthineers/nunit/pull/21)

# Slice 3 - High level changes
- **New functionalities added**:
   - Introduced outcome calculation helpers inside ExecutionHooks.      
   - Provided an API on `TestResult` to query the outcome of the hooked method through `TestExecutionContext`.
   - Tests added to validate correct outcome reporting in all possible outcomes.

- **Improvements on top of Slice 2**:
   - Maintained thread‑safety with reduced mutable state.

- **Code quality improvements**:
   - Encapsulated parameters for hook handlers in a separate class `HookData`.
   - Cleanups: removed unused event args, enforced get‑only properties, namespace/style fixes.
   
- **Pull request**: [PR-27 Healthineers NUnit Fork](https://github.com/Siemens-Healthineers/nunit/pull/27)

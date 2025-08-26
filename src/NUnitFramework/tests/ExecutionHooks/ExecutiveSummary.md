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
Minimal runtime cost when unused. The code avoids creating hook structures when not needed so no performance penalty for users not using hooks.
Hooks are lazily allocated, so projects not using hooks incur no allocation cost. Integration points add minimal overhead only when hooks are present.

- **Minimize intrusive changes**:   
Alignment with existing NUnit extension patterns, integration points like IWrapTestMethod / IWrapSetUpTearDown 
were considered to be used in order to minimize intrusive changes to NUnit Framework.

# High level changes
- Introduced an `ExecutionHooks` (originally HookExtensions) subsystem that:
   - Exposes hook collections on `TestExecutionContext`.
   - Allows attributes to add handlers to those hooks. `ExecutionHookAttribute` is provided as a base class for consumers to derive from.
   - Invokes registered handlers before and after test methods.
  
- Added TestHook abstractions to hold handlers. Thread-safe snapshots are used when invoking. For e.g. `BeforeHook` and `AfterHook`.

- Added a `HookDelegatingTestCommand` (delegating command) that wraps test execution and calls hook handlers at the correct times.

- Key Tests added:
   - Creation/TestHooksCreationAtMethodLevelTests.cs
   - Creation/TestHooksCreationAtAssemblyLevelTests.cs
   - Execution/CombinedHookTests.cs (ordering, ITestAction interaction)
   - Exception handling tests (verify propagation)
   - Testdata/ExecutionHooksTestData.cs (fixtures/attributes used in tests)

- Numerous code-review-driven improvements:
   - Sealing classes, 
   - Thread-safety fixes, 
   - Replacing custom delegate usage with EventHandler, 
   - Minimizing unnecessary allocations and avoiding creating instances of the hook structure when not required.

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

# Detailed changes
- **src/NUnitFramework/framework/Internal/ExecutionHooks/ExecutionHooks.cs**
   - New ExecutionHooks container (renamed from HookExtension).
   - Adds properties for each hook (e.g. BeforeTestHook, AfterTestHook) and creation/clone logic.
   - Lazy creation / avoidance of allocating hooks when unused.
   - Small API/visibility refinements (sealed, removed unused setters).

- **src/NUnitFramework/framework/Internal/ExecutionHooks/TestHook.cs (+ Forward/Reverse variants)*-  
   - New TestHook abstraction for registering/invoking handlers.
   - Introduced ForwardTestHook and ReverseTestHook to support ordering semantics.
   - Thread-safety: locking and returning a handler snapshot before invocation.
   - Public/internal methods: AddHandler, GetHandlers, InvokeHandlers; Count property.
   - Replaced earlier delegate implementation with EventHandler/Action usage and fixed CA1860/style issues.

- **src/NUnitFramework/framework/Attributes/ExecutionHookAttribute.cs (and base attribute)**
   - New attribute(s) to let users declare/implement execution hooks.
   - Implements IApplyToContext (or similar) so attributes add handlers to TestExecutionContext.
   - Helper methods to detect which hook methods are implemented on a derived attribute.

- **src/NUnitFramework/framework/Internal/Commands/HookDelegatingTestCommand.cs (delegating wrapper)*-          
   - New delegating TestCommand that:
    - Invokes before-test handlers.
    - Calls the innerCommand (actual test execution).
    - Invokes after-test handlers (reverse-order semantics where applicable).
   - Ensures inner command executes in presence of hooks; exception handling tests added/updated.
   - Contains logic to avoid wrapping/executing innerCommand unnecessarily when no hooks exist (review requested).

- **src/NUnitFramework/framework/Internal/TestExecutionContext.cs**
   - Exposes ExecutionHooks on the context (added property / accessor).
   - Ensures the ExecutionHooks instance is available to attributes and commands (with lazy creation).
   - Minor adjustments to prevent creating ExecutionHooks when not required.

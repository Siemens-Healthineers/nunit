// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    /// <summary>
    /// Useful when exposing IMethodInfo data without needing to expose the entire IMethodInfo interface
    /// and the possibility to invoke it.
    /// </summary>
    /// <param name="methodInfo">The <see cref="IMethodInfo"/> to be wrapped.</param>
    public class MethodInfoAdapter(IMethodInfo methodInfo)
    {
        private readonly IMethodInfo _methodInfo = methodInfo;

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public string Name => _methodInfo.Name;

        /// <summary>
        /// Gets the full declaring type of the method.
        /// </summary>
        public string DeclaringTypeFullName => _methodInfo.MethodInfo.DeclaringType?.FullName ?? string.Empty;

        /// <summary>
        /// Gets the parameters of the method.
        /// </summary>
        public IParameterInfo[] Parameters => _methodInfo.GetParameters();
    }
}

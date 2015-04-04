using System;
using System.Reflection;

namespace Nova.Library.ActionMethodRepository
{
    /// <summary>
    /// Class to make life inside the repository easier.
    /// </summary>
    internal abstract partial class OnAction
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Invokes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="context">The context.</param>
        public abstract void Invoke(object instance, ActionContext context);

        /// <summary>
        /// Initializes a new instance of the <see cref="OnAction" /> class.
        /// </summary>
        /// <param name="method">The method.</param>
        protected OnAction(MethodBase method)
        {
            var name = method.Name;

            Name = name.EndsWith("ACTION", StringComparison.OrdinalIgnoreCase)
                       ? name.Substring(0, name.Length - 6)
                       : name;
        }

        /// <summary>
        /// Creates the specified method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static OnAction Create<T>(MethodInfo method)
        {
            var parameters = method.GetParameters();
            if (parameters.Length > 1 ||
                (parameters.Length == 1 && parameters[0].ParameterType != typeof (ActionContext)))
                return null;

            return parameters.Length == 0
                       ? CreateWithoutParameters<T>(method)
                       : CreateWithParameters<T>(method);
        }

        /// <summary>
        /// Creates the method with parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        private static OnAction CreateWithParameters<T>(MethodInfo method)
        {
            if (method.IsStatic)
            {
                return new StaticOnActionWithParameters(method);
            }

            return new OnActionWithParameters<T>(method);
        }

        /// <summary>
        /// Creates the method without parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        private static OnAction CreateWithoutParameters<T>(MethodInfo method)
        {
            if (method.IsStatic)
            {
                return new StaticOnActionWithoutParameters(method);
            }

            return new OnActionWithoutParameters<T>(method);
        }
    }
}
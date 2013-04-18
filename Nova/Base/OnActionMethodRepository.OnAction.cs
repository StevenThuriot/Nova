﻿#region License
// 
//  Copyright 2012 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
#endregion

using System;
using System.Reflection;

namespace Nova.Base
{
	internal static partial class OnActionMethodRepository
	{
		/// <summary>
		/// Class to make life inside the repository easier.
		/// </summary>
		private abstract class OnAction
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
					(parameters.Length == 1 && parameters[0].ParameterType != typeof(ActionContext)))
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

        /// <summary>
        /// Static Method Without Parameters
        /// </summary>
		private class StaticOnActionWithoutParameters : OnAction
		{
            /// <summary>
            /// The action
            /// </summary>
			private readonly Action _Action;

            /// <summary>
            /// Initializes a new instance of the <see cref="StaticOnActionWithoutParameters" /> class.
            /// </summary>
            /// <param name="method">The method.</param>
			public StaticOnActionWithoutParameters(MethodInfo method)
				: base(method)
			{
				_Action = (Action)Delegate.CreateDelegate(typeof(Action), method);
			}

            /// <summary>
            /// Invokes the specified instance.
            /// </summary>
            /// <param name="instance">The instance.</param>
            /// <param name="context">The context.</param>
			public override void Invoke(object instance, ActionContext context)
			{
				_Action();
			}
		}

        /// <summary>
        /// Static Method With Parameters
        /// </summary>
		private class StaticOnActionWithParameters : OnAction
		{
            /// <summary>
            /// The action
            /// </summary>
			private readonly Action<ActionContext> _Action;

            /// <summary>
            /// Initializes a new instance of the <see cref="StaticOnActionWithParameters" /> class.
            /// </summary>
            /// <param name="method">The method.</param>
			public StaticOnActionWithParameters(MethodInfo method)
				: base(method)
			{
				_Action = (Action<ActionContext>)Delegate.CreateDelegate(typeof(Action<ActionContext>), method);
			}

            /// <summary>
            /// Invokes the specified instance.
            /// </summary>
            /// <param name="instance">The instance.</param>
            /// <param name="context">The context.</param>
			public override void Invoke(object instance, ActionContext context)
			{
				_Action(context);
			}
		}

        /// <summary>
        /// Method Without Parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
		private class OnActionWithoutParameters<T> : OnAction
		{
            /// <summary>
            /// The action
            /// </summary>
			private readonly Action<T> _Action;

            /// <summary>
            /// Initializes a new instance of the <see cref="OnActionWithoutParameters{T}" /> class.
            /// </summary>
            /// <param name="method">The method.</param>
			public OnActionWithoutParameters(MethodInfo method)
				: base(method)
			{
				_Action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), null, method);
			}

            /// <summary>
            /// Invokes the specified instance.
            /// </summary>
            /// <param name="instance">The instance.</param>
            /// <param name="context">The context.</param>
			public override void Invoke(object instance, ActionContext context)
			{
				_Action((T)instance);
			}
		}

        /// <summary>
        /// Method With Parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
		private class OnActionWithParameters<T> : OnAction
		{
            /// <summary>
            /// The action
            /// </summary>
			private readonly Action<T, ActionContext> _Action;

            /// <summary>
            /// Initializes a new instance of the <see cref="OnActionWithParameters{T}" /> class.
            /// </summary>
            /// <param name="method">The method.</param>
			public OnActionWithParameters(MethodInfo method)
				: base(method)
			{
				_Action = (Action<T, ActionContext>)Delegate.CreateDelegate(typeof(Action<T, ActionContext>), null, method);
			}

            /// <summary>
            /// Invokes the specified instance.
            /// </summary>
            /// <param name="instance">The instance.</param>
            /// <param name="context">The context.</param>
			public override void Invoke(object instance, ActionContext context)
			{
				_Action((T)instance, context);
			}
		}
	}
}

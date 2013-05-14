using System;
using System.Reflection;

#region License

// 
//  Copyright 2013 Steven Thuriot
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

namespace Nova.Library.ActionMethodRepository
{
    internal partial class OnAction
    {
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
            /// Initializes a new instance of the <see cref="OnAction.StaticOnActionWithoutParameters" /> class.
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
            /// Initializes a new instance of the <see cref="OnAction.StaticOnActionWithParameters" /> class.
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
            /// Initializes a new instance of the <see cref="OnAction.OnActionWithoutParameters{T}" /> class.
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
            /// Initializes a new instance of the <see cref="OnAction.OnActionWithParameters{T}" /> class.
            /// </summary>
            /// <param name="method">The method.</param>
            public OnActionWithParameters(MethodInfo method)
                : base(method)
            {
                _Action =
                    (Action<T, ActionContext>)Delegate.CreateDelegate(typeof(Action<T, ActionContext>), null, method);
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

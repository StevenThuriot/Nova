#region License
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
			public string Name { get; private set; }

			public abstract void Invoke(object instance, ActionContext context);

			protected OnAction(MethodBase method)
			{
				Name = method.Name;
			}

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

			private static OnAction CreateWithParameters<T>(MethodInfo method)
			{
				if (method.IsStatic)
				{
					return new StaticOnActionWithParameters(method);
				}

				return new OnActionWithParameters<T>(method);
			}

			private static OnAction CreateWithoutParameters<T>(MethodInfo method)
			{
				if (method.IsStatic)
				{
					return new StaticOnActionWithoutParameters(method);
				}

				return new OnActionWithoutParameters<T>(method);
			}
		}

		private class StaticOnActionWithoutParameters : OnAction
		{
			private readonly Action _Action;

			public StaticOnActionWithoutParameters(MethodInfo method)
				: base(method)
			{
				_Action = (Action)Delegate.CreateDelegate(typeof(Action), method);
			}

			public override void Invoke(object instance, ActionContext context)
			{
				_Action();
			}
		}

		private class StaticOnActionWithParameters : OnAction
		{
			private readonly Action<ActionContext> _Action;

			public StaticOnActionWithParameters(MethodInfo method)
				: base(method)
			{
				_Action = (Action<ActionContext>)Delegate.CreateDelegate(typeof(Action<ActionContext>), method);
			}

			public override void Invoke(object instance, ActionContext context)
			{
				_Action(context);
			}
		}

		private class OnActionWithoutParameters<T> : OnAction
		{
			private readonly Action<T> _Action;

			public OnActionWithoutParameters(MethodInfo method)
				: base(method)
			{
				_Action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), null, method);
			}

			public override void Invoke(object instance, ActionContext context)
			{
				_Action((T)instance);
			}
		}

		private class OnActionWithParameters<T> : OnAction
		{
			private readonly System.Action<T, ActionContext> _Action;

			public OnActionWithParameters(MethodInfo method)
				: base(method)
			{
				_Action = (System.Action<T, ActionContext>)Delegate.CreateDelegate(typeof(System.Action<T, ActionContext>), null, method);
			}

			public override void Invoke(object instance, ActionContext context)
			{
				_Action((T)instance, context);
			}
		}
	}
}

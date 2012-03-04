using System;
using System.Windows.Threading;

namespace Nova.Controls
{
	/// <summary>
	/// The interface for a view used in this framework.
	/// </summary>
	public interface IView
	{
		/// <summary>
		/// Starts the loading.
		/// </summary>
		void StartLoading();

		/// <summary>
		/// Stops the loading.
		/// </summary>
		void StopLoading();

		/// <summary>
		/// Invokes the specified action on the main thread.
		/// </summary>
		/// <param name="work">The work.</param>
		void InvokeOnMainThread(Action work);

		/// <summary>
		/// Invokes the specified action on the main thread.
		/// </summary>
		/// <param name="work">The work.</param>
		/// <param name="priority">The priority.</param>
		void InvokeOnMainThread(Action work, DispatcherPriority priority);
	}
}
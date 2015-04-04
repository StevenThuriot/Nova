namespace Nova.Library
{
	/// <summary>
	/// How to dispose a view's controls.
	/// </summary>
	public enum TreeType
	{
		/// <summary>
		/// Don't try to dispose controls.
		/// </summary>
		None = 0,
		
		/// <summary>
		/// Dispose the logical Tree
		/// </summary>
		LogicalTree = 1,

		/// <summary>
		/// Dispose the Visual Tree
		/// </summary>
		VisualTree = 2
	}
}

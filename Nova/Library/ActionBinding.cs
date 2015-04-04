using System;
using System.Windows.Data;

namespace Nova.Library
{
    /// <summary>
    /// A binding class to make binding to actions easier
    /// </summary>
    public class ActionBinding : Binding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBinding" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
		public ActionBinding(string path)
			: base("ActionManager." + path)
		{
			Mode = BindingMode.OneWay;
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBinding" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ActionBinding(Type type)
            :this(type.Name)
        {
            
        }
    }
}

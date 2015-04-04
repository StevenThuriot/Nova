using System;

namespace Nova.Library
{
    /// <summary>
    /// Attribute to give actions an alias.
    /// </summary>
    /// <remarks>Useful for OnBefore/After logic.
    /// e.g. OnAfterEnter when the action is not called Enter, 
    /// or for reusing methods for multiple actions.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string Alias { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasAttribute" /> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        public AliasAttribute(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new Exception("An alias can not be null, empty or whitespace.");

            Alias = alias.Replace(" ", "");
        }
    }
}

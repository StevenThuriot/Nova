namespace Nova.Shell.Converters
{
    /// <summary>
    /// Or Converter
    /// </summary>
    public class OrConverter : AggregateConverter<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrConverter" /> class.
        /// </summary>
        public OrConverter() : base((current, next) => current || next)
        {
        }
    }
}

using System;
using System.Diagnostics;

namespace SenseNet.Search.Querying
{
    /// <summary>
    /// Represents a name-value pair in the querying and indexing.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Name}:{ValueAsString}:{Type}")]
    public class SnTerm : IndexValue
    {
        /// <summary>
        /// Initializes an instance of the SnTerm with a named System.String value
        /// </summary>
        /// <param name="name">The name of the term.</param>
        /// <param name="value">System.String value</param>
        public SnTerm(string name, string value) : base(value) { Name = name; }
        /// <summary>
        /// Initializes an instance of the SnTerm with a named array of System.String value.
        /// </summary>
        /// <param name="name">The name of the term.</param>
        /// <param name="value">Array of System.String value</param>
        public SnTerm(string name, string[] value) : base(value) { Name = name; }
        /// <summary>
        /// Initializes an instance of the SnTerm with a named System.Boolean value
        /// </summary>
        /// <param name="name">The name of the term.</param>
        /// <param name="value">System.Boolean value</param>
        public SnTerm(string name, bool value) : base(value) { Name = name; }
        /// <summary>
        /// Initializes an instance of the SnTerm with a named System.Int32 value
        /// </summary>
        /// <param name="name">The name of the term.</param>
        /// <param name="value">System.Int32 value</param>
        public SnTerm(string name, int value) : base(value) { Name = name; }
        /// <summary>
        /// Initializes an instance of the SnTerm with a named System.Int64 value
        /// </summary>
        /// <param name="name">The name of the term.</param>
        /// <param name="value">System.Int64 value</param>
        public SnTerm(string name, long value) : base(value) { Name = name; }
        /// <summary>
        /// Initializes an instance of the SnTerm with a named System.Single value
        /// </summary>
        /// <param name="name">The name of the term.</param>
        /// <param name="value">System.Single value</param>
        public SnTerm(string name, float value) : base(value) { Name = name; }
        /// <summary>
        /// Initializes an instance of the SnTerm with a named System.Double value
        /// </summary>
        /// <param name="name">The name of the term.</param>
        /// <param name="value">System.Double value</param>
        public SnTerm(string name, double value) : base(value) { Name = name; }
        /// <summary>
        /// Initializes an instance of the SnTerm with a named System.DateTime value
        /// </summary>
        /// <param name="name">The name of the term.</param>
        /// <param name="value">System.DateTime value</param>
        public SnTerm(string name, DateTime value) : base(value) { Name = name; }

        /// <summary>
        /// Gets the name of the term.
        /// </summary>
        public string Name { get; }

        public override string ToString()
        {
            return $"{Name}:{base.ToString()}";
        }
        public static SnTerm Parse(string snTerm)
        {
            if (string.IsNullOrEmpty(snTerm))
                throw new ArgumentNullException(nameof(snTerm));

            // Find first and last index of the ':' character. We do it this way
            // instead of splitting because datetime values contain the same
            // character in the middle.
            var firstSeparatorIndex = snTerm.IndexOf(':');
            var lastSeparatorIndex = snTerm.LastIndexOf(':');
            if (firstSeparatorIndex < 1 || lastSeparatorIndex < 3 || 
                firstSeparatorIndex >= lastSeparatorIndex - 1 ||
                lastSeparatorIndex > snTerm.Length - 2)
                throw new InvalidOperationException($"Invalid SnTerm: {snTerm}");

            var name = snTerm.Substring(0, firstSeparatorIndex);
            var value = snTerm.Substring(firstSeparatorIndex + 1, lastSeparatorIndex - firstSeparatorIndex - 1);
            var typeFlag = snTerm.Substring(lastSeparatorIndex + 1);
            var valueType = (IndexValueType)TypeFlags.IndexOf(typeFlag, StringComparison.Ordinal);

            switch (valueType)
            {
                case IndexValueType.String: return new SnTerm(name, value);
                case IndexValueType.StringArray: throw new NotSupportedException();
                case IndexValueType.Bool: return new SnTerm(name, value == Yes);
                case IndexValueType.Int: return new SnTerm(name, int.Parse(value));
                case IndexValueType.Long: return new SnTerm(name, long.Parse(value));
                case IndexValueType.Float: return new SnTerm(name, float.Parse(value));
                case IndexValueType.Double: return new SnTerm(name, double.Parse(value));
                case IndexValueType.DateTime: return new SnTerm(name, DateTime.Parse(value));
                default:
                    throw new ArgumentException($"Unknown IndexValueType: {valueType}.");
            }
        }
    }
}

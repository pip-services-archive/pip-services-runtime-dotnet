using System.Collections.Generic;

namespace PipServices.Runtime.Validation
{
    /// <summary>
    ///     Represents a validation schema for object property.
    ///     The schema can use simple types like: "string", "number", "object", "DummyObject"
    ///     or specific schemas for object values
    /// </summary>
    public class PropertySchema
    {
        private readonly List<IPropertyValidationRule> _rules = new List<IPropertyValidationRule>();

        /// <summary>
        ///     Creates instance of the object property schema defined by a simple type
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="array">The array flag</param>
        /// <param name="type">The simple value type</param>
        /// <param name="optional">The optional flag</param>
        /// <param name="rules">A list of validation rules</param>
        public PropertySchema(string name, bool array, string type, bool optional,
            IEnumerable<IPropertyValidationRule> rules)
        {
            Name = name;
            IsArray = array;
            IsOptional = optional;
            Type = type;

            if (rules != null)
                _rules.AddRange(rules);
        }

        /// <summary>
        ///     Creates instance of the object property schema defined by complex schema
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="array">The array flag</param>
        /// <param name="schema">The value type schema</param>
        /// <param name="optional">The optional flag</param>
        /// <param name="rules">A list of validation rules</param>
        public PropertySchema(string name, bool array, Schema schema, bool optional,
            IEnumerable<IPropertyValidationRule> rules)
        {
            Name = name;
            IsArray = array;
            IsOptional = optional;
            Schema = schema;

            if (rules != null)
                _rules.AddRange(rules);
        }

        /// <summary>
        ///     The property name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The array flag
        /// </summary>
        public bool IsArray { get; }

        /// <summary>
        ///     The optional flag
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        ///     The simple type to define property value.
        ///     Supported the following types: 'int', 'float', 'long', 'number'
        ///     'string', 'boolean', 'object', 'array', 'map', 'TypeName'
        /// </summary>
        public string Type { get; }

        /// <summary>
        ///     The complex value schema
        /// </summary>
        public Schema Schema { get; }

        /// <summary>
        ///     The collection of validation rules
        /// </summary>
        public IList<IPropertyValidationRule> Rules
        {
            get { return _rules; }
        }
    }
}
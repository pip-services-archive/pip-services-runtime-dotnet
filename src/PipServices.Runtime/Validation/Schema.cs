using System.Collections.Generic;

namespace PipServices.Runtime.Validation
{
    /// <summary>
    ///     Represents a validation schema for complex objects.
    /// </summary>
    public class Schema
    {
        private readonly List<PropertySchema> _properties = new List<PropertySchema>();
        private readonly List<IValidationRule> _rules = new List<IValidationRule>();

        /// <summary>
        ///     Gets a list of object properties
        /// </summary>
        public IList<PropertySchema> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        ///     Gets a list of validation rules for entire object
        /// </summary>
        public IList<IValidationRule> Rules
        {
            get { return _rules; }
        }

        /// <summary>
        ///     Adds to the validation schema a required property defined by a simple type.
        /// </summary>
        /// <param name="name">A name of the property to be added</param>
        /// <param name="type">A simple type that defines the property value</param>
        /// <param name="rules">A set of validation rules for the property</param>
        /// <returns>A self reference to the schema for chaining</returns>
        public Schema WithProperty(string name, string type, params IPropertyValidationRule[] rules)
        {
            _properties.Add(new PropertySchema(name, false, type, false, rules));
            return this;
        }

        /// <summary>
        ///     Adds to the validation schema a required property array defined by a simple type.
        /// </summary>
        /// <param name="name">A name of the property to be added</param>
        /// <param name="type">A simple type that defines the property values</param>
        /// <param name="rules">A set of validation rules for the property</param>
        /// <returns>A self reference to the schema for chaining</returns>
        public Schema WithArray(string name, string type, params IPropertyValidationRule[] rules)
        {
            _properties.Add(new PropertySchema(name, false, type, false, rules));
            return this;
        }

        /// <summary>
        ///     Adds to the validation schema an optional property defined by a simple type.
        /// </summary>
        /// <param name="name">A name of the property to be added</param>
        /// <param name="type">A simple type that defines the property value</param>
        /// <param name="rules">A set of validation rules for the property</param>
        /// <returns>A self reference to the schema for chaining</returns>
        public Schema WithOptionalProperty(string name, string type, params IPropertyValidationRule[] rules)
        {
            _properties.Add(new PropertySchema(name, false, type, true, rules));
            return this;
        }

        /// <summary>
        ///     Adds to the validation schema an optional property array defined by a simple type.
        /// </summary>
        /// <param name="name">A name of the property to be added</param>
        /// <param name="type">A simple type that defines the property value</param>
        /// <param name="rules">A set of validation rules for the property</param>
        /// <returns>A self reference to the schema for chaining</returns>
        public Schema WithOptionalArray(string name, string type, params IPropertyValidationRule[] rules)
        {
            _properties.Add(new PropertySchema(name, true, type, true, rules));
            return this;
        }

        /// <summary>
        ///     Adds to the validation schema a required property defined by validation schema.
        /// </summary>
        /// <param name="name">A name of the property to be added</param>
        /// <param name="type">A simple type that defines the property value</param>
        /// <param name="rules">A set of validation rules for the property</param>
        /// <returns>A self reference to the schema for chaining</returns>
        public Schema WithPropertySchema(string name, Schema schema, params IPropertyValidationRule[] rules)
        {
            _properties.Add(new PropertySchema(name, false, schema, false, rules));
            return this;
        }

        /// <summary>
        ///     Adds to the validation schema a required property array defined by validation schema.
        /// </summary>
        /// <param name="name">A name of the property to be added</param>
        /// <param name="type">A simple type that defines the property value</param>
        /// <param name="rules">A set of validation rules for the property</param>
        /// <returns>A self reference to the schema for chaining</returns>
        public Schema WithArraySchema(string name, Schema schema, params IPropertyValidationRule[] rules)
        {
            _properties.Add(new PropertySchema(name, true, schema, false, rules));
            return this;
        }

        /// <summary>
        ///     Adds to the validation schema an optional property defined by validation schema.
        /// </summary>
        /// <param name="name">A name of the property to be added</param>
        /// <param name="type">A simple type that defines the property value</param>
        /// <param name="rules">A set of validation rules for the property</param>
        /// <returns>A self reference to the schema for chaining</returns>
        public Schema WithOptionalPropertySchema(string name, Schema schema, params IPropertyValidationRule[] rules)
        {
            _properties.Add(new PropertySchema(name, false, schema, true, rules));
            return this;
        }

        /// <summary>
        ///     Adds to the validation schema an optional property array defined by validation schema.
        /// </summary>
        /// <param name="name">A name of the property to be added</param>
        /// <param name="type">A simple type that defines the property value</param>
        /// <param name="rules">A set of validation rules for the property</param>
        /// <returns>A self reference to the schema for chaining</returns>
        public Schema WithOptionalArraySchema(string name, Schema schema, params IPropertyValidationRule[] rules)
        {
            _properties.Add(new PropertySchema(name, true, schema, true, rules));
            return this;
        }

        /// <summary>
        ///     Ads a validation rule to this schema
        /// </summary>
        /// <param name="rule">a validation rule to be added</param>
        /// <returns>a self reference to the schema for chaining</returns>
        public Schema WithRule(IValidationRule rule)
        {
            _rules.Add(rule);
            return this;
        }
    }
}
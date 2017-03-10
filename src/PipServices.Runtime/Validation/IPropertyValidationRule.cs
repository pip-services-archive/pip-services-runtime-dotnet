using System.Collections.Generic;
using PipServices.Runtime.Errors;

namespace PipServices.Runtime.Validation
{
    /// <summary>
    ///     Interface for object property validation rule.
    ///     If can performs validation for a specified object property.
    ///     For instance, it check for valid range, allowed or disallowed values.
    /// </summary>
    public interface IPropertyValidationRule
    {
        /// <summary>
        ///     Validates object property according to the schema and the rule.
        /// </summary>
        /// <param name="schema">a property schema this rule belongs to</param>
        /// <param name="value">the property value to be validated.</param>
        /// <returns>a list of validation errors or empty list if validation passed.</returns>
        IList<MicroserviceError> Validate(PropertySchema schema, object value);
    }
}
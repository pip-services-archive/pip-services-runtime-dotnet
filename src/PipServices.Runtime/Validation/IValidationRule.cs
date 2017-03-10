using System.Collections.Generic;
using PipServices.Runtime.Errors;

namespace PipServices.Runtime.Validation
{
    /// <summary>
    ///     Interface for object schema validation rule.
    ///     If can performs overall validation across the entire object.
    ///     For instance, it can check presence of one of several required properties.
    /// </summary>
    public interface IValidationRule
    {
        /// <summary>
        ///     Validates object according to the schema and the rule.
        /// </summary>
        /// <param name="schema">an object schema this rule belongs to</param>
        /// <param name="value">the object value to be validated.</param>
        /// <returns>a list of validation errors or empty list if validation passed.</returns>
        IList<MicroserviceError> Validate(Schema schema, object value);
    }
}
using System.ComponentModel.DataAnnotations;
using Fleck;
using lib;

namespace api.ClientEventFilters;

public class ValidateDataAnnotations : BaseEventFilter
{
    public override Task Handle<T>(IWebSocketConnection socket, T dto)
    {
        ValidateObjectRecursive(dto); // Call the recursive validation method
        return Task.CompletedTask;
    }

    private static void ValidateObjectRecursive(object obj)
    {
        var validationContext = new ValidationContext(obj);
        var validationResults = new List<ValidationResult>();

        // Validate the object
        Validator.TryValidateObject(obj, validationContext, validationResults, validateAllProperties: true);

        // If the object is complex and contains nested objects, recursively validate them
        foreach (var property in obj.GetType().GetProperties())
        {
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                var nestedObject = property.GetValue(obj);
                if (nestedObject != null)
                {
                    ValidateObjectRecursive(nestedObject);
                }
            }
        }

        // If validation fails, throw an exception
        if (validationResults.Any())
        {
            throw new ValidationException($"Validation failed for object of type {obj.GetType().Name}: {string.Join("; ", validationResults.Select(r => r.ErrorMessage))}");
        }
    }
}

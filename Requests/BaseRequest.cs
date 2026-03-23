using System.ComponentModel.DataAnnotations;

namespace BuzzIt.Requests
{
    /// <summary>
    /// Base class for all request patterns with common validation logic
    /// </summary>
    public abstract class BaseRequest : IRequest
    {
        protected readonly Dictionary<string, string> _errors = new();

        public abstract bool IsValid();

        public Dictionary<string, string> GetErrors()
        {
            return _errors;
        }

        protected void AddError(string field, string message)
        {
            _errors[field] = message;
        }

        protected void ClearErrors()
        {
            _errors.Clear();
        }

        protected bool ValidateRequired(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError(fieldName, $"{fieldName} is required.");
                return false;
            }
            return true;
        }

        protected bool ValidateMaxLength(string? value, int maxLength, string fieldName)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                AddError(fieldName, $"{fieldName} must not exceed {maxLength} characters.");
                return false;
            }
            return true;
        }

        protected bool ValidateRange(int value, int min, int max, string fieldName)
        {
            if (value < min || value > max)
            {
                AddError(fieldName, $"{fieldName} must be between {min} and {max}.");
                return false;
            }
            return true;
        }
    }
}

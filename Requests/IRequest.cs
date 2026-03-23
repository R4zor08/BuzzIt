namespace BuzzIt.Requests
{
    /// <summary>
    /// Base interface for all request patterns
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Validates the request data
        /// </summary>
        bool IsValid();

        /// <summary>
        /// Gets validation errors if any
        /// </summary>
        Dictionary<string, string> GetErrors();
    }
}

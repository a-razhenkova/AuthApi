using System.Diagnostics.CodeAnalysis;

namespace WebApi.V1
{
    public class SimpleResponseModel<TValue>
    {
        [SetsRequiredMembers]
        public SimpleResponseModel(TValue value)
        {
            Value = value;
        }

        /// <summary>
        /// Value.
        /// </summary>
        /// <example>value</example>
        public required TValue Value { get; set; }
    }
}
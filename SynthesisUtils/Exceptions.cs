using Mutagen.Bethesda.Skyrim;

namespace Synthesis.Util
{
    public class ConditionNotFoundException : Exception
    {
        private static readonly string _defaultMessage =
            "Unable to find condition matching the given criteria";
        public readonly IConditionGetter? Condition;

        public ConditionNotFoundException(
            string? message = null,
            IConditionGetter? condition = null
        )
            : base(message ?? _defaultMessage)
        {
            Condition = condition;
        }
    }
}

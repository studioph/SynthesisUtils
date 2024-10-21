using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Synthesis.Util
{
    /// <summary>
    /// Value object containing a source record and the corresponding winning context
    /// </summary>
    /// <typeparam name="TMod"></typeparam>
    /// <typeparam name="TModGetter"></typeparam>
    /// <typeparam name="TMajor"></typeparam>
    /// <typeparam name="TMajorGetter"></typeparam>
    /// <param name="Source"></param>
    /// <param name="Winning"></param>
    public sealed record ForwardRecordContext<TMod, TModGetter, TMajor, TMajorGetter>(
        TMajorGetter Source,
        IModContext<TMod, TModGetter, TMajor, TMajorGetter> Winning
    )
        where TMod : IMod, TModGetter
        where TModGetter : IModGetter
        where TMajor : TMajorGetter
        where TMajorGetter : IMajorRecordQueryableGetter;

    /// <summary>
    /// Value object containing the winning record context and the data needed to patch the record
    /// </summary>
    /// <typeparam name="TMod"></typeparam>
    /// <typeparam name="TModGetter"></typeparam>
    /// <typeparam name="TMajor"></typeparam>
    /// <typeparam name="TMajorGetter"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="Context"></param>
    /// <param name="Values"></param>
    public sealed record PatchingData<TMod, TModGetter, TMajor, TMajorGetter, TValue>(
        IModContext<TMod, TModGetter, TMajor, TMajorGetter> Context,
        TValue Values
    )
        where TMod : IMod, TModGetter
        where TModGetter : IModGetter
        where TMajor : TMajorGetter
        where TMajorGetter : IMajorRecordQueryableGetter
        where TValue : notnull;
}

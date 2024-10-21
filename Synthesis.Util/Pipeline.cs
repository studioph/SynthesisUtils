using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Synthesis.Util
{
    public abstract class PatcherPipeline<
        TMod,
        TModGetter,
        TMajor,
        TMajorGetter,
        TPatcher,
        TValue,
        TData
    >(TMod patchMod)
        where TMod : IMod, TModGetter
        where TModGetter : IModGetter
        where TMajor : TMajorGetter
        where TMajorGetter : IMajorRecordQueryableGetter
        where TPatcher : IPatcher<TMajor, TMajorGetter, TValue>
        where TValue : notnull
        where TData : notnull
    {
        protected readonly TMod _patchMod = patchMod;
        public uint PatchedCount { get; protected set; } = 0;

        public abstract IEnumerable<
            PatchingData<TMod, TModGetter, TMajor, TMajorGetter, TValue>
        > GetRecordsToPatch(TPatcher patcher, IEnumerable<TData> records);

        public void PatchRecord(
            PatchingData<TMod, TModGetter, TMajor, TMajorGetter, TValue> item,
            TPatcher patcher
        )
        {
            var target = item.Context.GetOrAddAsOverride(_patchMod);
            patcher.Patch(target, item.Values);
            PatchedCount++;
            Update(target);
        }

        public void PatchRecords(
            TPatcher patcher,
            IEnumerable<PatchingData<TMod, TModGetter, TMajor, TMajorGetter, TValue>> recordsToPatch
        )
        {
            foreach (var item in recordsToPatch)
            {
                PatchRecord(item, patcher);
            }
        }

        /// <summary>
        /// Callback when a record is patched
        /// </summary>
        /// <param name="updated"></param>
        protected virtual void Update(TMajorGetter updated) { }
    }

    /// <summary>
    /// A pipeline for applying multiple forwarding-style patchers to multiple collections of records
    /// </summary>
    /// <typeparam name="TMajor"></typeparam>
    /// <typeparam name="TMajorGetter"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class ForwardPatcherPipeline<TMod, TModGetter, TMajor, TMajorGetter, TValue>(
        TMod patchMod
    )
        : PatcherPipeline<
            TMod,
            TModGetter,
            TMajor,
            TMajorGetter,
            IForwardPatcher<TMajor, TMajorGetter, TValue>,
            TValue,
            ForwardRecordContext<TMod, TModGetter, TMajor, TMajorGetter>
        >(patchMod)
        where TMod : IMod, TModGetter
        where TModGetter : IModGetter
        where TMajor : TMajorGetter
        where TMajorGetter : IMajorRecordQueryableGetter
        where TValue : notnull
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TMod"></typeparam>
        /// <typeparam name="TModGetter"></typeparam>
        /// <param name="patcher"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public override IEnumerable<
            PatchingData<TMod, TModGetter, TMajor, TMajorGetter, TValue>
        > GetRecordsToPatch(
            IForwardPatcher<TMajor, TMajorGetter, TValue> patcher,
            IEnumerable<ForwardRecordContext<TMod, TModGetter, TMajor, TMajorGetter>> records
        ) =>
            records
                .Select(item => new PatchingData<TMod, TModGetter, TMajor, TMajorGetter, TValue>(
                    item.Winning,
                    patcher.Analyze(item.Source, item.Winning.Record)
                ))
                .Where(result => patcher.ShouldPatch(result.Values));
    }

    public abstract class TransformPatcherPipeline<TMod, TModGetter, TMajor, TMajorGetter, TValue>(
        TMod patchMod
    )
        : PatcherPipeline<
            TMod,
            TModGetter,
            TMajor,
            TMajorGetter,
            ITransformPatcher<TMajor, TMajorGetter, TValue>,
            TValue,
            IModContext<TMod, TModGetter, TMajor, TMajorGetter>
        >(patchMod)
        where TMod : IMod, TModGetter
        where TModGetter : IModGetter
        where TMajor : TMajorGetter
        where TMajorGetter : IMajorRecordQueryableGetter
        where TValue : notnull
    {
        public override IEnumerable<
            PatchingData<TMod, TModGetter, TMajor, TMajorGetter, TValue>
        > GetRecordsToPatch(
            ITransformPatcher<TMajor, TMajorGetter, TValue> patcher,
            IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> records
        ) =>
            records
                .Where(context => patcher.Filter(context.Record))
                .Select(context => new PatchingData<TMod, TModGetter, TMajor, TMajorGetter, TValue>(
                    context,
                    patcher.Apply(context.Record)
                ));
    }

    public abstract class ConditionalTransformPatcherPipeline<
        TMod,
        TModGetter,
        TMajor,
        TMajorGetter,
        TValue
    >(TMod patchMod)
        : TransformPatcherPipeline<TMod, TModGetter, TMajor, TMajorGetter, TValue>(patchMod)
        where TMod : IMod, TModGetter
        where TModGetter : IModGetter
        where TMajor : TMajorGetter
        where TMajorGetter : IMajorRecordQueryableGetter
        where TValue : notnull
    {
        public IEnumerable<
            PatchingData<TMod, TModGetter, TMajor, TMajorGetter, TValue>
        > GetRecordsToPatch(
            IConditionalTransformPatcher<TMajor, TMajorGetter, TValue> patcher,
            IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> records
        ) =>
            base.GetRecordsToPatch(patcher, records)
                .Where(result => patcher.ShouldPatch(result.Values));
    }
}

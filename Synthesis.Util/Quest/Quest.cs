using Mutagen.Bethesda.Skyrim;

namespace Synthesis.Util.Quest
{
    public interface IForwardQuestPatcher<TValue> : IForwardPatcher<IQuest, IQuestGetter, TValue>
        where TValue : notnull { }

    public class QuestPatcherPipeline<TValue>(ISkyrimMod patchMod)
        : ForwardPatcherPipeline<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter, TValue>(
            patchMod
        )
        where TValue : notnull
    {
        protected override void Update(IQuestGetter quest) =>
            Console.WriteLine($"Patched QUST:{quest.FormKey}({quest.EditorID})");
    }
}

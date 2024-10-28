using Mutagen.Bethesda.Skyrim;

namespace Synthesis.Util.Cell
{
    public class CellForwarderPipeline(ISkyrimMod patchMod)
        : ForwardPatcherPipeline<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(patchMod)
    {
        protected override void Update(ICellGetter cell) =>
            Console.WriteLine($"Patched CELL:{cell.FormKey}({cell.EditorID})");
    }
}

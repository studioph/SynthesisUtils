using Mutagen.Bethesda.Skyrim;

namespace Synthesis.Util.Cell
{
    /// <summary>
    /// Generic interface for forwarding Cell properties
    /// </summary>
    public interface ICellForwardPatcher<TValue> : IForwardPatcher<ICell, ICellGetter, TValue>
        where TValue : notnull { }

    public class CellPatcherPipeline<TValue>(ISkyrimMod patchMod)
        : ForwardPatcherPipeline<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter, TValue>(patchMod)
        where TValue : notnull
    {
        protected override void Update(ICellGetter cell)
        {
            Console.WriteLine($"Patched CELL:{cell.FormKey}({cell.EditorID})");
        }
    }
}

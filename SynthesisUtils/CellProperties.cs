using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace Synthesis.Utils
{
    /// <summary>
    /// Generic interface for forwarding Cell properties
    /// </summary>
    public interface ICellPropertyPatcher
    {
        /// <summary>
        /// Checks if the property for the given cell needs patching
        /// </summary>
        /// <param name="sourceCell">The source Cell to use as the base to compare against.</param>
        /// <param name="targetCell">The target Cell to compare to.</param>
        /// <returns></returns>
        bool NeedsPatch(ICellGetter sourceCell, ICellGetter targetCell);

        /// <summary>
        /// Patches the property for the given cell
        /// </summary>
        /// <param name="sourceCell">The source Cell to use the property value from.</param>
        /// <param name="targetCell">The target Cell to update with the new property value.</param>
        void PatchCellProperty(ICellGetter sourceCell, ICell targetCell);
    }

    /// <summary>
    /// Class to pipeline patching multiple properties of cells.
    /// </summary>
    /// <typeparam name="TMod">The writable mod type</typeparam>
    /// <typeparam name="TModGetter">The readonly mod type</typeparam>
    public class CellPropertiesPatcher<TMod, TModGetter>
    where TMod : IMod, TModGetter
    where TModGetter : IModGetter
    {
        private IEnumerable<ICellPropertyPatcher> Patchers { get; }
        private readonly TMod PatchMod;

        public CellPropertiesPatcher(TMod patchMod, params ICellPropertyPatcher[] patchers)
        {
            PatchMod = patchMod;
            Patchers = patchers;
        }

        public CellPropertiesPatcher(
            TMod patchMod,
            IEnumerable<ICellPropertyPatcher> patchers
        )
            : this(patchMod, patchers.ToArray()) { }

        /// <summary>
        /// Patches the given collection of cells using the patchers contained within the pipeline instance.
        /// Winning cell contexts must be writable (i.e. not simple contexts)
        /// </summary>
        /// <param name="cells">A collection of (source cell context, winning cell context) pairs</param>
        public void PatchCells(IEnumerable<Tuple<IModContext<ICellGetter>, IModContext<TMod, TModGetter, ICell, ICellGetter>>> cells)
        {
            foreach (var (sourceContext, winningContext) in cells)
            {
                var sourceCell = sourceContext.Record;
                var targetCell = winningContext.Record;
                foreach (var patcher in Patchers)
                {
                    if (patcher.NeedsPatch(sourceCell, targetCell))
                    {
                        patcher.PatchCellProperty(
                            sourceCell,
                            winningContext.GetOrAddAsOverride(PatchMod)
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Patches the given collection of cells using the patchers contained within the pipeline instance.
        /// Winning cell contexts must be writable (i.e. not simple contexts)
        /// </summary>
        /// <param name="cells">A collection of (source cell context, winning cell context) pairs</param>
        public void PatchCells(IEnumerable<KeyValuePair<IModContext<ICellGetter>, IModContext<TMod, TModGetter, ICell, ICellGetter>>> cells)
        {
            PatchCells(cells.Select(pair => Tuple.Create(pair.Key, pair.Value)));
        }
    }
}
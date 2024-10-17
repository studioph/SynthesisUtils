using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace Synthesis.Util.Cell
{
    /// <summary>
    /// Helpers for working with Cell records
    /// </summary>
    public static class CellUtil
    {
        /// <summary>
        /// Checks if a cell is part of a worldspace
        /// </summary>
        /// <param name="cellContext">The cell to check</param>
        /// <returns>True if the cell is a worldspace cell</returns>
        public static bool IsWorldspaceCell(this IModContext<ICellGetter> cellContext)
        {
            return cellContext.TryGetParent<IWorldspaceGetter>(out _);
        }
    }
}
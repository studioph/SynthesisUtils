using System.Collections.Immutable;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Synthesis.Util.Quest
{
    /// <summary>
    /// Generic utility class for working with quest aliases
    /// </summary>
    public static class QuestAliasUtil
    {
        /// <summary>
        /// Searches for an instance of a condition using the specified search criteria
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="searchFunc"></param>
        /// <returns></returns>
        public static IConditionGetter? FindAliasCondition(
            IQuestGetter quest,
            Func<IConditionGetter, bool> searchFunc
        )
        {
            return quest
                .Aliases.SelectMany(alias => alias.Conditions)
                .Where(searchFunc)
                .FirstOrDefault();
        }

        /// <summary>
        /// Searches for an instance of a condition using the specified search criteria
        /// </summary>
        /// <param name="quests"></param>
        /// <param name="searchFunc"></param>
        /// <returns></returns>
        public static IConditionGetter? FindAliasCondition(
            IEnumerable<IQuestGetter> quests,
            Func<IConditionGetter, bool> searchFunc
        )
        {
            return quests
                .SelectMany(quest => quest.Aliases)
                .SelectMany(alias => alias.Conditions)
                .Where(searchFunc)
                .FirstOrDefault();
        }

        /// <summary>
        /// Checks if the given alias has a condition that satisfies the given predicate
        /// </summary>
        /// <param name="alias">The quest alias to check</param>
        /// <returns>True if the alias already contains a condition satisfying the predicate</returns>
        public static bool HasCondition(
            IQuestAliasGetter alias,
            Func<IConditionGetter, bool> searchFunc
        ) => alias.Conditions.Where(condition => searchFunc(condition)).Any();

        /// <summary>
        /// Checks if the given alias has the condition
        /// </summary>
        /// <param name="alias">The quest alias to check</param>
        /// <returns>True if the alias already contains the condition</returns>
        public static bool HasCondition(IQuestAliasGetter alias, IConditionGetter condition) =>
            alias.Conditions.Where(cond => cond.Equals(condition)).Any();

        /// <summary>
        /// Finds the quest aliases that contain the condition
        /// </summary>
        /// <param name="quest">The quest to search</param>
        /// <returns>The quest aliases which contain the condition</returns>
        public static IEnumerable<IQuestAliasGetter> GetAliasesWithCondition(
            IQuestGetter quest,
            Func<IConditionGetter, bool> searchFunc
        ) => quest.Aliases.Where(alias => HasCondition(alias, searchFunc));

        /// <summary>
        /// Finds the quest aliases that contain the condition
        /// </summary>
        /// <param name="quest">The quest to search</param>
        /// <returns>The quest aliases which contain the condition</returns>
        public static IEnumerable<IQuestAliasGetter> GetAliasesWithCondition(
            IQuestGetter quest,
            IConditionGetter condition
        ) => quest.Aliases.Where(alias => HasCondition(alias, condition));
    }

    /// <summary>
    /// Class for adding a single condition to quest aliases
    /// </summary>
    /// <param name="condition">The condition to patch quest aliases with</param>
    /// <param name="searchFunc">Optional search function to check if a quest aliases needs patching. Default is to check for the existance of the target condition</param>
    public class QuestAliasConditionPatcher(
        IConditionGetter condition,
        Func<IConditionGetter, bool>? searchFunc = null
    )
    {
        /// <summary>
        /// The target condition to patch quest aliases with
        /// </summary>
        public readonly IConditionGetter Condition = condition;

        /// <summary>
        /// Optional search criteria to use when checking if a quest alias already contains the target condition, instead of checking for the whole condition object
        /// </summary>
        private readonly Func<IConditionGetter, bool>? SearchFunc = searchFunc;

        public readonly IDictionary<IQuestGetter, IList<IQuestAliasGetter>> PatchedRecords =
            new Dictionary<IQuestGetter, IList<IQuestAliasGetter>>();

        /// <summary>
        /// Gets the alias IDs of the quest aliases which contain the condition
        /// </summary>
        /// <param name="quest">The quest to search</param>
        /// <returns>The IDs of the quest aliases which contain the condition</returns>
        private IEnumerable<uint> GetAliasIDsWithCondition(IQuestGetter quest)
        {
            // Prefer searchFunc if present since that implies there's special circumstances
            var aliases = SearchFunc is not null
                ? QuestAliasUtil.GetAliasesWithCondition(quest, SearchFunc)
                : QuestAliasUtil.GetAliasesWithCondition(quest, Condition);

            return aliases.Select(alias => alias.ID);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="requiredAliases"></param>
        /// <returns></returns>
        private IEnumerable<uint> GetAliasesToPatch(
            IQuestGetter quest,
            IEnumerable<uint> requiredAliases
        )
        {
            var actualAliases = GetAliasIDsWithCondition(quest);
            var requiredSet = new HashSet<uint>(requiredAliases);
            return requiredSet.Except(actualAliases);
        }

        /// <summary>
        /// Patches the quest by adding the condition to relevant aliases
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="aliases"></param>
        private void PatchQuestAliases(IQuest quest, IEnumerable<uint> aliases)
        {
            Console.WriteLine($"Patching quest: {quest.EditorID}");
            var aliasMap = quest.Aliases.ToImmutableDictionary(alias => alias.ID);
            foreach (var aliasId in aliases)
            {
                var alias = aliasMap[aliasId];
                alias.Conditions.Add(Condition.DeepCopy());
                Console.WriteLine($"Added condition to alias: {alias.Name}");
                PatchedRecords.GetOrAdd(quest, () => new List<IQuestAliasGetter>()).Add(alias);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="state"></param>
        public void PatchQuest(
            IQuestGetter quest,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state
        )
        {
            var affectedAliases = GetAliasIDsWithCondition(quest);
            var winningQuest = quest.ToLinkGetter().TryResolve(state.LinkCache);

            if (winningQuest is null)
            {
                Console.WriteLine($"Unable to resolve FormKey: {quest.FormKey}");
                return;
            }

            var aliasesToPatch = GetAliasesToPatch(winningQuest, affectedAliases);
            if (aliasesToPatch.Any())
            {
                var patchQuest = state.PatchMod.Quests.GetOrAddAsOverride(winningQuest);
                PatchQuestAliases(patchQuest, aliasesToPatch);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="quests"></param>
        /// <param name="state"></param>
        public void PatchAll(
            IEnumerable<IQuestGetter> quests,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state
        )
        {
            foreach (var quest in quests)
            {
                PatchQuest(quest, state);
            }

            var totalAliasesPatched = PatchedRecords.Aggregate(
                0,
                (sum, next) => sum + next.Value.Count
            );
            Console.WriteLine(
                $"Patched {totalAliasesPatched} aliases across {PatchedRecords.Count} quests"
            );
        }
    }
}

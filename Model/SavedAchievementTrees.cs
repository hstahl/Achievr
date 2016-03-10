using System.Collections.Generic;

namespace Achievr.Model
{
    class SavedAchievementTrees
    {
        public SavedAchievementTrees(ICollection<AchievementTree> savedTrees = null)
        {
            if (savedTrees != null)
                this.SavedTrees = savedTrees;
            else
                this.SavedTrees = new HashSet<AchievementTree>();
        }

        public ICollection<AchievementTree> SavedTrees { get; set; }

        public void UpdateAdd(AchievementTree tree)
        {
            SavedTrees.Add(tree);
        }
    }
}

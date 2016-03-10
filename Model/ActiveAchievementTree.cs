namespace Achievr.Model
{
    public class ActiveAchievementTree
    {
        public AchievementTree ActiveTree { get; set; }

        public void Update(AchievementTree tree)
        {
            ActiveTree = tree;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Achievr
{
    public class AchievementTree
    {
        String name;
        ICollection<AchievementNode> nodes;

        public AchievementTree(string name)
        {
            this.name = name;
            nodes = new HashSet<AchievementNode>();
        }

        public class AchievementNode
        {
            Achievement node;
            ICollection<AchievementNode> depends_on;
            Tuple<int, int> coordinates;

            internal protected AchievementNode(Achievement node, int x, int y)
            {
                this.node = node;
                coordinates = new Tuple<int,int>(x, y);
                depends_on = new HashSet<AchievementNode>();
            }

            public Achievement GetNode()
            {
                return node;
            }

            public Tuple<int,int> GetLocationOnCanvas()
            {
                return coordinates;
            }

            public ICollection<AchievementNode> GetDependencies()
            {
                return depends_on;
            }

            internal void AddDependency(AchievementNode dependency)
            {
                depends_on.Add(dependency);
            }

            internal void RemoveDependency(AchievementNode dependency)
            {
                depends_on.Remove(dependency);
            }
        }

        public ICollection<AchievementNode> GetNodes()
        {
            return nodes;
        }

        public int GetTotalScore(bool unlocked)
        {
            int result = 0;
            foreach (AchievementNode a in nodes)
            {
                if (unlocked)
                    result += a.GetNode().IsUnlocked() ? a.GetNode().GetScoreValue() : 0;
                else
                    result += a.GetNode().GetScoreValue();
            }
            return result;
        }

        public void AddNode(Achievement node, int x, int y, params AchievementNode[] dependencies)
        {
            AchievementNode n = new AchievementNode(node, x, y);
            foreach (AchievementNode a in dependencies)
                n.AddDependency(a);
            nodes.Add(n);
        }

        public static void AddNodeDepedency(AchievementNode node, AchievementNode dependency)
        {
            node.AddDependency(dependency);
        }

        public static void RemoveNodeDependency(AchievementNode node, AchievementNode dependency)
        {
            node.RemoveDependency(dependency);
        }
    }
}

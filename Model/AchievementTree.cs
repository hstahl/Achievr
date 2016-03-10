using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Achievr.Model
{
    public class AchievementTree
    {
        public string Name { get; }
        public ICollection<AchievementNode> Nodes { get; }
        public string Score
        {
            get { return GetTotalScore(true) + "/" +GetTotalScore(false); }
        }

        public AchievementTree(string name)
        {
            this.Name = name;
            this.Nodes = new HashSet<AchievementNode>();
        }

        public class AchievementNode
        {
            public Achievement Node { get; }
            public ICollection<AchievementNode> DependsOn { get; set; }
            public Tuple<int, int> Coordinates { get; set; }

            internal protected AchievementNode(Achievement node, int x, int y)
            {
                this.Node = node;
                this.Coordinates = new Tuple<int,int>(x, y);
                this.DependsOn = new HashSet<AchievementNode>();
            }
        }

        private int GetTotalScore(bool unlocked)
        {
            int result = 0;
            foreach (AchievementNode a in Nodes)
            {
                if (unlocked)
                    result += a.Node.Unlocked ? a.Node.ScoreValue : 0;
                else
                    result += a.Node.ScoreValue;
            }
            return result;
        }

        public void AddNode(Achievement node, int x, int y, params AchievementNode[] dependencies)
        {
            AchievementNode n = new AchievementNode(node, x, y);
            foreach (AchievementNode a in dependencies)
                n.DependsOn.Add(a);
            Nodes.Add(n);
        }

        public static void AddNodeDepedency(AchievementNode node, AchievementNode dependency)
        {
            node.DependsOn.Add(dependency);
        }

        public static void RemoveNodeDependency(AchievementNode node, AchievementNode dependency)
        {
            node.DependsOn.Remove(dependency);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

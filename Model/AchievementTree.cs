using System;
using System.Collections.Generic;

namespace Achievr.Model
{
    public class AchievementTree
    {
        public string Name { get; set; }
        public ICollection<AchievementNode> Nodes { get; set; }
        public string Score
        {
            get { return GetTotalScore(true) + "/" +GetTotalScore(false); }
        }

        public AchievementTree()
        {
            this.Nodes = new List<AchievementNode>();
        }

        public AchievementTree(string name)
        {
            this.Name = name;
            this.Nodes = new List<AchievementNode>();
        }

        public class AchievementNode
        {
            public Achievement Node { get; set; }
            public ICollection<AchievementNode> DependsOn { get; set; }
            public Tuple<int, int> Coordinates { get; set; }

            public AchievementNode()
            {
                this.DependsOn = new List<AchievementNode>();
            }

            internal protected AchievementNode(Achievement node, int x, int y)
            {
                this.Node = node;
                this.Coordinates = new Tuple<int,int>(x, y);
                this.DependsOn = new List<AchievementNode>();
            }

            public void UpdateAddDependency(AchievementNode dependency)
            {
                DependsOn.Add(dependency);
            }

            public void UpdateDeleteDependency(AchievementNode dependency)
            {
                DependsOn.Remove(dependency);
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

        public void DeleteNode(AchievementNode node)
        {
            Nodes.Remove(node);
        }

        public override string ToString()
        {
            return Name;
        }

        public void UpdateAdd(AchievementNode node)
        {
            Nodes.Add(node);
        }
    }
}

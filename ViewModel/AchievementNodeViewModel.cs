using Achievr.Model;
using System;
using System.Collections.Generic;

namespace Achievr.ViewModel
{
    public class AchievementNodeViewModel : NotificationBase<AchievementTree.AchievementNode>
    {
        public AchievementNodeViewModel(AchievementTree.AchievementNode node = null) : base(node) { }

        public Achievement Node
        {
            get { return This.Node; }
            set { SetProperty(This.Node, value, () => This.Node = value); }
        }

        public ICollection<AchievementTree.AchievementNode> DependsOn
        {
            get { return This.DependsOn; }
            set { SetProperty(This.DependsOn, value, () => This.DependsOn = value); }
        }

        public Tuple<int, int> Coordinates
        {
            get { return This.Coordinates; }
            set { SetProperty(This.Coordinates, value, () => This.Coordinates = value); }
        }
    }
}

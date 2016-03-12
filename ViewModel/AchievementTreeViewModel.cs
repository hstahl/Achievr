using Achievr.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Achievr.ViewModel
{
    public class AchievementTreeViewModel : NotificationBase<AchievementTree>
    {
        public AchievementTreeViewModel(AchievementTree tree = null) : base(tree)
        {
            this.ScoreValue = 10;
        }

        public string Name
        {
            get { return This.Name; }
            set { SetProperty(This.Name, value, () => This.Name = value); }
        }

        public string Score
        {
            get { return This.Score; }
        }

        private ObservableCollection<AchievementNodeViewModel> _Nodes
            = new ObservableCollection<AchievementNodeViewModel>();
        public ObservableCollection<AchievementNodeViewModel> Nodes
        {
            get { return _Nodes; }
            set { SetProperty(ref _Nodes, value); }
        }

        private int _SelectedIndex = -1;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (SetProperty(ref _SelectedIndex, value))
                {
                    RaisePropertyChanged(nameof(SelectedNode));
                }
            }
        }

        public AchievementNodeViewModel SelectedNode
        {
            get { return (_SelectedIndex >= 0) ? _Nodes[_SelectedIndex] : null; }
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public bool Unlocked { get; set; }
        public int ScoreValue { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public AchievementNodeViewModel Dependency { get; set; }

        public void AddNode()
        {
            var node = new AchievementNodeViewModel();
            node.PropertyChanged += AchievementNode_OnNotifyPropertyAdded;
            node.Node = new Achievement(Title, Description, ScoreValue);
            node.Node.Unlocked = Unlocked;
            node.Coordinates = new Tuple<int, int>(X, Y);
            node.DependsOn = new List<AchievementTree.AchievementNode>();
            Nodes.Add(node);
            SelectedIndex = Nodes.IndexOf(node);
        }

        public void DeleteNode()
        {
            if (SelectedIndex >= 0)
            {
                var node = Nodes[SelectedIndex];
                Nodes.RemoveAt(SelectedIndex);
                This.DeleteNode(node);
            }
        }

        void AchievementNode_OnNotifyPropertyAdded(object sender, PropertyChangedEventArgs e)
        {
            This.UpdateAdd((AchievementNodeViewModel)sender);
        }
    }
}

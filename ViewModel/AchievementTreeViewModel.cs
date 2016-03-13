using Achievr.Model;
using System;
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

        private string _Score = "";
        public string Score
        {
            get { return _Score; }
            set { SetProperty(ref _Score, value); }
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

        // Values for creating new nodes. These fields work as content for new
        // achievement and edit achievement menus.
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { SetProperty(ref _Description, value); }
        }
        private int _ScoreValue = 10;
        public int ScoreValue
        {
            get { return _ScoreValue; }
            set { SetProperty(ref _ScoreValue, value); }
        }
        public int X { get; set; }
        public int Y { get; set; }

        public void AddNode()
        {
            var node = new AchievementNodeViewModel();
            This.UpdateAdd(node);
            node.Node = new Achievement(Title, Description, ScoreValue);
            node.Node.Unlocked = false;
            node.Coordinates = new Tuple<int, int>(X, Y);
            Nodes.Add(node);
            Score = This.Score;
            SelectedIndex = Nodes.IndexOf(node);
            UpdateAvailableDependencies();
        }

        public void DeleteNode()
        {
            if (SelectedIndex >= 0)
            {
                var node = Nodes[SelectedIndex];
                Nodes.RemoveAt(SelectedIndex);
                This.DeleteNode(node);
                Score = This.Score;
                UpdateAvailableDependencies();
            }
        }

        // Toggle Unlocked state iff node has no unlocked dependencies.
        // Toggle all unlocked dependent nodes if this node is toggled to locked
        public void ToggleAchieved()
        {
            if (SelectedIndex >= 0)
            {
                if (Nodes[SelectedIndex].Node.Unlocked)
                {
                    for (int i = 0; i < Nodes.Count; i++)
                    {
                        if (Nodes[i].DependsOn.Contains(Nodes[SelectedIndex]))
                        {
                            if (Nodes[i].Node.Unlocked)
                            {
                                int temp = SelectedIndex;
                                SelectedIndex = i;
                                ToggleAchieved();
                                SelectedIndex = temp;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var dependency in Nodes[SelectedIndex].DependsOn)
                    {
                        if (!dependency.Node.Unlocked)
                            return;
                    }
                }
                Nodes[SelectedIndex].Node.Unlocked =
                    !Nodes[SelectedIndex].Node.Unlocked;
                Score = This.Score;
            }
        }

        // Use this to set score values or the UI won't be updated.
        public void SetScoreValue()
        {
            if (SelectedIndex >= 0)
            {
                Nodes[SelectedIndex].Node.ScoreValue = ScoreValue;
                Score = This.Score;
            }
        }

        // Available dependencies are achievement nodes which are not yet
        // dependencies and are not the node itself
        void UpdateAvailableDependencies()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                var depList = new ObservableCollection<AchievementNodeViewModel>();
                for (int j = 0; j < Nodes.Count; j++)
                {
                    if (j == i)
                        continue;
                    if (Nodes[i].DependsOn.Contains(Nodes[j]))
                        continue;
                    depList.Add(Nodes[j]);
                }
                Nodes[i].AvailableDependencies = depList;
            }
        }
    }
}

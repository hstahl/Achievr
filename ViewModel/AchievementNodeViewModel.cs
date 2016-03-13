using Achievr.Model;
using System;
using System.Collections.ObjectModel;

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

        private ObservableCollection<AchievementNodeViewModel> _DependsOn
            = new ObservableCollection<AchievementNodeViewModel>();
        public ObservableCollection<AchievementNodeViewModel> DependsOn
        {
            get { return _DependsOn; }
            set { SetProperty(ref _DependsOn, value); }
        }

        private int _SelectedDependencyIndex = -1;
        public int SelectedDependencyIndex
        {
            get { return _SelectedDependencyIndex; }
            set
            {
                if (SetProperty(ref _SelectedDependencyIndex, value))
                {
                    RaisePropertyChanged(nameof(SelectedDependency));
                }
            }
        }

        public AchievementNodeViewModel SelectedDependency
        {
            get { return (SelectedDependencyIndex >= 0) ? DependsOn[SelectedDependencyIndex] : null; }
        }

        private ObservableCollection<AchievementNodeViewModel> _AvailableDependencies
            = new ObservableCollection<AchievementNodeViewModel>();
        public ObservableCollection<AchievementNodeViewModel> AvailableDependencies
        {
            get { return _AvailableDependencies; }
            set { SetProperty(ref _AvailableDependencies, value); }
        }

        private int _SelectedAvailableDependencyIndex = -1;
        public int SelectedAvailableDependencyIndex
        {
            get { return _SelectedAvailableDependencyIndex; }
            set
            {
                if (SetProperty(ref _SelectedAvailableDependencyIndex, value))
                {
                    RaisePropertyChanged(nameof(SelectedAvailableDependency));
                }
            }
        }

        public AchievementNodeViewModel SelectedAvailableDependency
        {
            get { return (SelectedAvailableDependencyIndex >= 0)
                    ? AvailableDependencies[SelectedAvailableDependencyIndex] : null; }
        }

        public void AddDependency()
        {
            if (SelectedAvailableDependencyIndex >= 0)
            {
                var dependency = SelectedAvailableDependency;
                This.UpdateAddDependency(dependency);
                DependsOn.Add(dependency);
                SelectedDependencyIndex = DependsOn.IndexOf(dependency);
                AvailableDependencies.Remove(dependency);
            }
        }

        public void DeleteDependency()
        {
            if (SelectedDependencyIndex >= 0)
            {
                var dependency = SelectedDependency;
                This.UpdateDeleteDependency(dependency);
                AvailableDependencies.Add(dependency);
                SelectedAvailableDependencyIndex = AvailableDependencies.IndexOf(dependency);
                DependsOn.Remove(dependency);
            }
        }

        public Tuple<int, int> Coordinates
        {
            get { return This.Coordinates; }
            set { SetProperty(This.Coordinates, value, () => This.Coordinates = value); }
        }
    }
}

using Achievr.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Achievr.ViewModel
{
    public class SavedTreesViewModel : NotificationBase
    {
        SavedAchievementTrees savedTrees;
        
        public SavedTreesViewModel()
        {
            savedTrees = new SavedAchievementTrees();
            _SelectedIndex = -1;
        }

        private ObservableCollection<AchievementTreeViewModel> _Trees
            = new ObservableCollection<AchievementTreeViewModel>();
        public ObservableCollection<AchievementTreeViewModel> Trees
        {
            get { return _Trees; }
            set { SetProperty(ref _Trees, value); }
        }

        private int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (SetProperty(ref _SelectedIndex, value))
                {
                    RaisePropertyChanged(nameof(SelectedTree));
                }
            }
        }

        public AchievementTreeViewModel SelectedTree
        {
            get { return (SelectedIndex >= 0) ? Trees[SelectedIndex] : null; }
        }

        public string NewTreeName { get; set; }
        public void AddTree()
        {
            var tree = new AchievementTreeViewModel();
            tree.PropertyChanged += SavedTrees_OnNotifyPropertyAdded;
            tree.Name = NewTreeName;
            Trees.Add(tree);
            SelectedIndex = Trees.IndexOf(tree);
        }

        public void DeleteTree()
        {
            if (SelectedIndex >= 0)
            {
                var tree = Trees[SelectedIndex];
                Trees.RemoveAt(SelectedIndex);
                savedTrees.SavedTrees.Remove(tree);
            }
        }

        public void SavedTrees_OnNotifyPropertyAdded(object sender, PropertyChangedEventArgs e)
        {
            savedTrees.UpdateAdd((AchievementTreeViewModel)sender);
        }
    }
}

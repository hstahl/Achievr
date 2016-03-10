using Achievr.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Achievr.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private AchievementTree activeTree;
        private ObservableCollection<AchievementTree> savedTrees;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public MainPageViewModel(AchievementTree activeTree, ICollection<AchievementTree> savedTrees)
        {
            this.activeTree = activeTree;
            this.savedTrees = new ObservableCollection<AchievementTree>(savedTrees);
        }

        public ObservableCollection<AchievementTree> SavedTrees
        {
            get { return this.savedTrees; }
            set
            {
                this.savedTrees = value;
                this.OnPropertyChanged();
            }
        }

        public AchievementTree ActiveTree
        {
            get { return this.activeTree; }
            set
            {
                this.activeTree = value;
                this.OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using Achievr.Model;
using System.ComponentModel;

namespace Achievr.ViewModel
{
    public class ActiveAchievementTreeViewModel : NotificationBase
    {
        private ActiveAchievementTree activeTree;

        public ActiveAchievementTreeViewModel()
        {
            activeTree = new ActiveAchievementTree();
        }

        private AchievementTreeViewModel _Active = null;
        public AchievementTreeViewModel Active
        {
            get { return _Active; }
            set { SetProperty(ref _Active, value); }
        }

        private bool _HasTreeLoaded = false;
        public bool HasTreeLoaded
        {
            get { return _HasTreeLoaded; }
            set { SetProperty(ref _HasTreeLoaded, value); }
        }

        public AchievementTreeViewModel NextActive { get; set; }
        public void Activate()
        {
            if (NextActive != null)
            {
                NextActive.PropertyChanged += Active_OnNotifyPropertyChanged;
                Active = NextActive;
                HasTreeLoaded = true;
            }
        }

        public void Active_OnNotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            activeTree.Update((AchievementTreeViewModel)sender);
        }
    }
}

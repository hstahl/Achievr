using Achievr.ViewModel;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Achievr
{
    public sealed partial class MainPage : Page
    {
        public SavedTreesViewModel SavedTreesViewModel { get; set; }
        public ActiveAchievementTreeViewModel ActiveTreeViewModel { get; set; }

        public MainPage()
        {
            InitializeComponent();

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var saved = localSettings.Values["Saved"] as string;
            if (saved != null)
                this.SavedTreesViewModel = SavedTreesViewModel.Deserialize(saved);
            else
                this.SavedTreesViewModel = new SavedTreesViewModel();
            this.ActiveTreeViewModel = new ActiveAchievementTreeViewModel();
            var app = App.Current as App;
            app.Saved = SavedTreesViewModel;

            DrawAchievementTreeOnCanvas();
        }

        private void DrawAchievementTreeOnCanvas()
        {
            mainCanvas.Children.Clear();
            if (!ActiveTreeViewModel.HasTreeLoaded)
                return;
            foreach (AchievementNodeViewModel a in this.ActiveTreeViewModel.Active.Nodes)
            {
                SolidColorBrush colorUnlocked = Resources["achievement-unlocked-color"] as SolidColorBrush;
                SolidColorBrush colorLocked = Resources["achievement-locked-color"] as SolidColorBrush;
                Grid achievementContainer = new Grid();
                achievementContainer.Style = Resources["alt-achievement-container-style"] as Style;
                achievementContainer.Background = a.Node.Unlocked
                    ? colorUnlocked
                    : colorLocked;
                achievementContainer.PointerPressed += Achievement_Click;
                Canvas.SetLeft(achievementContainer, a.Coordinates.Item1 - 40);
                Canvas.SetTop(achievementContainer, a.Coordinates.Item2 - 40);
                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                RowDefinition row3 = new RowDefinition();
                row1.Height = new GridLength(13.0);
                row2.Height = new GridLength(1.0, GridUnitType.Star);
                row3.Height = new GridLength(13.0);
                achievementContainer.RowDefinitions.Add(row1);
                achievementContainer.RowDefinitions.Add(row2);
                achievementContainer.RowDefinitions.Add(row3);
                mainCanvas.Children.Add(achievementContainer);
                TextBlock scoreText = new TextBlock();
                scoreText.Style = Resources["achievement-score-text"] as Style;
                scoreText.Text = a.Node.ScoreValue.ToString();
                achievementContainer.Children.Add(scoreText);
                TextBlock titleText = new TextBlock();
                titleText.Style = Resources["achievement-title-text"] as Style;
                titleText.Text = a.Node.Title;
                achievementContainer.Children.Add(titleText);
                foreach (AchievementNodeViewModel d in a.DependsOn)
                {
                    Line depArrow = new Line();
                    depArrow.Style = Resources["achievement-dependency-line"] as Style;
                    depArrow.X1 = a.Coordinates.Item1;
                    depArrow.Y1 = a.Coordinates.Item2;
                    depArrow.X2 = d.Coordinates.Item1;
                    depArrow.Y2 = d.Coordinates.Item2;
                    depArrow.Stroke = d.Node.Unlocked
                        ? colorUnlocked
                        : colorLocked;
                    mainCanvas.Children.Add(depArrow);
                }
            }
        }

        private void hamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = false;
        }

        private void NewAchievementCreateButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NewAchievementNameBox.Text;
            if (name == null || name.Length == 0)
                return;
            SavedTreesViewModel.NewTreeName = name;
            SavedTreesViewModel.AddTree();
            NewAchievementNameBox.Text = "";
            NewAchievementTreeNameDialog.Hide();
        }

        public void UpdateActiveTree()
        {
            if (SavedTreesViewModel.SelectedIndex < 0)
                return;
            ActiveTreeViewModel.NextActive = SavedTreesViewModel.Trees[SavedTreesViewModel.SelectedIndex];
            ActiveTreeViewModel.Activate();
            DrawAchievementTreeOnCanvas();
        }

        private void OpenTreeButton_Click(object sender, RoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = false;
            UpdateActiveTree();
        }

        private void mainCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!ActiveTreeViewModel.HasTreeLoaded)
                return;
            bool? editingEnabled = editButton.IsChecked;
            if (!editingEnabled.HasValue)
                editingEnabled = false;
            if ((bool)editingEnabled)
            {
                PointerPoint ptrPt = e.GetCurrentPoint(mainCanvas);
                double x, y;
                x = ptrPt.Position.X;
                y = ptrPt.Position.Y;
                ActiveTreeViewModel.Active.X = (int)x;
                ActiveTreeViewModel.Active.Y = (int)y;
                ActiveTreeViewModel.Active.Title = null;
                ActiveTreeViewModel.Active.Description = null;
                ActiveTreeViewModel.Active.ScoreValue = 10;
                NewAchievementMenu.ShowAt(this);
            }
        }

        private void NewAchievementMenuDoneButton_Click(object sender, RoutedEventArgs e)
        {
            NewAchievementMenu.Hide();
            ActiveTreeViewModel.Active.AddNode();
            // Back to defaults but not in a pretty way
            ActiveTreeViewModel.Active.Title = null;
            ActiveTreeViewModel.Active.Description = null;
            ActiveTreeViewModel.Active.ScoreValue = 10;
            // Update canvas
            DrawAchievementTreeOnCanvas();
        }

        private void Achievement_Click(object sender, PointerRoutedEventArgs e)
        {
            int indexOfAchievement = mainCanvas.Children.IndexOf((Grid)sender);
            for (int i = indexOfAchievement; i >= 0; i--)
            {   // a hack to avoid counting dependency lines towards index
                if (mainCanvas.Children[i].GetType() != typeof(Grid))
                    indexOfAchievement--;
            }
            ActiveTreeViewModel.Active.SelectedIndex = indexOfAchievement;
            bool? editingEnabled = editButton.IsChecked;
            if (!editingEnabled.HasValue)
                editingEnabled = false;
            if ((bool)editingEnabled)
            {
                ActiveTreeViewModel.Active.ScoreValue
                    = ActiveTreeViewModel.Active.SelectedNode.Node.ScoreValue;
                EditAchievementMenu.ShowAt(this);
            }
            else
            {
                ActiveTreeViewModel.Active.ToggleAchieved();
                DrawAchievementTreeOnCanvas();
            }
            e.Handled = true;
        }

        private void EditAchievementDoneButton_Click(object sender, RoutedEventArgs e)
        {
            ActiveTreeViewModel.Active.SelectedNode.Node.ScoreValue
                = ActiveTreeViewModel.Active.ScoreValue;
            ActiveTreeViewModel.Active.SetScoreValue();
            EditAchievementMenu.Hide();
            DrawAchievementTreeOnCanvas();
        }

        private void EditAchievementDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            EditAchievementMenu.Hide();
            ActiveTreeViewModel.Active.DeleteNode();
            DrawAchievementTreeOnCanvas();
        }
    }
}

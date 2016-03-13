using Achievr.Model;
using Achievr.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Achievr
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
                StackPanel achievementContainer = new StackPanel();
                achievementContainer.Style = Resources["achievement-container-style"] as Style;
                achievementContainer.PointerPressed += Achievement_Click;
                Canvas.SetLeft(achievementContainer, a.Coordinates.Item1);
                Canvas.SetTop(achievementContainer, a.Coordinates.Item2);
                mainCanvas.Children.Add(achievementContainer);
                Image icon = new Image();
                icon.Style = Resources[a.Node.Unlocked ? "achievement-icon-unlocked" : "achievement-icon-locked"] as Style;
                achievementContainer.Children.Add(icon);
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
            int indexOfAchievement = mainCanvas.Children.IndexOf((StackPanel)sender);
            for (int i = indexOfAchievement; i >= 0; i--)
            {   // a hack to avoid counting dependency lines towards index
                if (mainCanvas.Children[i].GetType() != typeof(StackPanel))
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

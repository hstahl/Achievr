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
            AchievementTree activeTree = (AchievementTree)localSettings.Values["activeTree"];
            ICollection<AchievementTree>savedTrees = (List<AchievementTree>)localSettings.Values["savedTrees"];
            if (savedTrees == null)
                savedTrees = new List<AchievementTree>();
            if (activeTree != null)
                titleText.Text = activeTree.ToString();

            this.SavedTreesViewModel = new SavedTreesViewModel();
            this.ActiveTreeViewModel = new ActiveAchievementTreeViewModel();

            AchievementTree testTree = new AchievementTree("Test Tree");
            Achievement testAchievement = new Achievement("Test", "Just a test!");
            testAchievement.ToggleUnlocked();
            testTree.AddNode(testAchievement, 50, 20);
            Achievement depAchievement = new Achievement("Deps", "Dep Test");
            testTree.AddNode(depAchievement, 200, 200, testTree.Nodes.First());

            DrawAchievementTreeOnCanvas();
        }

        private void DrawAchievementTreeOnCanvas()
        {
            mainCanvas.Children.Clear();
            if (ActiveTreeViewModel.Active == null)
                return;
            foreach (AchievementTree.AchievementNode a in this.ActiveTreeViewModel.Active.Nodes)
            {
                StackPanel achievementContainer = new StackPanel();
                achievementContainer.Style = Resources["achievement-container-style"] as Style;
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
                foreach (AchievementTree.AchievementNode d in a.DependsOn)
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
    }
}

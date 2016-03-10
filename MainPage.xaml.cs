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
        AchievementTree activeTree;
        ArrayList savedTrees;

        public MainPage()
        {
            InitializeComponent();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            activeTree = (AchievementTree)localSettings.Values["activeTree"];
            savedTrees = (ArrayList)localSettings.Values["savedTrees"];
            if (savedTrees == null)
                savedTrees = new ArrayList();
            if (activeTree != null)
                titleText.Text = activeTree.ToString();
            AchievementTree testTree = new AchievementTree("Test Tree");
            Achievement testAchievement = new Achievement("Test", "Just a test!");
            testAchievement.ToggleUnlocked();
            testTree.AddNode(testAchievement, 50, 20);
            Achievement depAchievement = new Achievement("Deps", "Dep Test");
            testTree.AddNode(depAchievement, 200, 200, testTree.GetNodes().First());
            DrawAchievementTreeOnCanvas(testTree, mainCanvas);
        }

        private void DrawAchievementTreeOnCanvas(AchievementTree tree, Canvas canvas)
        {
            canvas.Children.Clear();
            foreach (AchievementTree.AchievementNode a in tree.GetNodes())
            {
                StackPanel achievementContainer = new StackPanel();
                achievementContainer.Style = Resources["achievement-container-style"] as Style;
                Canvas.SetLeft(achievementContainer, a.GetLocationOnCanvas().Item1);
                Canvas.SetTop(achievementContainer, a.GetLocationOnCanvas().Item2);
                canvas.Children.Add(achievementContainer);
                Image icon = new Image();
                icon.Style = Resources[a.GetNode().IsUnlocked() ? "achievement-icon-unlocked" : "achievement-icon-locked"] as Style;
                achievementContainer.Children.Add(icon);
                TextBlock titleText = new TextBlock();
                titleText.Style = Resources["achievement-title-text"] as Style;
                titleText.Text = a.GetNode().GetName();
                achievementContainer.Children.Add(titleText);
                foreach (AchievementTree.AchievementNode d in a.GetDependencies())
                {
                    Line depArrow = new Line();
                    depArrow.Style = Resources["achievement-dependency-line"] as Style;
                    depArrow.X1 = a.GetLocationOnCanvas().Item1;
                    depArrow.Y1 = a.GetLocationOnCanvas().Item2;
                    depArrow.X2 = d.GetLocationOnCanvas().Item1;
                    depArrow.Y2 = d.GetLocationOnCanvas().Item2;
                    canvas.Children.Add(depArrow);
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
            UpdateActiveTree(new AchievementTree(name));
            NewAchievementTreeNameDialog.Hide();
            mainSplitView.IsPaneOpen = false;
        }

        public void UpdateActiveTree(AchievementTree tree)
        {
            if (activeTree != null)
            {
                Stack tempStack = new Stack(savedTrees);
                tempStack.Push(activeTree);
                savedTrees = new ArrayList(tempStack);
            }
            activeTree = tree;
            titleText.Text = activeTree.ToString();
            DrawAchievementTreeOnCanvas(activeTree, mainCanvas);
        }
    }
}

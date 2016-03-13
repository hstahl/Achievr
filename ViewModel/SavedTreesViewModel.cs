using Achievr.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;

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

        /// <summary>
        /// Serialize the object and its members to XML. We only need the
        /// Trees collection to reconstruct everything.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            var doc = new XmlDocument();
            var rootElement = doc.CreateElement("Trees");
            doc.AppendChild(rootElement);
            foreach (var tree in Trees)
            {
                var t = doc.CreateElement("Tree");

                var name = doc.CreateElement("Name");
                name.InnerText = tree.Name;

                var _nodes = doc.CreateElement("_Nodes");
                foreach(var node in tree.Nodes)
                {
                    var n = doc.CreateElement("Node");

                    var title = doc.CreateElement("Title");
                    title.InnerText = node.Node.Title;

                    var description = doc.CreateElement("Description");
                    description.InnerText = node.Node.Description;

                    var unlocked = doc.CreateElement("Unlocked");
                    unlocked.InnerText = node.Node.Unlocked.ToString();

                    var scoreValue = doc.CreateElement("ScoreValue");
                    scoreValue.InnerText = node.Node.ScoreValue.ToString();

                    var _depends = doc.CreateElement("_DependsOn");
                    foreach (var dep in node.DependsOn)
                    {
                        var index = tree.Nodes.IndexOf(dep);
                        var d = doc.CreateElement("index");
                        d.InnerXml = index.ToString();
                        _depends.AppendChild(d);
                    }

                    var coords = doc.CreateElement("Coordinates");
                    var x = doc.CreateElement("x");
                    var y = doc.CreateElement("y");
                    x.InnerText = node.Coordinates.Item1.ToString();
                    y.InnerText = node.Coordinates.Item2.ToString();
                    coords.AppendChild(x);
                    coords.AppendChild(y);

                    n.AppendChild(title);
                    n.AppendChild(description);
                    n.AppendChild(unlocked);
                    n.AppendChild(scoreValue);
                    n.AppendChild(_depends);
                    n.AppendChild(coords);

                    _nodes.AppendChild(n);
                }

                t.AppendChild(name);
                t.AppendChild(_nodes);

                rootElement.AppendChild(t);
            }
            return doc.OuterXml;
        }

        public static SavedTreesViewModel Deserialize(string xml)
        {
            var vm = new SavedTreesViewModel();
            var doc = new XmlDocument();
            doc.InnerXml = xml;

            for (int i = 0; i < doc["Trees"].ChildNodes.Count; i++)
            {
                var name = doc["Trees"].ChildNodes[i]["Name"].InnerText;
                vm.NewTreeName = name;
                vm.AddTree();

                for (int j = 0; j < doc["Trees"].ChildNodes[i]["_Nodes"].ChildNodes.Count; j++)
                {
                    var node = doc["Trees"].ChildNodes[i]["_Nodes"].ChildNodes[j];

                    vm.SelectedTree.Title = node["Title"].InnerText;
                    vm.SelectedTree.Description = node["Description"].InnerText;
                    vm.SelectedTree.ScoreValue = Convert.ToInt32(node["ScoreValue"].InnerText);
                    vm.SelectedTree.X = Convert.ToInt32(node["Coordinates"]["x"].InnerText);
                    vm.SelectedTree.Y = Convert.ToInt32(node["Coordinates"]["y"].InnerText);
                    vm.SelectedTree.AddNode();

                    var unlocked = Convert.ToBoolean(node["Unlocked"].InnerText);
                    if (unlocked)
                    {
                        vm.SelectedTree.SelectedNode.Node.ToggleUnlocked();
                        vm.SelectedTree.SetScoreValue(); // not pretty but simple
                    }
                }
                for (int j = 0; j < vm.SelectedTree.Nodes.Count; j++)
                {
                    var node = doc["Trees"].ChildNodes[i]["_Nodes"].ChildNodes[j];

                    if (node["_DependsOn"].ChildNodes.Count > 0)
                    {
                        vm.SelectedTree.SelectedIndex = j;
                        foreach (XmlNode dep in node["_DependsOn"].ChildNodes)
                        {
                            var index = Convert.ToInt32(dep.InnerText);
                            var depNode = vm.SelectedTree.Nodes[index];
                            var availableNode = vm.SelectedTree.SelectedNode.AvailableDependencies.IndexOf(depNode);
                            vm.SelectedTree.SelectedNode.SelectedAvailableDependencyIndex = availableNode;
                            vm.SelectedTree.SelectedNode.AddDependency();
                            // Need to decrement here since the list view does it for us normally
                            // as the observable collection is updated.
                            vm.SelectedTree.SelectedNode.SelectedAvailableDependencyIndex--;
                        }
                    }
                }
            }
            return vm;
        }
    }
}

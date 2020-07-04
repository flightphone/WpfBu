using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfBu.Models;
using MaterialDesignThemes;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace WpfBu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentId { get; set; }
        public Dictionary<string, RootForm> formList { get; set; }
        public ObservableCollection<RootForm> WinListSource { get; set; }
        public MainWindow()
        {
            var dat = new MainWindowModel();
            DataContext = dat;
            formList = new Dictionary<string, RootForm>();
            WinListSource = new ObservableCollection<RootForm>();
            InitializeComponent();
            WinList.ItemsSource = WinListSource;
            /*
            treeItem it = new treeItem("")
            {
                id = "81",
                attributes = new Dictionary<string, string> { { "link1", "RegulationPrint.FlightCardsList" }, { "params", "134"} }
            };
            RootForm fm = FindOrCreate(it);
            if (fm != null)
            {
                userMenu.Content = fm.userMenu;
                userContent.Content = fm.userContent;
                CurrentId = fm.id;
                MenuToggleButton.IsChecked = false;
            }
            */
            //81 134
        }
        private void MenuToggleButton_OnClick(object sender, RoutedEventArgs e)
            => mainTree.Focus();

        private void mainTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                return;
            var it = (treeItem)e.NewValue;
            if (string.IsNullOrEmpty(it.state))
            {
                RootForm fm = FindOrCreate(it);
                if (fm != null)
                {
                    userMenu.Content = fm.userMenu;
                    userContent.Content = fm.userContent;
                    CurrentId = fm.id;
                    MenuToggleButton.IsChecked = false;
                }
                else
                {
                    MessageBox.Show("Не реализовано", "uSmart-3.0", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            
        }

        private RootForm Create(treeItem it)
        {
            RootForm res = null;
            if (it.attributes["link1"] == "RegulationPrint.Dgs.DogovorList")
            {
                res = new Docs();
                return res;
            }

            if (!string.IsNullOrEmpty(it.attributes["params"]))
            {
                res = new Finder();
                return res;
            }
            return res;
        }

        private RootForm FindOrCreate(treeItem it)
        {
            
            RootForm res; 
            if (formList.ContainsKey(it.id))
            {
                res = formList[it.id];
            }
            else
            {
                res = Create(it);
                if (res != null)
                {
                    res.id = it.id;
                    res.Parent = this;
                    res.start(it.attributes["params"]);
                    formList.Add(it.id, res);
                    WinListSource.Add(res);
                }
            }
            return res;
        }

        private void RemoveList_Click(object sender, RoutedEventArgs e)
        {
            string id = ((Button)sender).Tag.ToString();
            RootForm fm = formList[id];
            WinListSource.Remove(fm);
            formList.Remove(id);
            if (id == CurrentId)
            {
                userMenu.Content = null;
                userContent.Content = null;
            }
            userContent.Focus();
        }

        private void FocusList_Click(object sender, RoutedEventArgs e)
        {
            string id = ((Button)sender).Tag.ToString();
            RootForm fm = formList[id];
            userMenu.Content = fm.userMenu;
            userContent.Content = fm.userContent;
            CurrentId = fm.id;
            userContent.Focus();
        }
    }
}

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
        private Dictionary<string, RootForm> formList { get; set; }
        private ObservableCollection<RootForm> WinListSource { get; set; }
        public MainWindow()
        {
            var dat = new MainWindowModel();
            DataContext = dat;
            formList = new Dictionary<string, RootForm>();
            WinListSource = new ObservableCollection<RootForm>();
            InitializeComponent();
            WinList.ItemsSource = WinListSource;
            //FilterGrid.ItemsSource = dat.Fcols;
            //dat.Fcols[0].FindString = "aaaa";
            //dat.Fcols[0].Sort = "По убыванию";
            
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
            if (!string.IsNullOrEmpty(it.attributes["params"]))
            {
                res = new Finder();
                res.id = it.id;
                res.start(it.attributes["params"]);
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
                /*
                res = new RootForm
                {
                    userMenu = new TextBlock() { 
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 22,
                    Text = "Не реализовано"
                    },
                    
                    new Button
                    {
                        Style = (Style)Resources["MaterialDesignFloatingActionMiniAccentButton"],
                        HorizontalAlignment = HorizontalAlignment.Left,
                        ToolTip = "Добавить",
                        Margin = new Thickness() { Left = 8 },
                        Content = new PackIcon()
                        {
                            Kind= PackIconKind.Plus,
                            Height = 24,
                            Width = 24
                        }
                    },
                    
                    userContent = new TextBox
                    {
                        TextWrapping = TextWrapping.Wrap,
                        AcceptsReturn = true,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    }
                    
                };
                */
                //HintAssist.SetHint((DependencyObject)res.userContent, "Введите текст");
                if (res != null)
                {
                    formList.Add(it.id, res);
                    WinListSource.Add(res);
                }
            }
            return res;
        }

        
    }
}

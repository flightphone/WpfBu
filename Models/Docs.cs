using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes;
using MaterialDesignThemes.Wpf;
using System.Windows.Data;
using System.Windows.Media;
using System.Data;
using System.Globalization;

namespace WpfBu.Models
{
    class Docs : Finder
    {
        public List <EditField>  Editors { get; set; }

        public DataRow WorkOrder { get; set; }

        public DataTable ParamTab { get; set; }

        public DataView WorkView { get; set; }




        public EditorList El { get; set; }
        public ParamMenu ParamText { get; set; }
        public override void AddInit(FinderMenu fm, Finder form)
        {
            Button bt = new Button
            {
                Style = (Style)form.Parent.Resources["MaterialDesignFloatingActionMiniAccentButton"],
                HorizontalAlignment = HorizontalAlignment.Left,
                ToolTip = "Параметры",
                Margin = new Thickness() { Right = 8 },
                Content = new PackIcon()
                {
                    Kind = PackIconKind.Settings,
                    Height = 24,
                    Width = 24,
                    Foreground = Brushes.MediumBlue
                },

            };

            bt.Click += (object sender, RoutedEventArgs e) =>
            {
                
                userContent.Content = El;
                userMenu.Content = ParamText;
            };


            fm.ButPanel.Children.Add(bt);
        }

        public override void start(object o)
        {
            SQLParams = new Dictionary<string, object>
            {
                { "@DateStart", new DateTime(2012, 8, 1) },
                { "@DateFinish", new DateTime(2012, 8, 10) },
                { "@AL_UTG", "<ВСЕ>"}
            };
            Editors = new List<EditField>()
            {
                new EditField()
                {
                    FieldCaption = "Начало",
                    FieldName = "DateStart",
                    DisplayFormat = "dd.MM.yyyy HH:mm",
                    FieldEditor = new TextBox(){
                        MinWidth = 300,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                },
                new EditField()
                {
                    FieldCaption = "Окончание",
                    FieldName = "DateFinish",
                    DisplayFormat = "dd.MM.yyyy HH:mm",
                    FieldEditor = new TextBox(){
                        MinWidth = 300,
                        VerticalAlignment = VerticalAlignment.Center

                    }
                },
                new EditField()
                {
                    FieldCaption = "Авиакомпания",
                    FieldName = "AL_UTG",
                    FieldEditor = new FinderEditor()
                    {
                        MinWidth= 300 
                    }
                }

            };
            El = new EditorList()
            {
                DataContext = this
            };
            ParamText = new ParamMenu();
            ParamText.Descr.Text = "Параметры";
            ParamText.ButOK.Click += (object sender, RoutedEventArgs e) =>
            {

                foreach (DataColumn c in ParamTab.Columns)
                {
                    SQLParams["@" + c.ColumnName] = WorkOrder[c.ColumnName];
                }
                UpdateTab();
                //userContent.Content = MainGrid;
                userMenu.Content = MenuControl;
            };

            ParamText.ButCancel.Click += (object sender, RoutedEventArgs e) =>
            {

                userContent.Content = MainGrid;
                userMenu.Content = MenuControl;
            };

            ParamTab = new DataTable();
            ParamTab.Columns.AddRange(new DataColumn[] {
                new DataColumn()
                {
                    ColumnName = "DateStart",
                    DataType = typeof(System.DateTime),
                    
                },
                new DataColumn()
                {
                    ColumnName = "DateFinish",
                    DataType = typeof(System.DateTime)
                },
                new DataColumn()
                {
                    ColumnName = "AL_UTG",
                    DataType = typeof(System.String)
                }
            });
            WorkOrder = ParamTab.NewRow();
            
            foreach (DataColumn c in ParamTab.Columns)
            {
                WorkOrder[c.ColumnName] = SQLParams["@" + c.ColumnName];
            }

            ParamTab.Rows.Add(WorkOrder);
            WorkView = new DataView(ParamTab);

            

            foreach (var e in Editors)
            {
                Binding bd = new Binding(e.FieldName);
                bd.Source = ParamTab;
                bd.StringFormat = e.DisplayFormat;
                bd.ConverterCulture = CultureInfo.CurrentCulture;
                
                if (e.FieldEditor.GetType() == typeof(TextBox))
                    ((TextBox)e.FieldEditor).SetBinding(TextBox.TextProperty, bd);
                if (e.FieldEditor.GetType() == typeof(FinderEditor))
                {
                    ((FinderEditor)e.FieldEditor).EditFind.SetBinding(TextBox.TextProperty, bd);
                }

            }
            JoinRow al = new JoinRow()
            {
                IdDeclare = "206",
                fields = new Dictionary<string, string>()
                {
                    {"AL_UTG", "AL_UTG" }
                }
            };
            Finder fc = new Finder();
            fc.OKFun = true;
            fc.start(al.IdDeclare);
            fc.MenuControl.ButOK.Click += (object sender, RoutedEventArgs e) =>
            {

                if (fc.MainGrid.SelectedItem == null)
                {
                    MessageBox.Show("Выберете запись", "Выбор записи");
                    return;
                }
                DataRow rw = ((DataRowView)fc.MainGrid.SelectedItem).Row;
                foreach (string s in al.fields.Keys)
                {
                    WorkOrder[al.fields[s]] = rw[s];
                }
                userContent.Content = El;
                userMenu.Content = ParamText;
            };


            fc.MenuControl.ButCancel.Click += (object sender, RoutedEventArgs e) =>
            {
                userContent.Content = El;
                userMenu.Content = ParamText;
            };

            al.FindConrol = fc;
            Editors[2].joinRow = al;

            ((FinderEditor)(Editors[2].FieldEditor)).ButFind.Click +=
                (object sender, RoutedEventArgs e) =>
                {
                    userContent.Content = Editors[2].joinRow.FindConrol.userContent;
                    userMenu.Content = Editors[2].joinRow.FindConrol.userMenu;
                };
            base.start(o);
        }
    }
}

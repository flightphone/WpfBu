using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Data;
using System.Windows.Controls;
using System.Globalization;
using System.Drawing;

namespace WpfBu.Models
{
    public class Editor : RootForm
    {
        public Finder ReferFinder { get; set; }

        public List<EditField> Editors { get; set; }

        public DataRow WorkRow { get; set; }

        public DataTable ParamTab { get; set; }

        public EditorList El { get; set; }
        public ParamMenu ParamText { get; set; }

        public virtual void Add()
        {
            ParamText.Descr.Text = "Новая запись";
            for (int i = 0; i < ParamTab.Columns.Count; i++)
                WorkRow[i] = DBNull.Value;

            ReferFinder.userContent.Content = El;
            ReferFinder.userMenu.Content = ParamText;

            //MessageBox.Show("Add");
        }

        public virtual void Edit()
        {
            if (ReferFinder.MainGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберете запись для редактирования", "Редактирование записи");
                return;
            }
            DataRow rw = ((DataRowView)ReferFinder.MainGrid.SelectedItem).Row;
            for (int i = 0; i < ParamTab.Columns.Count; i++)
                WorkRow[i] = rw[i];

            ParamText.Descr.Text = rw[ReferFinder.DispField].ToString();
            ReferFinder.userContent.Content = El;
            ReferFinder.userMenu.Content = ParamText;

            //MessageBox.Show("Edit");
        }

        public virtual void Delete()
        {
            //MessageBox.Show("Delete");
            if (ReferFinder.MainGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберете запись для удаления", "Удаление записи");
                return;
            }
            DataRow rw = ((DataRowView)ReferFinder.MainGrid.SelectedItem).Row;
            string s = $"Удалить запись '{rw[ReferFinder.DispField]}'?";
            MessageBox.Show(s);
        }

        public virtual void Save()
        {

        }


        public void ButOK_Click(object sender, RoutedEventArgs e)
        {
            int i = (int)((Button)sender).Tag;
            JoinRow al = Editors[i].joinRow;
            Finder fc = (Finder)al.FindConrol;
            if (fc.MainGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберете запись", Editors[i].FieldCaption);
                return;
            }
            DataRow rw = ((DataRowView)fc.MainGrid.SelectedItem).Row;
            foreach (string s in al.fields.Keys)
            {
                WorkRow[al.fields[s]] = rw[s];
            }
            ReferFinder.userContent.Content = El;
            ReferFinder.userMenu.Content = ParamText;
        }

        public void ButCancel_Click(object sender, RoutedEventArgs e)
        {
            ReferFinder.userContent.Content = El;
            ReferFinder.userMenu.Content = ParamText;
        }

        public void ButFind_Cklick(object sender, RoutedEventArgs e)
        {
            int i = (int)((Button)sender).Tag;
            ReferFinder.userContent.Content = Editors[i].joinRow.FindConrol.userContent;
            ReferFinder.userMenu.Content = Editors[i].joinRow.FindConrol.userMenu;
        }

    public override void start(object o)
        {
            ReferFinder = (Finder)o;
            ParamTab = ReferFinder.data.Clone();
            WorkRow = ParamTab.NewRow();
            ParamTab.Rows.Add(WorkRow);
            Editors = new List<EditField>();
            int minwidth = 500;

            string sql = $"select * from t_sysFieldMap where decname = '{ReferFinder.DecName}'";
            DataTable sysFieldMap = MainObj.Dbutil.Runsql(sql);


            for (int i = 0; i < ReferFinder.Fcols.Count; i++)
            {
                Control fe = null;
                JoinRow jr = new JoinRow();
                Finder fc = null;
                string GroupDec = "";
                DataRow[] a = sysFieldMap.Select($"dstfield = '{ReferFinder.Fcols[i].FieldName}' and isnull(classname, '') <> ''");

                if (a.Length > 0)
                {
                    string ClassName = a[0]["classname"].ToString();
                    if (ClassName == "Bureau.Finder" || ClassName == "Bureau.GridCombo")
                    {
                        jr.classname = ClassName;
                        jr.IdDeclare = a[0]["iddeclare"].ToString();
                        GroupDec = a[0]["groupdec"].ToString();
                        DataRow[] b = sysFieldMap.Select($"groupdec = '{GroupDec}'");
                        jr.fields = new Dictionary<string, string>();
                        foreach (var rw in b)
                        {
                            jr.fields.Add(rw["srcfield"].ToString(), rw["dstfield"].ToString());
                        }

                        fc = new Finder();
                        fc.OKFun = true;
                        fc.start(jr.IdDeclare);
                        fc.MenuControl.ButOK.Tag = i;
                        fc.MenuControl.ButOK.Click += ButOK_Click;
                        fc.MainGrid.MouseDoubleClick += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
                        {
                            ButOK_Click(fc.MenuControl.ButOK,  null);
                        };


                        fc.MenuControl.ButCancel.Click += ButCancel_Click;
                        jr.FindConrol = fc;
                    }

                    if (ClassName == "Bureau.Finder")
                    {
                        fe = new FinderEditor()
                        {
                            MinWidth = minwidth
                        };

                        ((FinderEditor)fe).ButFind.Tag = i;
                        ((FinderEditor)fe).ButFind.Click += ButFind_Cklick;

                        
                    }

                    if (ClassName == "Bureau.GridCombo")
                    {
                        fe = new ComboBox()
                        {
                            MinWidth = minwidth,
                            Foreground = System.Windows.Media.Brushes.White
                        };

                        ComboBox cb = (ComboBox)fe;
                        cb.ItemsSource = fc.MainView;
                        cb.DisplayMemberPath = a[0]["srcfield"].ToString();
                        DataRow[] c = sysFieldMap.Select($"groupdec = '{GroupDec}' and keyfield = 1");
                        cb.SelectedValuePath = c[0]["srcfield"].ToString();

                        Binding bd = new Binding(c[0]["dstfield"].ToString());
                        bd.Source = ParamTab;
                        cb.SetBinding(ComboBox.SelectedValueProperty, bd);

                    }
                }
                if (fe == null)
                    fe = new TextBox()
                        {
                            MinWidth = minwidth,
                            VerticalAlignment = VerticalAlignment.Center,
                            Foreground = System.Windows.Media.Brushes.White
                        };



                
                    Editors.Add(
                        new EditField()
                        {
                            FieldCaption = ReferFinder.Fcols[i].FieldCaption,
                            FieldName = ReferFinder.Fcols[i].FieldName,
                            DisplayFormat = ReferFinder.Fcols[i].DisplayFormat,
                            FieldEditor = fe,
                            joinRow = jr
                        }
                        );
                

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

                    /*
                    if (e.FieldEditor.GetType() == typeof(ComboBox))
                        ((ComboBox)e.FieldEditor).SetBinding(ComboBox.TextProperty, bd);
                    */

                }


                El = new EditorList()
                {
                    DataContext = this
                };
                ParamText = new ParamMenu();
                ParamText.Descr.Text = "Редактор";
                ParamText.ButOK.Click += (object sender, RoutedEventArgs e) =>
                {


                    Save();
                    ReferFinder.userContent.Content = ReferFinder.MainGrid;
                    ReferFinder.userMenu.Content = ReferFinder.MenuControl;

                };

                ParamText.ButCancel.Click += (object sender, RoutedEventArgs e) =>
                {

                    ReferFinder.userContent.Content = ReferFinder.MainGrid;
                    ReferFinder.userMenu.Content = ReferFinder.MenuControl;
                };


                //base.start(o);
            }

        }
    }
}

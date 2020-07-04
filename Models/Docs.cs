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

namespace WpfBu.Models
{
    class Docs : Finder
    {
        public override void AddInit(FinderMenu fm, Finder form)
        {
            Button bt = new Button
            {
                Style = (Style)form.Parent.Resources["MaterialDesignFloatingActionMiniAccentButton"],
                HorizontalAlignment = HorizontalAlignment.Left,
                ToolTip = "Файлы",
                Margin = new Thickness() { Right = 8 },
                Content = new PackIcon()
                {
                    Kind = PackIconKind.Files,
                    Height = 24,
                    Width = 24,
                    Foreground = Brushes.MediumBlue
                },

            };

            bt.Click += (object sender, RoutedEventArgs e) =>
            {



                if (MainGrid.SelectedItem == null)
                {
                    MessageBox.Show("Выберете запись", "Детали");
                    return;
                }
                DataRow rw = ((DataRowView)MainGrid.SelectedItem).Row;


                FilesDoc res;
                string idchiled = "Файлы" + "_" + rw[KeyF].ToString();

                if (Parent.formList.ContainsKey(idchiled))
                {
                    res = (FilesDoc)Parent.formList[idchiled];
                }
                else
                {
                    res = new FilesDoc
                    {
                        id = idchiled,
                        Parent = this.Parent
                    };
                    
                    res.start(rw[KeyF].ToString());
                    Parent.formList.Add(idchiled, res);
                    Parent.WinListSource.Add(res);
                }

                Parent.userMenu.Content = res.userMenu;
                Parent.userContent.Content = res.userContent;
                Parent.CurrentId = idchiled;

            };


            fm.ButPanel.Children.Add(bt);
        }
    }
}

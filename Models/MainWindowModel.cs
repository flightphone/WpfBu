﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Animation;
using MaterialDesignThemes;
using MaterialDesignThemes.Wpf;

namespace WpfBu.Models
{
    public class treeItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public ObservableCollection<treeItem> children { get; set; }

        public string iconCls { get; set; }

        public PackIconKind Kind { get; set; }

        public Dictionary<string, string> attributes { get; set; }

        public string state { get; set; }

        public treeItem(string t)
        {
            text = t;
        }

        public override string ToString()
        {
            return text;
        }
    }

    
    public class MainWindowModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<treeItem> MovieCategories { get; set; }

        private DataTable menuTab { get; set; }

        public MainWindowModel()
        {
            
            MainObj.IsPostgres = true;
            string sql;
            if (MainObj.IsPostgres)
            {
                MainObj.ConnectionString = @"Host=localhost;Username=postgres;Password=aA12345678;Database=uflights";
                MainObj.Account = "Admin";
                sql = "select a.*, '' web  from fn_mainmenu('ALL', @Account) a order by a.ordmenu, idmenu";
            }
            else
            {
                MainObj.ConnectionString = @"data source=localhost\SQLEXPRESS8;User ID=sa;Password=aA12345678;database=uFlights";
                MainObj.Account = "malkin";
                sql = "select a.* from fn_mainmenuweb('ALL', @Account) a order by a.ordmenu, idmenu";
            }
            MainObj.Dbutil = new DBUtil();
            Dictionary<string, object> par = new Dictionary<string, object> { {"@Account", MainObj.Account } };
            menuTab = MainObj.Dbutil.Runsql(sql, par);
            FilterItems("");
        }
        private void FilterItems(string keyword)
        {
            var rootItem = new treeItem("root");
            rootItem.children = new ObservableCollection<treeItem>();
            DataRow[] rw = menuTab.Select("caption like '%" + keyword + "%'", "ordmenu, idmenu");
            CreateItems("Root/", rootItem, rw);
            MovieCategories = rootItem.children;
        }

        public void CreateItems(string Root, treeItem Mn, DataRow[] Tab)
        {

            var ListCaption = new List<string>();
            var k = Root.Split('/', StringSplitOptions.RemoveEmptyEntries).Length;
            for (var x = 0; x < Tab.Length; x++)
            {
                var mi = Tab[x];
                var Caption = mi["caption"].ToString();
                var ItemIcon = mi["web"].ToString();
                if (Caption.IndexOf(Root) == 0)
                {
                    var bi = Caption.Split('/');
                    var ItemCaption = bi[k];
                    if (ListCaption.IndexOf(ItemCaption) == -1)
                    {
                        ListCaption.Add(ItemCaption);
                        treeItem ilist = new treeItem(ItemCaption);
                        ilist.id = (k == bi.Length - 1) ? mi["idmenu"].ToString() : mi["idmenu"].ToString() + "_node";
                        ilist.attributes = new Dictionary<string, string>() { { "link1", mi["link1"].ToString() }, { "params", mi["params"].ToString() } };

                        /*
                        if (string.IsNullOrEmpty(ItemIcon))
                            ilist.Kind = PackIconKind.Paper;
                        else
                            ilist.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), ItemIcon);
                        */
                        ilist.Kind = PackIconKind.Paper;
                        if (Mn.children == null)
                        { Mn.children = new ObservableCollection<treeItem>(); }
                        Mn.children.Add(ilist);
                        Mn.state = "closed";

                        if (Mn.Kind == PackIconKind.Paper)
                            Mn.Kind = PackIconKind.Folder;
                        /*
                        if (string.IsNullOrEmpty(ItemIcon))
                        {
                            if (Mn.Kind == PackIconKind.Paper)
                                Mn.Kind = PackIconKind.Folder;
                        }
                        else
                        {
                            if (Mn.Kind == PackIconKind.Paper || Mn.Kind == PackIconKind.Folder)
                                Mn.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), ItemIcon);
                        }
                        */



                        if (k != bi.Length - 1)
                        {
                            CreateItems(Root + ItemCaption + "/", ilist, Tab);
                        }
                    }
                }
            }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                FilterItems(_searchKeyword);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MovieCategories)));
            }
        }

        

    }
}


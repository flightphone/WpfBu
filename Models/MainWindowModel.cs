using System;
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

namespace WpfBu.Models
{
    public class treeItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public ObservableCollection<treeItem> children { get; set; }

        public string iconCls { get; set; }

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

    public class MainObj {
        public static string ConnectionString = @"data source=localhost\SQLEXPRESS8;User ID=sa;Password=aA12345678;database=uFlights";
        public static string Account="malkin";
    }
    public class MainWindowModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<treeItem> MovieCategories { get; set; }

        private DataTable menuTab { get; set; }

        private void createImage()
        {
            if (!Directory.Exists("images"))
                Directory.CreateDirectory("images");

            var sql = "select idimage, image_bmp from t_sysmenuimage";
            var data = new DataTable();
            var da = new SqlDataAdapter(sql, MainObj.ConnectionString);
            da.Fill(data);
            for (int i = 0; i < data.Rows.Count; i++)
            {
                string fname = "images/i" + data.Rows[i]["idimage"].ToString() + ".png";
                string s = data.Rows[i]["image_bmp"].ToString();
                byte[] buf = Convert.FromBase64String(s);
                MemoryStream mstr = new MemoryStream(buf);
                Bitmap bmp = new Bitmap(mstr);
                bmp.Save(fname, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public MainWindowModel()
        {
            //createImage();

            menuTab = new DataTable();
            string sql = "select a.* , dbo.fn_getmenuimageid(a.caption) idimage from fn_mainmenu('ALL', @Account) a order by a.ordmenu, idmenu";
            var da = new SqlDataAdapter(sql, MainObj.ConnectionString);
            da.SelectCommand.Parameters.AddWithValue("@Account", MainObj.Account);
            da.Fill(menuTab);
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
                        if ((int)mi["idimage"] > 0)
                            ilist.iconCls = "images/i" + mi["idimage"].ToString() + ".png";
                        else
                            ilist.iconCls = "images/i0.png";


                        if (Mn.children == null)
                        { Mn.children = new ObservableCollection<treeItem>(); }
                        Mn.children.Add(ilist);
                        Mn.state = "closed";
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


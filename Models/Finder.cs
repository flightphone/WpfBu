using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Data;
using System.Windows.Media;


namespace WpfBu.Models
{
    public class FinderField
    {
        public string FieldName { get; set; }
        public string FieldCaption { get; set; }
        public int Width { get; set; }
        public bool Visible { get; set; }
        public string DisplayFormat { get; set; }
        public string FindString { get; set; }
        public string Sort { get; set; }
    }

    public class Finder : RootForm
    {
        public DataGrid MainGrid;

        public string SQLText { get; set; }
        public string DecName { get; set; }
        public string Descr { get; set; }

        public string EditProc { get; set; }
        public string DelProc { get; set; }

        public string SumFields { get; set; }
        public int FROZENCOLS { get; set; }

        public List<FinderField> Fcols { get; set; }


        public override void start(object o)
        {
            try
            {
                string sql;
                if (MainObj.IsPostgres)
                    sql = "select iddeclare, decname, descr, dectype, decsql, keyfield, dispfield, keyvalue, dispvalue, keyparamname, dispparamname, isbasename, descript, addkeys, tablename, editproc, delproc, image_bmp, savefieldlist, p.paramvalue from t_rpdeclare d left join t_sysparams p on 'GridFind' || d.decname = p.paramname where iddeclare = ";
                else
                    sql = "select iddeclare, decname, descr, dectype, decsql, keyfield, dispfield, keyvalue, dispvalue, keyparamname, dispparamname, isbasename, descript, addkeys, tablename, editproc, delproc, image_bmp, savefieldlist, p.paramvalue from t_rpdeclare d left join t_sysparams p on 'GridFind' + d.decname = p.paramname where iddeclare = ";
                sql = sql + o.ToString();

                DataTable t_rp = MainObj.Dbutil.Runsql(sql);
                DataRow rd = t_rp.Rows[0];
                string paramvalue = rd["paramvalue"].ToString();
                SQLText = rd["decsql"].ToString();
                DecName = rd["decname"].ToString();
                Descr = rd["descr"].ToString();
                text = Descr;

                EditProc = rd["editproc"].ToString();
                DelProc = rd["delproc"].ToString();


                CreateColumns(paramvalue);
                UpdateTab();

                userMenu = new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 22,
                    Text = Descr
                };
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void CreateColumns(string s)
        {
            Fcols = new List<FinderField>();
            XmlDocument xm = new XmlDocument();
            XmlElement xRoot, xCol;
            xm.LoadXml(s);
            xRoot = xm.DocumentElement;

            SumFields = xRoot.GetAttribute("SumFields");
            int frcols = 0;
            int.TryParse(xRoot.GetAttribute("FROZENCOLS"), out frcols);
            FROZENCOLS = frcols;
            int n = xRoot.ChildNodes.Count;

            for (int i = 0; i < n; i++)
            {

                xCol = (XmlElement)xRoot.ChildNodes.Item(i);
                if (xCol.Name == "COLUMN")
                {
                    string FName = xCol.Attributes["FieldName"].Value;
                    if (MainObj.IsPostgres)
                        FName = FName.ToLower();
                    string Title = xCol.Attributes["FieldCaption"].Value;
                    int Width = 0;
                    int.TryParse(xCol.Attributes["Width"].Value, out Width);
                    bool Vis = (xCol.Attributes["Visible"].Value == "1");
                    string DispFormat = xCol.Attributes["DisplayFormat"].Value;
                    if (Vis)
                    {
                        Fcols.Add(new FinderField()
                        {
                            FieldName = FName,
                            FieldCaption = Title,
                            Width = Width,
                            DisplayFormat = DispFormat,
                            Visible = Vis,
                            Sort = "Нет"
                        });
                    }
                }
            }

            MainGrid = new DataGrid()
            {
                IsReadOnly = true,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                CanUserSortColumns = false,
                AutoGenerateColumns = false
            };

            MainGrid.LoadingRow += MainGrid_LoadingRow;

            foreach (FinderField f in Fcols)
            {
                if (f.Visible)
                {
                    Binding bn = new Binding(f.FieldName);
                    bn.StringFormat = f.DisplayFormat;
                    MainGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = f.FieldCaption,
                        Binding = bn,
                        MaxWidth = 500
                    });
                }
            }
            this.userContent = MainGrid;
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //throw new NotImplementedException();
            DataRowView item = e.Row.Item as DataRowView;
            if (item != null)
            {
                DataRow row = item.Row;
                int c = 0;
                if (row.Table.Columns.Contains("Color"))
                    if (row["Color"] != DBNull.Value)
                        c = (int)row["Color"];
                
                if (row.Table.Columns.Contains("color"))
                    if (row["color"] != DBNull.Value)
                        c = (int)row["color"];
                if (c != 0)
                {
                    System.Drawing.Color dc = System.Drawing.Color.FromArgb(c);
                    e.Row.Background = new SolidColorBrush(Color.FromRgb(dc.R, dc.G, dc.B));
                    //e.Row.Foreground = new SolidColorBrush(Color.FromRgb((byte)0, (byte)0, (byte)0));
                }    
            }
        }

        public virtual void UpdateTab()
        {
            string PrepareSQL = SQLText;
            PrepareSQL = PrepareSQL.Replace("[Account]", MainObj.Account);
            DataTable data = MainObj.Dbutil.Runsql(PrepareSQL);
            MainGrid.ItemsSource = data.DefaultView;
        }
    }
}
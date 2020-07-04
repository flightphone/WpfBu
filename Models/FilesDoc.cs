using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Text;
//using CefSharp.Wpf;

namespace WpfBu.Models
{
    class FilesDoc : RootForm
    {
        //public ChromiumWebBrowser Web { get; set; }
        public Frame Web { get; set; }
        public override void start(object o)
        {
            userContent = new ContentControl();
            userMenu = new ContentControl();
            //Web = new ChromiumWebBrowser("http://127.0.0.1:5000/Docfiles/dir?id=" + o.ToString() + "/");
            Web = new Frame() { 
                Source = new Uri("http://127.0.0.1:5000/Docfiles/dir?id=" + o.ToString() + "/")
            };
            userContent.Content = Web;
            text = "Файлы " + o.ToString();
            //id = "Файлы " + o.ToString();
            userMenu.Content = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 22,
                Text = "Файлы " + o.ToString()
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using System.Xml;

namespace XmlStream
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      String urlRead = "https://support.nexiogroup.com/SF/example.xml";
      String urlWrite = "ftp://ftp.cluster006.hosting.ovh.net/SF/example.xml";
      int num = ReadXml(urlRead);
      Console.WriteLine(DateTime.Now);
      WriteXml(urlWrite, "<answer>5-coucou</answer>");
      int nextNum = num;
      while (nextNum == num)
      {
        Thread.Sleep(2000);
        nextNum = ReadXml(urlRead);
      }
      Console.WriteLine(DateTime.Now);
    }

    private int ReadXml(String URLString)
    {
      int index = 0;
      XmlTextReader reader = new XmlTextReader(URLString);
      while (reader.Read())
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.Element: // The node is an element.
            break;
          case XmlNodeType.Text: //Display the text in each element.
            Console.WriteLine(reader.Value);
            index = Convert.ToInt32(reader.Value.Substring(0, reader.Value.IndexOf('-')));
            break;
          case XmlNodeType.EndElement: //Display the end of the element.
            break;
        }
      }

      return index;
    }

    private static void WriteXml(String URLString, String data)
    {
      FtpWebRequest request = (FtpWebRequest)WebRequest.Create(URLString);
      request.Method = WebRequestMethods.Ftp.AppendFile;
      request.ContentLength = data.Length;
      request.Credentials = new NetworkCredential("nexiogro-support", "A8ZH5PvC9w");
      Stream requestStream = request.GetRequestStream();
      StreamWriter sw = new StreamWriter(requestStream);
      sw.WriteLine(data);
      sw.Close();
      requestStream.Close();
      FtpWebResponse response = (FtpWebResponse)request.GetResponse();
      response.Close();
    }
  }
}

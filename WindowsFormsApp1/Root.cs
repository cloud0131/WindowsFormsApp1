using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using static WindowsFormsApp1.Form1;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    /// <summary>
    /// デシリアライズ用クラス。一文字でも変えると動かなくなる。触るな。
    /// </summary>
    [XmlRoot("root")]
    public class Root
    {
        //フィールド
        [XmlElement("result_code")]
        public string result_code { get; set; }

        [XmlArray("return_data")]
        [XmlArrayItem("user")]
        public List<User> users { get; set; } = new List<User>();


        // コンストラクタなし。
        // メソッドなし。

    }
}

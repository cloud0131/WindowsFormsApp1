using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static WindowsFormsApp1.Form1;
using static WindowsFormsApp1.Http;
using static WindowsFormsApp1.Root;
using static WindowsFormsApp1.User;
using static WindowsFormsApp1.SubUser;


namespace WindowsFormsApp1
{
    public static class Http
    {
        // フィールド　コンストラクタなし


        // メソッド
        //ストリーム　
        /// <summary>
        /// ※Stream.close()がないので、取得後はcloseしてください。
        /// </summary>
        /// <param name="select"></param>
        /// <param name="URL"></param>
        /// <returns>StreamReader型</returns>
        public static StreamReader Request_response(string select, string URL)
        {
            byte[] DataBytes = Encoding.UTF8.GetBytes(select);
            WebRequest req = WebRequest.Create(URL);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = DataBytes.Length;
            Stream requStream = req.GetRequestStream();
            requStream.Write(DataBytes, 0, DataBytes.Length);
            requStream.Close();

            WebResponse response = req.GetResponse();
            Stream resSt = response.GetResponseStream();
            StreamReader sre = new StreamReader(resSt, Encoding.GetEncoding("shift_jis"));
            return sre;
        }

        //　ストリームからデータを取り出す：1.Rootオブジェクトを受け取る。
        public static Root Request_Root(string select, string URL)
        {
            // Root root = new Root();
            try
            {
                using (StreamReader sr = Request_response(select, URL))
                {

                    XmlSerializer se = new XmlSerializer(typeof(Root));
                    Root root = (Root)se.Deserialize(sr);

                    if (root.result_code == "0000")
                    {
                        foreach (var user in root.users)
                        {
                            User data = new User();
                            data.family_name = user.family_name;//姓
                            data.name = user.name;//名前

                            if (user.sex == "1")//性別
                            { data.sex = "1"; }
                            else
                            { data.sex = "2"; }

                            data.age = user.age;//歳
                            data.birthday = user.birthday;//生年月日

                            data.address = user.address;//住所
                            data.note = user.note;//備考

                            root.users.Add(data);
                        }
                        return root;
                    }
                    else { return root; }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }//Respons_Root終了



        //　ストリームからデータを取り出す：2.DataSetを受け取る。
        public static Dictionary<String,DataTable> Request_dataset(string select, string URL)
        {
            try
            {

                using (StreamReader sr = Request_response(select, URL))
                {

                    DataSet dtSet = new DataSet();

                    dtSet.ReadXml(sr);

                    Dictionary<String, DataTable> tables = new Dictionary<string, DataTable> { { "root", dtSet.Tables["root"] } };
                    //リストの0番目（中身はテーブル）.行の0番目"result_code"]
                    //(String)tables[0].Rows[0]["result_code"] == "0000"
                    if ("0000".Equals((String)tables["root"].Rows[0]["result_code"]))
                    { 
                        tables.Add("user", dtSet.Tables["user"]);
                        return tables;
                    }
                    return tables;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        enum Resultcode
        {
            Comp = 0000,
            Notfound = 0101,
            Duplication = 0102,
            Parameter = 0103

        }

        public static String ResultPopup(String code)
        {
            switch (code)
            {
                case "0000":
                    return "成功しました。";
                case "0101":
                    return "データがありません。";
                case "0102":
                    return "IDが重複しています。IDを変えてください";
                case "0103":
                    return "入力したデータを\nもう一度確認してください。";
                default:
                    return "なぜエラーです";

            }

        }
    }
}

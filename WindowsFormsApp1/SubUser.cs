using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using static WindowsFormsApp1.User;
using System.Windows;

namespace WindowsFormsApp1
{
    public class SubUser : User
    {

        public Dictionary<String, String> Error { get; set; } = new Dictionary<string, string>();

        public Dictionary<String, String> Userdata { get; } = new Dictionary<string, string>();

        /// <summary>
        /// SubUserクラスのフィールドにあるUserdataDictionaryに、Userクラスのフィールドをセットする。戻り値なし。
        /// </summary>
        public void UserdataSetter()
        {
            var UserClassInfo = this.GetType().GetFields();

            foreach (var Info in UserClassInfo)
            {
                Userdata.Add(Info.Name, (String)Info.GetValue(this));
            }
        }
        /// <summary>
        /// SubUser.DataCheckでエラーチェックが終わった後の検索の為の文字列が保持されます。
        /// </summary>
        public String Kensaku { get; set; }


        /// <summary>
        /// Userクラスのフィールドの値を参照し、nullと空白チェックを行います。
        /// </summary>
        /// <returns>bool：全てNULLならtrue、一つでも値が入っていればfalse</returns>
        public bool AddNullCheck()
        {
            if (this.AllSpeaceCheck())
            {
                return true;
            }
            else if(string.IsNullOrWhiteSpace(this.birthday) &&
                    string.IsNullOrWhiteSpace(this.address))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 全ての値をIsNullOrWhiteSpaceにかけて、入力されていればtrue、入力されていない項目はfalse
        /// </summary>
        /// <returns>falseの場合、入力されていない項目はSubUser.ErrorDicsinaryに入れ、Error.Countが0なら戻り値はfalse</returns>
        public bool ErrorCheck()
        {
            foreach (KeyValuePair<String, String> pair in Userdata)
            {
                if (string.IsNullOrWhiteSpace(pair.Value) && pair.Key != "note")
                {
                    Error.Add(pair.Key, pair.Key + "が入力されていません。");
                }
            }
            if (Error.Count != 0)
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        /// ErrorDictionaryを参照し、どの項目にデータが入力されていないかを文字列にして返す。
        /// </summary>
        /// <returns>例：name \n address \n にデータが入力されていません。</returns>
        public String ErrorStr()
        {
            string str = "";
            foreach (KeyValuePair<String, String> pair in Error)
            {
                if (pair.Key == "note") { break; }
                str += pair.Key + "\n";
            }
            str += "にデータが入力されていません";

            return str;
        }
        /// <summary>
        /// SubUser.Userdataを基に入力チェックを行います。１部でもエラーがあればtrue
        /// </summary>
        /// <returns>エラーがなければfalseし、検索のための文字列をSubUser.kensakuに保持します</returns>
        public bool DataCheck()
        {

            // ID
            if (!Regex.IsMatch(this.id, "^[0-9a-zA-Z]{5}"))
            { Error.Add("id", "半角英数字5桁で入れてください"); }

            // 名前
            if (this.name.Length > 21)
            { Error.Add("name", "20文字以内で入れてください"); }

            // 姓
            if (this.family_name.Length > 21)
            { Error.Add("family_name", "20文字以内で入れてください"); }

            // 性別
            // チェックなし

            // 年齢
            if (!Regex.IsMatch(this.age, "[0-9]{1,3}"))
            { Error.Add("age", "半角英数字5桁で入れてください"); }

            // 生年月日
            if (!Regex.IsMatch(this.birthday, "[0-9]{8}"))
            { Error.Add("birthday", "半角数字8桁で入れてください"); }
            else if (this.birthday.Substring(4, 2) == "00" || this.birthday.Substring(6, 2) == "00")
            { Error.Add("birthday", "半角数字8桁で入れてください"); }

            // 住所
            if (this.address.Length > 101)
            { Error.Add("address", "100文字以内で入れてください"); }


            if (Error.Count != 0)
            {
                return true;
            }
            else
            {
                this.Kensaku = String.Join("&", Userdata.Select(item => item.Key + "=" + item.Value));
                return false;
            }
        }


        /// <summary>
        /// ErrorDicsionaryに入った項目を改行有りの文字列にして返します。
        /// </summary>
        /// <returns>例：名前は20字以内で入れてください"\n"...;</returns>
        public String ErrorStr2()
        {
            string str = "";
            foreach (KeyValuePair<String, String> pair in Error)
            {
                str += pair.Key + " は " + pair.Value + "\n";
            }
            return str;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
namespace WindowsFormsApp1
{
    [Serializable]
    public class User
    {
        //フィールド
        [System.Xml.Serialization.XmlElement("id")]
        public string id;
        public string ID
        {
            get { return id; }
            set { id = Setter(value); }
        }


        [System.Xml.Serialization.XmlElement("family_name")]
        public string family_name;
        public string FAMILY_NAME
        {
            get { return family_name; }
            set { family_name = Setter(value); }
        }

        [System.Xml.Serialization.XmlElement("name")]
        public string name;
        public string NAME
        {
            get { return name; }
            set { name = Setter(value); }
        }


        [System.Xml.Serialization.XmlElement("sex")]
        public string sex;
        public string SEX
        {
            get { return sex; }
            set { sex = value; }
        }

        [System.Xml.Serialization.XmlElement("age")]
        public string age;
        public string AGE
        {
            get { return age; }
            set
            {
                string w = "";
                w = Setter(value);
                age = w.TrimStart(new Char[] { '0' });
            }
        }

        [System.Xml.Serialization.XmlElement("birthday")]
        public string birthday;
        public string BIRTHDAY
        {
            get
            {
                if (string.IsNullOrEmpty(birthday))
                {
                    return birthday;
                }
                else
                {
                    return birthday.Replace("/", "").Trim();
                }
            }
            set
            {
                this.birthday = value;
                //if (!string.IsNullOrWhiteSpace(this.birthday))
                //{
                //    this.birthday.Insert(6, "/");
                //    this.birthday.Insert(4, "/");
                //}

            }
        }


        [System.Xml.Serialization.XmlElement("address")]
        public string address;
        public string ADDRESS
        {
            get { return address.Replace("<", "＜").Replace(">", "＞").Trim(); }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    address = value.Replace("<", "＜").Replace(">", "＞").Trim();
                }
                else { address = value; }

            }
        }


        [System.Xml.Serialization.XmlElement("note")]
        public string note;
        public string NOTE
        {
            get { return note; }
            set { note = Setter(value); }
        }

        /// <summary>
        /// nullOrEmptyなら空文字、値があれば空白をReplaceします。
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public string Setter(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return "";
            }
            else
            {
                return val.Replace(" ", "").Replace("　", "").Replace("<","＜").Replace(">","＞").Trim();
            }

        }

        /// <summary>
        /// UserクラスのフィールドがすべてNULLか空文字か判定します。全てnullならtrue。
        /// </summary>
        /// <param name="Userdata"></param>
        /// <returns>1部でも入っていれば、Userdata連想配列に値を追加し、更にfalseを返します。</returns>
        public bool AllSpeaceCheck()
        {
            if (string.IsNullOrWhiteSpace(this.id) &&
                string.IsNullOrWhiteSpace(this.family_name) &&
                string.IsNullOrWhiteSpace(this.name) &&
                string.IsNullOrWhiteSpace(this.sex) &&
                string.IsNullOrWhiteSpace(this.age)
                 )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 入力を正規表現と文字数チェックし、エラーがあった場合は空文字にしてメッセージボックスを出す。
        /// </summary>
        /// <returns>戻り値：なし</returns>
        public void SelectErrorCheck()
        {
            Dictionary<String, String> Error = new Dictionary<string, string>();
            // ID
            if ( !string.IsNullOrWhiteSpace(this.id) && !Regex.IsMatch(this.id, "^[0-9a-zA-Z]{5}"))
            {
                Error.Add("id", "半角英数字5桁で入れてください");
                this.id = "";
            }

            // 名前
            if (this.name.Length > 21)
            {
                Error.Add("名前", "20文字以内で入れてください");
                this.name = "";
            }

            // 姓
            if (this.family_name.Length > 21)
            {
                Error.Add("姓", "20文字以内で入れてください");
                this.family_name = "";
            }

            // 年齢
            if (!string.IsNullOrWhiteSpace(this.age) && !Regex.IsMatch(this.age, "[0-9]{1,3}"))
            {
                Error.Add("年齢", "半角英数字5桁で入れてください");
                this.age = "";
            }

            string str = "";
            if (Error.Count != 0)
            {
                foreach (KeyValuePair<String, String> pair in Error)
                {
                    str += pair.Key + " は " + pair.Value + "\n";
                }
                MessageBox.Show(str,"エラー発生個所は除いて検索します。");
            }
        }


        /// <summary>
        /// NULLや空文字が入っている箇所は除いて検索に使う文字列を返します。
        /// </summary>
        /// <returns>検索に使う文字列（例：id=xx&name=oo）</returns>
        public string SelectStrCreate()
        {
            Dictionary<String, String> select = new Dictionary<string, string>();

            var UserClassInfo = this.GetType().GetFields();

            foreach (var Info in UserClassInfo)
            {
                if (!string.IsNullOrWhiteSpace((String)Info.GetValue(this)))
                {
                    select.Add(Info.Name, (String)Info.GetValue(this));
                }
            }

            String kensaku = "";
            int i = 0;
            foreach (KeyValuePair<string, string> key in select)
            {
                i++;
                switch (key.Key)
                {
                    case "id":

                        kensaku += "id=" + key.Value;
                        break;
                    case "family_name":
                        kensaku += "family_name=" + key.Value;
                        break;
                    case "name":
                        kensaku += "name=" + key.Value;
                        break;
                    case "sex":
                        kensaku += "sex=" + key.Value;
                        break;
                    case "age":
                        kensaku += "age=" + key.Value;
                        break;
                    case "birthday":
                        kensaku += "name=" + key.Value;
                        break;
                    case "address":
                        kensaku += "sex=" + key.Value;
                        break;
                    case "note":
                        kensaku += "age=" + key.Value;
                        break;

                }
                //　検索文字列の最後には"＆"は入れない IF文
                if (select.Count != i)
                {
                    kensaku += "&";
                }
            }
            return kensaku;
        }


        /* 　C#の場合コンストラクタを書かなくても
         * 　new User(){id=○○,name=xx}
         * 　　OR
         * 　user = new  User(){}
         * 　user.id = ○○
         * 　user.name = XX
         * 　
         * 　で値を渡せる。（プロパティのおかげ）
         */

        /// <summary>
        /// ラジオボタンからの1or2か、又はどちらでもない（Null）を判定
        /// </summary>
        /// <param name="man"></param>
        /// <param name="woman"></param>
        /// <returns>戻り値：String（"1"か"2"、もしくは null）</returns>
        public static string WhichSexOrNull(bool man, bool woman)
        {
            string sex = "";

            if (man) { sex = "1"; }
            else if (woman) { sex = "2"; }
            else { sex = null; }

            return sex;
        }


        /// <summary>
        /// Userクラスのフィールドに保持している" id "の値を参照して、nullチェックと正規表現チェックを行います。
        /// </summary>
        /// <returns>空白、正規表現に合わない場合はエラーのMessageBoxとfalse。正しい入力であればtrue。</returns>
        public bool IDErrorCheck()
        {
            if (string.IsNullOrWhiteSpace(this.id))
            {
                MessageBox.Show("idを入力してください");
                return false;
            }
            else if (!Regex.IsMatch(this.id, "^[0-9a-zA-Z]{5}"))
            {
                MessageBox.Show("半角数字5桁で入れてください");
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Resources;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form //partialはクラスを分割して書くことができる修飾詞
    {
        // フィールド：From1
        // コンストラクタ：From1
        public Form1()
        {
            InitializeComponent();
            // this.pictureBox1.Parent = tabPage1;
            DateTime datetime = DateTime.Today;
            toolStripStatusLabel2.Text = datetime.ToString("yyyy年MM月dd日ddd");
        }

        //メソッド：From1
        //ホーム（テスト用）
        private void button1_Click(object sender, EventArgs e)
        {
            // テスト実施画面　後で消すか、各ページに飛んだりデータをリセットする画面にする
            this.label54.Text = "!--- WelCome ---!\n" +
                $"今日は{toolStripStatusLabel2.Text}曜日です\n" +
                "上記のタブページから機能を利用してください。";
        }
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //　検索　取得メソッド
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void SelectButtonClick(object sender, EventArgs e)
        {
            // 後でAPIに検索をかける為の宣言
            string select = "";

            // 入力データをcheckするためインスタンス化する。 
            SubUser user = new SubUser();
            user.ID = SelectId.Text;
            user.FAMILY_NAME = SelectFamily.Text;
            user.NAME = SelectName.Text;
            // 性別はどちらか判定、またはNullを入れる。
            user.SEX = User.WhichSexOrNull(SelectMan.Checked, SelectWoman.Checked);
            user.AGE = SelectAge.Text;

            //すべて空文字か？一部だけ入力されているか？入力データされているデータは合っているかチェックする。
            if (!user.AllSpeaceCheck())
            {
                user.SelectErrorCheck();
                select = user.SelectStrCreate();
            }


            //スクロールバーに状況を表示する
            //コントロールを初期化する
            ProgressBar1.Minimum = 0;
            ProgressBar1.Maximum = 3;
            ProgressBar1.Value = 0;

            //時間のかかる処理を開始する
            for (int i = 1; i <= 3; i++)
            {
                //1秒間待機する（時間のかかる処理があるものとする）
                System.Threading.Thread.Sleep(1000);

                //ProgressBar1の値を変更する
                ProgressBar1.Value = i;

                //（フォーム全体を再描画するには、次のようにする）
            }

            try
            {
                Dictionary<String, DataTable> tables = Http.Request_dataset(select, ConfigurationManager.AppSettings["select"]);

                if ((String)tables["root"].Rows[0]["result_code"] == "0000")
                {
                    dataGridView1.DataSource = tables["user"];
                    tabControl1.SelectedTab = tabPage6;
                    this.selectResult.Text = "データの検索に成功しました。";
                    searchResult.Visible = true;
                }
                else if ((String)tables["root"].Rows[0]["result_code"] == "0101")
                {
                    this.selectResult.Text = "検索しましたが、\nデータはありませんでした。";
                }
                else
                { this.selectResult.Text = "データはありませんでした。"; }

            }
            catch (Exception) { MessageBox.Show("データの取得ができませんでした。\nサーバーを確認してください"); }

        }
        // 検索の性別をリセットする---------------------
        private void SexResetButtonClick(object sender, EventArgs e)
        {
            SelectId.Text = "";
            SelectFamily.Text = "";
            SelectName.Text = "";
            SelectAge.Text = "";
            SelectMan.Checked = false;
            SelectWoman.Checked = false;
        }

        // 検索結果タブに飛ぶボタン-----------------------
        private void searchResult_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage6;
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++       
        // "検索結果"画面のデータグリッドビューの表示を整える
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            //セルの列がSEXなら、男、女に変える。
            if (dgv.Columns[e.ColumnIndex].Name == "SEX")
            {
                if ("1".Equals((String)e.Value))
                {
                    e.Value = "男";
                }
                else if ("2".Equals((String)e.Value))
                {
                    e.Value = " 女";
                }
                else { e.Value = "その他"; }
            }

            //セルの列がBIRTHDAYなら、年月日にする
            if (dgv.Columns[e.ColumnIndex].Name == "BIRTHDAY")
            {
                String val = Convert.ToString(e.Value);

                if (Regex.IsMatch((String)e.Value, "[0-9]{8}"))
                {
                    String W = "";
                    W = val.Substring(0, 4) + "年";
                    W += val.Substring(4, 2) + "月";
                    W += val.Substring(val.Length - 2) + "日";

                    e.Value = W;
                }
            }

        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 追加のメソッド
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void AddButtonClick(object sender, EventArgs e)
        {
            Dictionary<String, String> err = new Dictionary<String, String>();

            // 入力データをcheckするためインスタンス化する。 
            SubUser user = new SubUser();

            user.ID = AddId.Text;
            user.FAMILY_NAME = AddFamily.Text;
            user.NAME = AddName.Text;
            user.SEX = User.WhichSexOrNull(AddMan.Checked, AddWoman.Checked);
            user.AGE = Convert.ToString(numAge.Value);
            user.BIRTHDAY = AddBirthday.Value.ToString("yyyyMMdd");
            user.ADDRESS = AddAddress.Text;
            user.NOTE = AddNote.Text;
            user.UserdataSetter();

            // case1.値が全部入っていない（DataTimePickerがあるので、trueには滅多にならない）
            if (user.AddNullCheck())
            {
                MessageBox.Show("すべての項目にデータが入っていません。" +
                    "\n項目にデータを入力してください");
            }
            // case2.値が1部入っている。→入っていない項目を指摘する
            // case3.値がすべて入っているが、正規表現とは違う
            else if (user.ErrorCheck())
            {
                MessageBox.Show(user.ErrorStr());
            }
            //すべての項目が入っているが、正規表現とは一致しない。
            else if (user.DataCheck())
            {
                MessageBox.Show(user.ErrorStr2());
            }
            //すべての項目が入っており、正規表現と一致する。
            else
            {
                addPanel.Visible = true;
                resultid.Text = user.id;
                resultfam.Text = user.family_name;
                resultname.Text = user.name;
                if (user.sex == "1") { resultsex.Text = "男性"; } else { resultsex.Text = "女性"; }
                resultage.Text = user.age;
                resultbirth.Text = user.birthday;
                resultaddress.Text = user.address;
                resultnote.Text = user.note;
                DialogResult result = MessageBox.Show("この内容で追加しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Streamに接続する
                    try
                    {
                        Dictionary<String, DataTable> tables = Http.Request_dataset(user.Kensaku, ConfigurationManager.AppSettings["insert"]);
                        String code = Http.ResultPopup((String)tables["root"].Rows[0]["result_code"]);
                        MessageBox.Show(code);

                    }
                    catch (NullReferenceException ex)
                    {
                        Console.WriteLine(ex.Message);
                        MessageBox.Show("エラーが発生しました。\n'<''>'（大なり記号、小なり記号）は使わないでください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex) 
                    {
                        MessageBox.Show("データの取得ができませんでした。\nサーバーを確認してください");
                        Console.WriteLine(ex.Message); 
                    }
                }
            }
            if (user.Error != null) { user.Error.Clear(); }
            if (user.Userdata != null) { user.Userdata.Clear(); }
        }

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 更新ページのメソッド
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /*　更新処理
         * 1.入力されたIDから検索して値を自動で入れる。なければエラーをポップする
         * 2.値を入力してもらう。""やnullならエラーを出す。
         */
        /*+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * IDを基に変更をかける
         *+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         */
        private void ChangeIDButtonClick(object sender, EventArgs e)
        {
            try
            {
                User ID = new User() { id = upDateID.Text };
                if (ID.IDErrorCheck())
                {
                    StreamReader sr = Http.Request_response("id=" + ID.id, ConfigurationManager.AppSettings["select"]);

                    XmlSerializer se = new XmlSerializer(typeof(Root));
                    Root root = (Root)se.Deserialize(sr);

                    if (root.result_code == "0000")
                    {
                        cangesPanel.Visible = true;
                        cangeResultPanel.Visible = true;
                        foreach (var user in root.users)
                        {
                            changeFamiryname.Text = user.family_name;
                            changeName.Text = user.name;

                            changeSexMan.Checked = (user.sex == "1");
                            changeSexWemen.Checked = (user.sex == "2");

                            changeAge.Text = user.age;

                            int yyyy;
                            int MM;
                            int dd;
                            if (!Regex.IsMatch((String)user.birthday, "[0-9]{8}"))
                            {
                                user.birthday = Regex.Replace(user.birthday, "[^0-9]", "");
                            }

                            yyyy = Convert.ToInt32(user.birthday.Substring(0, 4));
                            MM = Convert.ToInt32(user.birthday.Substring(4, 2));
                            dd = Convert.ToInt32(user.birthday.Substring(user.birthday.Length - 2, 2));
                            try
                            {
                                chengeDateTime.Value = new DateTime(yyyy, MM, dd);
                            }
                            catch (Exception) { MessageBox.Show("生年月日の値が取得できません。\n再度入力してください"); }

                            changeAddress.Text = user.address;
                            changeNote.Text = user.note;
                            upDateID.ReadOnly = true;
                            button2.Visible = false;
                            changCansel.Visible = true;

                        }
                        sr.Close();

                    }// result_codeが0000の場合のif終了
                    else if (root.result_code == "0101")
                    {
                        MessageBox.Show("データがありません。\nIDを確認してください。");
                        cangesPanel.Visible = false;
                        cangeResultPanel.Visible = false;
                    }

                }// if(IDErrorCheck())終了、エラーメッセージはIDErrorCheck内で出す。
            }
            catch (Exception) { MessageBox.Show("サーバーへの接続を確認してください。"); }
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. 変更するデータを受け取り、確認する。
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void ChangBotan_Click(object sender, EventArgs e)
        {
            SubUser user = new SubUser();

            user.ID = upDateID.Text;
            user.FAMILY_NAME = changeFamiryname.Text;
            user.NAME = changeName.Text;
            user.SEX = User.WhichSexOrNull(changeSexMan.Checked, changeSexWemen.Checked);
            user.AGE = Convert.ToString(changeAge.Value);
            user.BIRTHDAY = chengeDateTime.Value.ToString("yyyyMMdd");
            user.ADDRESS = changeAddress.Text;
            user.NOTE = changeNote.Text;
            // SubUserクラスのフィールドにあるUserdataDictionaryにフィールドの変数をセットする
            user.UserdataSetter();

            // case1.値が全部入っていない
            if (user.AddNullCheck())
            {
                MessageBox.Show("すべての項目にデータが入っていません。" +
                    "\n項目にデータを入力してください");
                return;
            }
            // case2.値が1部入っている。→入っていない項目を指摘する
            // case3.値がすべて入っているが、正規表現とは違う
            else if (user.ErrorCheck())
            {
                MessageBox.Show(user.ErrorStr());
                return;
            }
            //すべての項目が入っているが、正規表現とは一致しない。
            else if (user.DataCheck())
            {
                MessageBox.Show(user.ErrorStr2());
                return;
            }
            //すべての項目が入っており、正規表現と一致する。
            else
            {
                cangeResultPanel.Visible = true;
                cangeREPanel.Visible = true;
                cangeIDlabel.Text =  user.id;
                cangeFamlabel.Text = user.family_name;
                cangeNamelabel.Text = user.name;
                if (user.sex == "1") { cangeSexlabel.Text = "男"; }
                else { cangeSexlabel.Text = "女"; }
                cangeAgelabel.Text =user.age;

                cangeBirthresslabel.Text =
                   $"{user.birthday.Substring(0, 4)}年{user.birthday.Substring(4, 2)}月{user.birthday.Substring(6, 2)}日";
                cangeAddresslabel.Text =  user.address;
                cangeNotelabel.Text =  user.note;
                DialogResult result = MessageBox.Show("この内容で変更しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Streamに接続する
                    try
                    {
                        Dictionary<String, DataTable> tables = Http.Request_dataset(user.Kensaku, ConfigurationManager.AppSettings["update"]);
                        String code = Http.ResultPopup((String)tables["root"].Rows[0]["result_code"]);
                        MessageBox.Show(code);
                    }
                    catch (NullReferenceException ex)
                    {
                        Console.WriteLine(ex.Message);
                        MessageBox.Show("エラーが発生しました。\n'<'　'>'（大なり記号、小なり記号）は使わないでください。","エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        MessageBox.Show("エラーが発生しました。\nサーバの接続を確認してください。");
                    }
                }
            }
            if (user.Error != null) user.Error.Clear();
            if (user.Userdata != null) user.Userdata.Clear();
        }

        private void changCansel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("違うIDの情報を更新しますか？", "入力されているデータをリセットします", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                upDateID.ReadOnly = false;
                button2.Visible = true;
                changCansel.Visible = false;

                changeFamiryname.Text = "";
                changeName.Text = "";

                changeSexMan.Checked = true;
                changeSexWemen.Checked = false;

                changeAge.Text = "";

                chengeDateTime.Value = DateTime.Today;

                changeAddress.Text = "";
                changeNote.Text = "";

                cangeREPanel.Visible = false;
            }

        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //削除画面のメソッド
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void deleteButton_Click(object sender, EventArgs e)
        {
            deleteLabel.Text = "";
            string err = "";

            User ID = new User() { id = deleteId.Text };

            try {

                if (ID.IDErrorCheck())
                {
                    StreamReader sre = Http.Request_response("id=" + ID.id, ConfigurationManager.AppSettings["select"]);
                    XmlSerializer se = new XmlSerializer(typeof(Root));
                    Root root = (Root)se.Deserialize(sre);

                    if (root.result_code == "0000")
                    {
                        deletePanel.Visible = true;

                        foreach (var user in root.users)
                        {
                            deletefam.Text = user.family_name;
                            deletename.Text = user.name;

                            if (user.sex == "1")
                            {
                                deletesex.Text = "男性";
                            }
                            else
                            {
                                deletesex.Text = "女性";
                            }
                            deleteage.Text = user.age;

                            deletebirth.Text =
                                user.birthday.Substring(0, 4) + "年 " + user.birthday.Substring(4, 2) + "月 " + user.birthday.Substring(6, 2) + "日";

                            deleteaddress.Text = user.address;
                            deletenote.Text = user.note;
                        }
                        deleteId.Enabled = false;
                    }
                    else
                    {
                        err = "データがありません。IDが違うようです";
                    }
                    sre.Close();
                }
                deleteerror.Text = err;
            }
            catch (Exception) { MessageBox.Show("エラーが出ました。\nデータサーバーを確認してください。"); }
        }

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 削除画面で『はい』を押した場合
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(deleteId.Text + "\n本当に削除しますか？", "データ削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    string kensaku = "id=" + deleteId.Text;
                    Dictionary<String, DataTable> tables = Http.Request_dataset(kensaku, ConfigurationManager.AppSettings["delete"]);

                    string code = Http.ResultPopup((String)tables["root"].Rows[0]["result_code"]);
                    MessageBox.Show(code);
                    deleteLabel.Text = "データを1件削除しました。";
                    deleteId.Text = "";
                    deleteId.Enabled = true;
                    deletePanel.Visible = false;
                }
                else
                {
                    MessageBox.Show("入力しリセットします。再度IDを入力しなおしてください。");
                    deleteId.Text = "";
                    deleteId.Enabled = true;
                    deletePanel.Visible = false;
                }
            }
            catch (Exception) { MessageBox.Show("データの取得ができませんでした。\nサーバーを確認してください"); }
        }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 削除画面で『いいえ』を押した場合
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void NoDeleteLinkLabel(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("データをリセットします。");
            deleteId.Text = "";
            deleteId.Enabled = true;
            deletePanel.Visible = false;
        }

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 行のヘッダーをダブルクリックしたときに、編集か削除のメッセージボックスを出す。
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Int32 selectedCellCount = dataGridView1.GetCellCount(DataGridViewElementStates.Selected);
            String[] selectdata = new String[selectedCellCount];
            for (int i = 0; i < selectedCellCount; i++)
            {
                selectdata[i] = dataGridView1.SelectedCells[i].Value.ToString();
            }
            var result = MessageBox.Show($"ID：{selectdata[0]}を編集しますか？", "確認", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                tabControl1.SelectedTab = tabPage4;
                upDateID.Text = selectdata[0];
                changeFamiryname.Text = "";
                changeName.Text = "";
                changeSexMan.Checked = false;
                changeSexWemen.Checked = false;
                changeAge.Value = 0;
                chengeDateTime.Value = DateTime.Today;
                changeAddress.Text = "";
                changeNote.Text = "";
                return;
            }
            else
            {
                var result2 = MessageBox.Show($"ID：{ selectdata[0] }を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result2 == DialogResult.Yes)
                {
                    tabControl1.SelectedTab = tabPage5;
                    deleteId.Text = selectdata[0];
                    deleteId.Enabled = true;
                    deletePanel.Visible = false;
                    return;
                }
            }
        }

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ステータスバーに全検索するボタンを付けた。
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            //Button1のクリックイベントハンドラ
            //コントロールを初期化する
            ProgressBar1.Minimum = 0;
            ProgressBar1.Maximum = 5;
            ProgressBar1.Value = 0;

            //時間のかかる処理を開始する
            for (int i = 1; i <= 5; i++)
            {
                //1秒間待機する（時間のかかる処理があるものとする）
                System.Threading.Thread.Sleep(1000);

                //ProgressBar1の値を変更する
                ProgressBar1.Value = i;

                //（フォーム全体を再描画するには、次のようにする）
                //this.Update();
            }
            string select = "";
            try { 
            Dictionary<String, DataTable> tables = Http.Request_dataset(select, ConfigurationManager.AppSettings["select"]);

            if ((String)tables["root"].Rows[0]["result_code"] == "0000")
            {
                tabControl1.SelectedTab = tabPage6;
            }
            else { MessageBox.Show("検索できませんでした。"); }

            dataGridView1.DataSource = tables["user"];
            }catch (Exception) { MessageBox.Show("データの取得ができませんでした。\nサーバーを確認してください"); }

        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 
        // それぞれのヘルプページ
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 


        private void SelectHelpLinkClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpForm help = HelpForm.GetInstance();

                help.Visible=true;
                help.HelpTabPage(SelectHelpLabel.Tag);
            
        }
        private void addLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpForm help = HelpForm.GetInstance();

            help.Visible = true;
            help.HelpTabPage(addLinkLabel.Tag);
        }

        private void changeLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpForm help = HelpForm.GetInstance();

            help.Visible = true;
            help.HelpTabPage(changeLinkLabel.Tag);
        }

        private void deleteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpForm help = HelpForm.GetInstance();

            help.Visible = true;
            help.HelpTabPage(deleteLinkLabel.Tag);
        }
        private void SelectHelpLinkClick(object sender, EventArgs e)
        {
            HelpForm help = HelpForm.GetInstance();

            help.Visible = true;
            help.HelpTabPage(SelectHelpLabel.Tag);
        }


    }//Form1終了
}//namespease終了






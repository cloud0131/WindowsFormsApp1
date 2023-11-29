using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class HelpForm : Form
    {
        // フィールド
        private static HelpForm Help { get; set; } = new HelpForm();

        //コンストラクタ
        /// <summary>
        /// HelpFormクラスはコンストラクタがprivateであり、新しいインスタンスは作れない。
        /// </summary>
        /// <returns>" HelpForm. GetInstance() "メソッドで、HelpFormの持っているインスタンスをgetできる。</returns>
        private HelpForm() 
        {
            InitializeComponent();
            DateTime datetime = DateTime.Today;
            HelpStatusLabel1.Text = datetime.ToString("yyyy年MM月dd日ddd");
        }

        /// <summary>
        /// HelpFormクラスが自身のフィールドに保持しているインスタンスを渡します。
        /// </summary>
        /// <returns>HelpFromがフィールドに保持している、HelpFormインスタンスを返す。</returns>
        //　メソッド
        public static HelpForm GetInstance()
        {
            if(Help == null)
            {
                Help = new HelpForm();
            }
            return Help;
        }


 /*　記録用
        //if (this.select == null && this.add == null && this.change == null && this.delete == null)
        /// <summary>
        /// "HelpForm"ウィンドウが開いているか、booleanで返します
        /// </summary>
        /// <returns>開いているならtrue、開いていなければfalse</returns>

        public bool formOpencheck ()
        {
            for (int i = 0; i < System.Windows.Forms.Application.OpenForms.Count; i++)
            {
                Form f = Application.OpenForms[i];
                if ("HelpForm".Equals(f.Text))
                {
                    return true;
                }
            }
            return false;
        }
 */
        /// <summary>
        /// 引数に" Object tag "を要求し、開いているHelpFormのタブページを切り替えます。
        /// </summary>
        /// <param name="tag"></param>
        public void HelpTabPage(Object tag)
        {
            // windowsフォームの初期化
            String Tag = (string)tag;

            switch (Tag)
            {
                case "select":
                    tabControl1.SelectedTab = tabPage1;
                    break;

                case "add":
                    tabControl1.SelectedTab = tabPage2;
                    break;

                case "change":
                    tabControl1.SelectedTab = tabPage3;
                    break;

                case "delete":
                    tabControl1.SelectedTab = tabPage4;
                    break;

                default:
                    tabControl1.SelectedTab = tabPage1;
                    break;


            }

        }

        private void HelpForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;

        }
    }
}

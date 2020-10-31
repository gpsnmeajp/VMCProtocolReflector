using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace VMCProtocolReflectorGUI
{
    public partial class frmGUI : Form
    {
        const string title = "VMCProtocolReflector v0.01";
        const string filename = "setting.txt";

        const int SendObjLength = 10;

        private List<NumericUpDown> PortNumericUpDownList = new List<NumericUpDown>();
        private List<TextBox> IPAddressTextBox = new List<TextBox>();
        private List<Button> AddButton = new List<Button>();


        public frmGUI()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            Setup();
        }

        private void Setup()
        {
            for (int i = 1; i < SendObjLength + 1; i++)
            {
                Control[] numericUpDown = this.Controls.Find($"nudSendPort{i:00}", true);
                Control[] textbox = this.Controls.Find($"txtIPAddress{i:00}", true);
                Control[] button = this.Controls.Find($"btnAdd{i:00}", true);

                PortNumericUpDownList.Add((NumericUpDown)(numericUpDown[0]));
                IPAddressTextBox.Add(((TextBox)textbox[0]));
                AddButton.Add(((Button)button[0]));
            }

            Init();
            LoadSettings();
        }

        private void Init()
        {
            nudReceivePort.Value = 39539;
            for (int i = 0; i < SendObjLength; i++)
            {
                IPAddressTextBox[i].Text = "127.0.0.1";
                PortNumericUpDownList[i].Value = 39600 + i;
                PortNumericUpDownList[i].Visible = false;
                IPAddressTextBox[i].Visible = false;
                AddButton[i].Visible = true;
            }
        }

        private void LoadSettings()
        {
            string[] settings;
            try
            {
                settings = File.ReadAllLines(filename);
            }
            catch
            {
                MessageBox.Show("設定を読み込むことができませんでした。初期設定を表示します。", title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                nudReceivePort.Value = int.Parse(settings[0]);

                for (int i = 1; i < settings.Length; i++)
                {
                    string[] host = settings[i].Split(':');
                    if (host.Length == 2)
                    {
                        SetSendData(host[0], int.Parse(host[1]), i - 1);
                    }
                }
            }
            catch
            {
                MessageBox.Show("GUIでは読み込むことのできない設定です。初期設定を表示します。", title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void SetSendData(string IP, int port, int index)
        {
            IPAddressTextBox[index].Text = IP;
            PortNumericUpDownList[index].Value = port;
            PortNumericUpDownList[index].Visible = true;
            IPAddressTextBox[index].Visible = true;
            AddButton[index].Visible = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Name;
            int index = int.Parse(name.Substring(name.Length - 2, 2)) - 1;

            PortNumericUpDownList[index].Visible = true;
            IPAddressTextBox[index].Visible = true;
            AddButton[index].Visible = false;
        }

        private void nudSendPort_DoubleClick(object sender, EventArgs e)
        {
            string name;
            if (sender is TextBox)
            {
                name = ((TextBox)sender).Name;
            }
            else
            {
                name = ((NumericUpDown)sender).Name;
            }

            int index = int.Parse(name.Substring(name.Length - 2, 2)) - 1;

            PortNumericUpDownList[index].Visible = false;
            IPAddressTextBox[index].Visible = false;
            AddButton[index].Visible = true;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            if (SaveSettings())
            {
                MessageBox.Show("設定を保存しました。Reflectorの次回起動時から反映されます。", title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool SaveSettings()
        {
            var sendPorts = new List<string>();
            for (int i = 0; i < SendObjLength; i++)
            {
                var sendData = SendDataGet(i);
                if (sendData != null) sendPorts.Add(sendData);
            }

            //設定数チェック
            if (sendPorts.Count < 1)
            {
                MessageBox.Show("送信先が設定されていません。", title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //重複チェック
            for (int i = 0; i < sendPorts.Count - 1; i++)
            {
                for (int j = i + 1; j < sendPorts.Count; j++)
                {
                    if ((string)sendPorts[i] == (string)sendPorts[j])
                    {
                        MessageBox.Show("送信先が重複しています。", title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            string output = nudReceivePort.Value.ToString() + "\r\n";
            for (int i = 0; i < sendPorts.Count; i++)
            {
                output += sendPorts[i].ToString() + "\r\n";
            }
            File.WriteAllText("setting.txt", output);
            return true;
        }

        private string SendDataGet(int index)
        {
            if (AddButton[index].Visible || IPAddressTextBox[index].Text == string.Empty)
            {
                return null;
            }
            else
            {
                return IPAddressTextBox[index].Text + ":" + PortNumericUpDownList[index].Value;
            }
        }

        private void btnReStart_Click(object sender, EventArgs e)
        {
            if (!SaveSettings())
            {
                return;
            }

            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName("VMCProtocolReflector");
            foreach (System.Diagnostics.Process p in ps)
            {
                try
                {
                    //プロセスを強制的に終了させる
                    p.Kill();
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }


            Process.Start("VMCProtocolReflector.exe");

        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("設定を初期化します。よろしいですか？", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Init();
            }
        }
    }
}

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
        const string filename = "setting.txt";
        const int SendObjLength = 10;
        private List<NumericUpDown> PortNumericUpDownList = new List<NumericUpDown>();
        private List<TextBox> IPAddressTextBox = new List<TextBox>();
        private List<Button> AddButton = new List<Button>();


        public frmGUI()
        {
            InitializeComponent();
            Setup();
        }

        private void Setup()
        {
            for (int i = 1; i < SendObjLength+1; i++)
            {
                Control[] numericUpDown = this.Controls.Find($"nudSendPort{i:00}", true);
                Control[] textbox = this.Controls.Find($"txtIPAddress{i:00}", true);
                Control[] button = this.Controls.Find($"btnAdd{i:00}", true);
                PortNumericUpDownList.Add((NumericUpDown)(numericUpDown[0]));
                IPAddressTextBox.Add(((TextBox)textbox[0]));
                AddButton.Add(((Button)button[0]));
            }


            string[] settings = File.ReadAllLines(filename);

            nudReceivePort.Value = int.Parse(settings[0]);

            for (int i = 1; i < settings.Length; i++)
            {
                string[] host = settings[i].Split(':');
                if (host.Length == 2)
                {
                    SetSendData(host[0],int.Parse(host[1]),i-1);
                }
            }
        }

        private void SetSendData(string IP,int port,int index)
        {
            IPAddressTextBox[index].Text = IP;
            PortNumericUpDownList[index].Value = port;
            PortNumericUpDownList[index].Visible = true;
            IPAddressTextBox[index].Visible = true;
            AddButton[index].Visible = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = ((Button) sender).Name;
            int index = int.Parse(name.Substring(name.Length - 2, 2)) - 1;

            PortNumericUpDownList[index].Visible = true;
            IPAddressTextBox[index].Visible = true;
            AddButton[index].Visible = false;
        }

        private void nudSendPort_DoubleClick(object sender, EventArgs e)
        {
            string name;
            if (sender is TextBox)
                name = ((TextBox)sender).Name;
            else
                name = ((NumericUpDown)sender).Name;

            int index = int.Parse(name.Substring(name.Length - 2, 2)) - 1;

            PortNumericUpDownList[index].Visible = false;
            IPAddressTextBox[index].Visible = false;
            AddButton[index].Visible = true;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            var sendPorts = new List<string>();
            for (int i = 0; i < SendObjLength; i++)
            {
                var sendData = SendDataGet(i);
                if (sendData != null) sendPorts.Add(sendData);
            }


            if (sendPorts.Count < 2)
            {
                MessageBox.Show("送信先が2個以下しか設定されていません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                for (int i = 0; i < sendPorts.Count-1; i++)
                {
                    for (int j = i+1; j < sendPorts.Count; j++)
                    {
                        if ((string)sendPorts[i] == (string)sendPorts[j])
                        {
                            MessageBox.Show("送信先が重複しています。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                File.WriteAllText("setting.txt", nudReceivePort.Value.ToString() + "\r\n");

                for (int i = 0; i < sendPorts.Count; i++)
                    File.AppendAllText("setting.txt", sendPorts[i].ToString() + "\r\n");

                MessageBox.Show("設定完了。", "設定完了", MessageBoxButtons.OK);
            }

        }

        private string SendDataGet(int index)
        {
            if (AddButton[index].Visible || IPAddressTextBox[index].Text == string.Empty)
                return null;
            else
                return IPAddressTextBox[index].Text + ":" + PortNumericUpDownList[index].Value;
        }

        private void btnReStart_Click(object sender, EventArgs e)
        {
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

    }
}

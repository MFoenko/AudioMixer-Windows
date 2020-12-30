using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioMixer
{
    public partial class Form1 : Form
    {


        public delegate void DataDelegate(int control, float val);
        public DataDelegate myDelegate;

        public Form1()
        {
            InitializeComponent();
            // dump all audio devices
            foreach (AudioDevice device in AudioUtilities.GetAllDevices())
            {
                Console.WriteLine("dev "+device.FriendlyName);
                
            }

            // dump all audio sessions
            foreach (AudioSession session in AudioUtilities.GetAllSessions())
            {
                if (session.Process != null)
                {
                    // only the one associated with a defined process
                    Console.WriteLine("process "+session.Process.ProcessName);
                    comboBox1.Items.Add(session);
                }
            }



            //AudioUtilities.IAudioSessionManager2 manager = AudioUtilities.GetAudioSessionManager();
            //manager.RegisterSessionNotification(new SessionNotification());

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // serialPort1.WriteLine("a");
            //serialPort1.Encoding = new ASCIIEncoding();
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(mySerialPort_DataReceived);
            serialPort1.Open();

            myDelegate = new DataDelegate(OnData);
        }

        private void OnData(int control, float val)
        {
            trackBar1.Value = (int)(val * trackBar1.Maximum);
            AudioSession session = (AudioSession)comboBox1.SelectedItem;
            if(session != null) session.SetVolume(val);
        }

        private void mySerialPort_DataReceived(
                    object sender,
                    SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int control = sp.ReadByte();
            int val = sp.ReadByte();
            Console.WriteLine("control=" + control + " val=" + val);
            float v = val / 256f;
            trackBar1.Invoke(myDelegate, new object[] { control, v });

        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPort1.Close();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioSession session = (AudioSession)comboBox1.SelectedItem;
            if (session == null) return;
            trackBar1.Value = (int)(session.GetVolume() * trackBar1.Maximum);
        }

        /*
         * private class SessionNotification : AudioUtilities.IAudioSessionNotification
                {
                    public void OnSessionCreated(AudioUtilities.IAudioSessionControl NewSession)
                    {

                    }
                }*/

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Enabled = false;
            this.trackBar1.Location = new System.Drawing.Point(23, 27);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 104);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.PortName = "COM3";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(0, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(88, 21);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(91, 134);
            this.panel1.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(97, 140);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(622, 542);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }


}

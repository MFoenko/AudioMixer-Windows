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

        public Panel[] panels;
        public delegate void DataDelegate(int control, float val);
        public DataDelegate myDelegate;

        public Form1()
        {
            InitializeComponent();
            panels = new Panel[2];
            panels[0] = CreateControl(0);
            panels[1] = CreateControl(1);

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
                    foreach(Panel panel in panels)
                    {
                        ComboBox comboBox = panel.Controls.Find("comboBox", true).FirstOrDefault() as ComboBox;
                        comboBox.Items.Add(session);
                    }
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
            if (control < 0 || control >= panels.Length) return;
            TrackBar trackBar = panels[control].Controls.Find("trackBar", true).FirstOrDefault() as TrackBar;
            ComboBox comboBox = panels[control].Controls.Find("comboBox", true).FirstOrDefault() as ComboBox;
            trackBar.Value = (int)(val * trackBar.Maximum);
            AudioSession session = (AudioSession)comboBox.SelectedItem;
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
            panels[control].Invoke(myDelegate, new object[] { control, v });

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

        private Panel CreateControl(int i)
        {
            

            ComboBox comboBox = new System.Windows.Forms.ComboBox();
            comboBox.FormattingEnabled = true;
            comboBox.Location = new System.Drawing.Point(0, 0);
            comboBox.Name = "comboBox";
            comboBox.Size = new System.Drawing.Size(100, 20);
            comboBox.TabIndex = 2;
            comboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);

            TrackBar trackBar = new System.Windows.Forms.TrackBar();
            trackBar.Enabled = false;
            trackBar.Location = new System.Drawing.Point(25, 20);
            trackBar.Maximum = 100;
            trackBar.Name = "trackBar";
            trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            trackBar.Size = new System.Drawing.Size(50, 100);
            trackBar.TabIndex = 0;
            trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;

            Panel panel = new System.Windows.Forms.Panel();
            panel.AutoSize = true;
            panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            panel.Controls.Add(comboBox);
            panel.Controls.Add(trackBar);
            panel.Location = new System.Drawing.Point(3, 3);
            panel.Name = "panel" + i;
            //panel.TabIndex = 4;

            this.tableLayoutPanel1.Controls.Add(panel);
            return panel;
        }

      

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }


}

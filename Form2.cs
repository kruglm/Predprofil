using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Alturos.Yolo;
using System.Speech.Synthesis;
using System.Timers;


namespace ProjectWork
{
    public partial class Form2 : Form
    {
        YoloWrapper yoloWrapper;
        Form1 temp;
        private static System.Timers.Timer aTimer;
        SpeechSynthesizer synth = new SpeechSynthesizer();
        VideoCaptureDevice camera;
        int j = 0;
        BackgroundWorker bgWorker;


        public Form2(VideoCaptureDevice cameraIn, Form1 f)
        {   
            InitializeComponent();
            yoloWrapper = new YoloWrapper(@".\YoloFiles\yolov3-tiny.cfg", @".\YoloFiles\yolov3-tiny.weights", @".\YoloFiles\coco.names");
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += BgWorker_DoWork;
            synth.SetOutputToDefaultAudioDevice();
            camera = cameraIn;
            camera.NewFrame += camera_NewFrame;
            camera.Start();
            this.Size = new Size(camera.VideoResolution.FrameSize.Width, camera.VideoResolution.FrameSize.Height+150);
            pic.Size = camera.VideoResolution.FrameSize;           
            Log.Location = new Point(0, camera.VideoResolution.FrameSize.Height);
            Log.Size = new Size(camera.VideoResolution.FrameSize.Width-15, 110);
            temp = f;
            //aTimer = new System.Timers.Timer();
            //aTimer.Interval = 2000;
            //aTimer.Elapsed += OnTimedEvent;
            //aTimer.AutoReset = true;
            //aTimer.Enabled = true;
            Log.ReadOnly = true;
            Log.ScrollBars = ScrollBars.Vertical;
            Log.Text = "Log:";            
            bgWorker.RunWorkerAsync();
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (bgWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                byte[] bytesImage = null;
                if (pic.InvokeRequired)
                    pic.Invoke(new Action(() =>
                    {
                        bytesImage = ImageToByte(pic.Image);
                    }));
                else
                    bytesImage = ImageToByte(pic.Image);
                IEnumerable<Alturos.Yolo.Model.YoloItem> items = null;
                try
                {
                    items = yoloWrapper.Detect(bytesImage);
                }
                catch
                {
                    continue;
                }
                
                if (items.Count() == 0)
                {
                    if (Log.InvokeRequired)
                        Log.Invoke(new Action(() =>
                        {
                            Log.Text += Environment.NewLine + "Log № " + j + " Объекты не определены";

                        }));
                }
                foreach (var p in items)
                {

                    synth.Speak(p.Type);
                    if (Log.InvokeRequired)
                        Log.Invoke(new Action(() =>
                        {
                            Log.Text += Environment.NewLine + "Log № " + j + " ";
                            Log.Text += p.Type + " " + p.Confidence * 100 + "%";
                        }));
                    else
                    {
                        Log.Text += Environment.NewLine + "Log № " + j + " ";
                        Log.Text += p.Type + " " + p.Confidence * 100 + "%";
                    }
                }
                j++;
            }
        }

        private void camera_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pic.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void OnTimedEvent(Object sender, System.Timers.ElapsedEventArgs e)
        {
            byte[] bytesImage = null;
            if (pic.InvokeRequired)
                pic.Invoke(new Action(() =>
                {
                    bytesImage = ImageToByte(pic.Image);
                }));
            else
                bytesImage = ImageToByte(pic.Image);
            var items = yoloWrapper.Detect(bytesImage);
            if (items.Count() == 0)
            {
                if (Log.InvokeRequired)
                    Log.Invoke(new Action(() =>
                    {
                        Log.Text += Environment.NewLine + "Log № " + j + " Объекты не определены";
                        
                    }));                
            }
            foreach (var p in items)
            {
                
                synth.Speak(p.Type);
                if (Log.InvokeRequired)
                    Log.Invoke(new Action(() =>
                    {
                        Log.Text += Environment.NewLine + "Log № " + j + " ";
                        Log.Text +=  p.Type + " " + p.Confidence*100+"%";                      
                    }));
                else
                {
                    Log.Text +=  Environment.NewLine + "Log № " + j + " ";
                    Log.Text +=  p.Type + " " + p.Confidence*100 +"%";
                }
            }
            j++;

        }
        public byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            //aTimer.Stop();
            //aTimer.Dispose();
            bgWorker.CancelAsync();
            camera.Stop();
            //temp.Close();
            Application.Exit();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Log_TextChanged(object sender, EventArgs e)
        {
            Log.Focus();
            Log.Select(Log.Text.Length, 0);
            Log.ScrollToCaret();
            //Log.SelectionLength = 0;
        }
    }
}

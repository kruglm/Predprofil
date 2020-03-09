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

namespace ProjectWork
{
    public partial class Form1 : Form
    {
        //Коллекция Камер
        FilterInfoCollection filterinfocollection;
        VideoCaptureDevice videoCaptureDevice;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Загрузка Камер в combobox
            filterinfocollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterinfo in filterinfocollection)
                camerabox.Items.Add(filterinfo.Name);
            camerabox.SelectedIndex = 0;
            //Выбор Камеры
            videoCaptureDevice = new VideoCaptureDevice();
            camerabox.SelectedIndexChanged += camerabox_SelectedIndexChanged;
            if (camerabox.Items.Count > 0)
                camerabox.SelectedIndex = 0;
            camerabox_SelectedIndexChanged(null, null);
            rebox.SelectedIndexChanged += rebox_SelectedIndexChanged;
            if (rebox.Items.Count > 0)
                rebox.SelectedIndex = 0;


        }
        void camerabox_SelectedIndexChanged(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice(filterinfocollection[camerabox.SelectedIndex].MonikerString);
            //Загрузка Разрешений в combobox
            rebox.Items.Clear();
            for (int i = 0; i < videoCaptureDevice.VideoCapabilities.Length; i++)
            {
                string resolution_size = videoCaptureDevice.VideoCapabilities[i].FrameSize.ToString();
                rebox.Items.Add(resolution_size);
            }
        }
        void rebox_SelectedIndexChanged(object sender, EventArgs e)
        {
            videoCaptureDevice.VideoResolution = videoCaptureDevice.VideoCapabilities[rebox.SelectedIndex];
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form2 newForm = new Form2(videoCaptureDevice, this);
            newForm.Show();
            this.Hide();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

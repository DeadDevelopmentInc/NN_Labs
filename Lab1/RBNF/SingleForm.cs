using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RBNF
{
    public partial class SingleForm : Form
    {
        RBNF.RadialNetwork _RBFN;
        string testPath;

        public SingleForm()
        {
            InitializeComponent();
            trainingSamples = new List<List<double>>();
            trainingLabels = new List<List<double>>();
            testSamples = new List<List<double>>();
            testLabels = new List<List<double>>();

        }

        private void RB_Train()
        {
            int K = 4;
            int numOfEpocks = 5;
            double learningRate = 0.001;
            double maxError = 0.3;

            _RBFN = new RadialNetwork(25, 5, 1);
            textBox1.Text = _RBFN.

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("rbfn.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, _RBFN);

                MessageBox.Show("Объект сериализован");
            }
        }

        private void trainButton_Click(object sender, EventArgs e)
        {
            RB_Train();
        }

        private void testButton_Click(object sender, EventArgs e)
        {
        }

        private void recButton_Click(object sender, EventArgs e)
        {
            string fileToDisplay = null;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileToDisplay = openFileDialog1.FileName;
                testPath = fileToDisplay;
            }
            var img = new Bitmap(fileToDisplay);
            pictureBox1.ClientSize = new Size(30, 30);
            pictureBox1.Image = img;
        }

        private void getData()
        {
            double[][] allData = new double[20][];
            allData[0] = new double[] { -0.784, 1.255, -1.332, -1.306, 0, 0, 1 };
            allData[1] = new double[] { -0.995, -0.109, -1.332, -1.306, 0, 0, 1 };
            allData[2] = new double[] { -1.206, 0.436, -1.386, -1.306, 0, 0, 1 };
            allData[3] = new double[] { -1.312, 0.164, -1.278, -1.306, 0, 0, 1 };
            allData[4] = new double[] { -0.890, 1.528, -1.332, -1.306, 0, 0, 1 };
            allData[5] = new double[] { -0.468, 2.346, -1.170, -1.048, 0, 0, 1 };
            allData[6] = new double[] { -1.312, 0.982, -1.332, -1.177, 0, 0, 1 };
            allData[7] = new double[] { -0.890, 0.982, -1.278, -1.306, 0, 0, 1 };
            allData[8] = new double[] { -1.523, -0.382, -1.332, -1.306, 0, 0, 1 };
            allData[9] = new double[] { -0.995, 0.164, -1.278, -1.435, 0, 0, 1 };

            allData[10] = new double[] { 1.220, 0.436, 0.452, 0.241, 0, 1, 0 };
            allData[11] = new double[] { 0.587, 0.436, 0.344, 0.370, 0, 1, 0 };
            allData[12] = new double[] { 1.115, 0.164, 0.560, 0.370, 0, 1, 0 };
            allData[13] = new double[] { -0.362, -2.019, 0.074, 0.112, 0, 1, 0 };
            allData[14] = new double[] { 0.693, -0.655, 0.398, 0.370, 0, 1, 0 };
            allData[15] = new double[] { -0.151, -0.655, 0.344, 0.112, 0, 1, 0 };
            allData[16] = new double[] { 0.482, 0.709, 0.452, 0.498, 0, 1, 0 };
            allData[17] = new double[] { -0.995, -1.746, -0.305, -0.275, 0, 1, 0 };
            allData[18] = new double[] { 0.798, -0.382, 0.398, 0.112, 0, 1, 0 };
            allData[19] = new double[] { -0.679, -0.927, 0.020, 0.241, 0, 1, 0 };
        }
        private int checkClass(List<double> actualOutput)
        {
        }
    }
}

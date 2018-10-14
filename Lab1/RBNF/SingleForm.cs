using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RBNF
{
    public partial class SingleForm : Form
    {
        Preprocessing.Dataset DataSet;
        RBFN _RBF;
        private int TestingNoInClass = 30;
        private List<List<double>> trainingSamples;
        private List<List<double>> trainingLabels;
        private List<List<double>> testSamples;
        private List<List<double>> testLabels;
        private List<List<double>> Centroids;
        private string trainingSamplesAfterPCA = "trainingSamplesAfterPCA.txt",
            trainingLabelsAfterPCA = "trainingLabelsAfterPCA.txt",
            testSamplesAfterPCA = "testSamplesAfterPCA.txt",
            testLabelsAfterPCA = "testLabelsAfterPCA.txt";

        public SingleForm()
        {
            InitializeComponent();
            
        }

        private void trainButton_Click(object sender, EventArgs e)
        {

        }

        private void testButton_Click(object sender, EventArgs e)
        {

        }

        private void recButton_Click(object sender, EventArgs e)
        {

        }
    }
}

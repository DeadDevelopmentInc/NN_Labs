using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadialNetwork
{
    public class Neuron
    {
        public double[] Centroids { get; set; }
        public double Input { get; set; }
        public double Width { get; set; }
        public double[] HoWeights { get; set; }
        public double OBias { get; set; }
        public double Output { get; set; }

        public Neuron(int numInput, int numOutput)
        {
            Centroids = new double[numInput];
            HoWeights = new double[numOutput];
        }

        public Neuron()
        {

        }
    }
}


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadialNetwork
{
    public class Layer
    {

        private int numHidden;
        private int numOutput;
        private int numInput;

        public List<Neuron> Neurons { get; } = new List<Neuron>();

        public Layer(int numOfNeurons)
        {
            for (int i = 0; i < numOfNeurons; i++) { Neurons.Add(new Neuron()); }
        }

        public Layer(int numInput, int numHidden, int numOutput)
        {
            this.numHidden = numHidden;
            this.numOutput = numOutput;
            for(int i = 0; i < numHidden; i++) { Neurons.Add(new Neuron(numInput, numOutput)); }
        }

        public double[][] GetCentroids()
        {
            var centroids = new double[Neurons.Count][];
            for(int i = 0; i < centroids.Length; i++)
            {
                centroids[i] = Neurons[i].Centroids;
            }
            return centroids;
        }

        public void SetInputs()
        {

        }
    }
}

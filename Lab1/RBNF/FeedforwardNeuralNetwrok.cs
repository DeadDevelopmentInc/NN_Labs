using System;
using System.Collections.Generic;

namespace RBNF
{
    public class FeedforwardNeuralNetwrok
    {
        private int numOfLayers;
        private int numOfInput;
        private List<List<Neuron>> network;
        private List<int> numOfNeuronsPerLayer;
        private double ErrorSqr = 0;

        public double GetErrorSqr
        {
            get { return ErrorSqr; }
        }
        /// <summary>
        /// Build the network given the number of layers
        /// </summary>
        /// <param name="numOfLayers">An integer that represents the number of layers of the network</param>
        public FeedforwardNeuralNetwrok(int numOfLayers)
        {
            if (numOfLayers < 2)
                throw new Exception("Can't Initiate Network with lower than 2 layers");
            this.numOfLayers = numOfLayers;
            this.network = new List<List<Neuron>>();
        }

        /// <summary>
        /// Construct the network given the number of neurons per layer.
        /// </summary>
        /// <param name="numOfNeuronsPerLayer">A List that represents 
        /// the number of neurons for each layer in the network.
        /// </param>
        public void setNetwork(List<int> numOfNeuronsPerLayer)
        {
            if (this.numOfLayers != numOfNeuronsPerLayer.Count)
                throw new Exception("Wrong List size for numOfNeuronsPerLayer");

            this.numOfNeuronsPerLayer = numOfNeuronsPerLayer;

            this.numOfInput = this.numOfNeuronsPerLayer[0];

            for (int i = 1; i < this.numOfLayers; ++i)
            {
                this.network.Add(new List<Neuron>());
            }

        }

        /// <summary>
		/// Create neurons and add them to specific <paramref name="layerIndex"/> with the activation function of the layer.
        /// </summary>
        /// <param name="layerIndex">Index of the layer in the network [starting from 1]</param>
        /// <param name="activationFunction">The activation function that will be used 
        /// in the output of each neuron in the layer.</param>
        public void setLayer(int layerIndex, mathFunction activationFunction)
        {
            if (layerIndex == 0)
                throw new Exception("Can't set Input Layer");

            for (int i = 0; i < this.numOfNeuronsPerLayer[layerIndex]; ++i)
            {
                Neuron neuron = new Neuron(this.numOfNeuronsPerLayer[layerIndex - 1], activationFunction, false);
                this.network[layerIndex - 1].Add(neuron);
            }
        }

        // least mean square 
        public void LMSsetLayer(int layerIndex, mathFunction activationFunction)
        {
            if (layerIndex == 0)
                throw new Exception("Can't set Input Layer");

            for (int i = 0; i < this.numOfNeuronsPerLayer[layerIndex]; ++i)
            {
                Neuron neuron = new Neuron(this.numOfNeuronsPerLayer[layerIndex - 1], activationFunction, true);
                this.network[layerIndex - 1].Add(neuron);
            }
        }

        /// <summary>
        /// Set neuron in layer given its index, the index of neuron, the weights and bias.
        /// </summary>
        /// <param name="layerIndex">Index of the layer the network [starting from 1]</param>
        /// <param name="neuronIndex">Index of the layer the network [0 base]</param>
        /// <param name="weights">The weights of the neuron.</param>
        /// <param name="bias">The bias of the neuron.</param>
        public void setNeuron(int layerIndex, int neuronIndex, List<double> weights, double bias)
        {
            if (layerIndex == 0)
                throw new Exception("no neuron at Input Layer");
            if (weights.Count != this.numOfNeuronsPerLayer[layerIndex - 1])
                throw new Exception("Invalid weights size");

            this.network[layerIndex - 1][neuronIndex].update(weights, bias);
        }

        /// <summary>
        /// Compute the output of the network.
        /// </summary>
        /// <param name="input"> Network input [Features]</param>
        /// <returns>List of the output values after feedforward</returns>
        public List<double> feedforward(List<double> input)
        {
            if (this.numOfInput != input.Count)
                throw new Exception("Invalid input size");

            List<double> currentInput = input;
            List<double> nextInput = new List<double>();	// nextInput = currentOutput

            for (int i = 1; i < this.numOfLayers; ++i)
            {
                for (int j = 0; j < this.numOfNeuronsPerLayer[i]; ++j)
                {
                    nextInput.Add(this.network[i - 1][j].feedforward(currentInput));
                }
                // Prepare input to next layer
                currentInput = nextInput;
                nextInput = new List<double>();
            }

            List<double> output = new List<double>();
            for (int i = 0; i < currentInput.Count; i++)
            {
                output.Add(currentInput[i]);
            }
            return output;
        }

        /// <summary>
        /// Train the network given the training samples (features) and the labels (classes),
        /// then using the chosen learning algorithm to update the weights.
        /// </summary>
        /// <param name="trainingSamples"> Training Samples Features</param>
        /// <param name="trainingLabels"> Training Labels (classes)</param>
        /// <param name="learningRate">Training learning Rate</param>
        /// <param name="learningAlgorithm">The learning algorithm that will be used to train the network</param>
        public void train(List<List<double>> trainingSamples, List<List<double>> trainingLabels, double learningRate, Backpropagation learningAlgorithm)
        {
            double sum = 0;
            for (int i = 0; i < trainingSamples.Count; ++i)
            {
                List<double> output = this.feedforward(trainingSamples[i]);
                this.network = learningAlgorithm.Learn(learningRate, trainingSamples[i], trainingLabels[i], this.network);
                for (int j = 0; j < network[0].Count; j++)
                {
                    sum += Math.Pow(network[0][j].SignalError, 2);
                }
            }
            ErrorSqr = sum / network[0].Count;
        }
    }
}
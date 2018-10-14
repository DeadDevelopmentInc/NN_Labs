using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBNF
{
    public class Backpropagation
    {
        public Backpropagation() { }

        public List<List<Neuron>> Learn(double learningRate, List<double> input, List<double> output, List<List<Neuron>> network)
        {
            // Backward "calculate Signal Error"
            for (int i = network.Count - 1; i >= 0; --i)
            {
                for (int j = 0; j < network[i].Count; ++j)
                {
                    Neuron currentNeuron = network[i][j];

                    if (i == network.Count - 1)
                    {
                        currentNeuron.SignalError = (output[j] - currentNeuron.Output) * currentNeuron.ActivationFunction.derivative(currentNeuron.Net);
                    }
                    else
                    {
                        double sum = 0;
                        for (int k = 0; k < network[i + 1].Count; ++k)
                        {
                            Neuron nextNeuron = network[i + 1][k];
                            sum += (nextNeuron.Weights[j] * nextNeuron.SignalError);
                        }
                        currentNeuron.SignalError = sum * currentNeuron.ActivationFunction.derivative(currentNeuron.Net);

                    }

                    network[i][j] = currentNeuron;
                }
            }

            // Forward "calcualte Delta and Update"
            for (int i = 0; i < network.Count; ++i)
            {
                for (int j = 0; j < network[i].Count; ++j)
                {
                    List<double> currentWeights = network[i][j].Weights;
                    double currentBias = network[i][j].Bias;

                    // Updating Weights
                    for (int k = 0; k < currentWeights.Count; ++k)
                    {
                        double delta;
                        if (i == 0)
                        {
                            delta = learningRate * network[i][j].SignalError * input[k];
                        }
                        else
                        {
                            delta = learningRate * network[i][j].SignalError * network[i - 1][k].Output;
                        }
                        currentWeights[k] += delta;
                    }
                    // Updating Bias
                    currentBias += learningRate * network[i][j].SignalError;

                    network[i][j].update(currentWeights, currentBias);
                }
            }

            return network;
        }
    }
}

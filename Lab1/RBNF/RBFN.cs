using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBNF
{
    class RBFN
    {
        private int TestingNoInClass = 30;
        private int numOfEpocks;
        private double maxError, learningRate;
        private List<List<double>> Centroids; // means of all features 
        private List<double> Variance;
        private List<List<double>> trainingSamples, trainingLabels, testSamples, testLabels;
        EM emAlg;
        FeedforwardNeuralNetwrok neuralNetwork;
        Backpropagation backpropagation;

        public RBFN(int K, int numOfEpocks, double learningRate, double maxError, List<List<double>> trainingSamples, List<List<double>> trainingLabels, List<List<double>> testSamples, List<List<double>> testLabels)
        {
            this.numOfEpocks = numOfEpocks;
            this.trainingSamples = trainingSamples;
            this.trainingLabels = trainingLabels;
            this.testSamples = testSamples;
            this.testLabels = testLabels;
            this.maxError = maxError;
            this.learningRate = learningRate;
            Centroids = new List<List<double>>();  //[NumOfCluster][NumOfFeatures (means)]
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int counter = K; // Num of Clusters
            while (counter-- > 0)
            {
                int index = rnd.Next(trainingSamples.Count);
                Centroids.Add(trainingSamples[index]);
            }

            emAlg = new EM(ref Centroids, trainingSamples);
            Variance = emAlg.GetVariance;
            ////

            ////
            List<int> numOfNeuronsPerLayer = new List<int>();
            backpropagation = new Backpropagation();

            numOfNeuronsPerLayer.Add(K);
            numOfNeuronsPerLayer.Add(3);

            this.neuralNetwork = new FeedforwardNeuralNetwrok(2);
            this.neuralNetwork.setNetwork(numOfNeuronsPerLayer);

            this.neuralNetwork.LMSsetLayer(1, new IdentityFunction());

        }

        public void Train()
        {
            for (int E = 0; E < numOfEpocks; ++E)
            {
                double Error = 0;
                for (int index = 0; index < trainingSamples.Count; index++)
                {
                    List<double> G = new List<double>();
                    for (int i = 0; i < Variance.Count; i++)
                    {
                        double R_Sqr = 0;

                        for (int col = 0; col < trainingSamples[index].Count; col++)
                        {
                            double r = Math.Abs(trainingSamples[index][col] - Centroids[i][col]);
                            R_Sqr += (r * r);
                        }

                        G.Add(Math.Exp((-1 * R_Sqr) / (2 * Variance[i] * Variance[i])));
                    }
                    this.neuralNetwork.train(new List<List<double>>() { G }, new List<List<double>>() { trainingLabels[index] }, learningRate, backpropagation);
                    Error += (this.neuralNetwork.GetErrorSqr);
                }
                Error /= trainingSamples.Count;
                if (Error < maxError)
                    return;
            }
        }

        public int[,] Test()
        {
            int[,] matrix = new int[3, 4];

            for (int index = 0; index < testSamples.Count; index++)
            {
                List<double> G = new List<double>();
                for (int i = 0; i < Variance.Count; i++)
                {
                    double R_Sqr = 0;

                    for (int col = 0; col < testSamples[index].Count; col++)
                    {
                        double r = Math.Abs(testSamples[index][col] - Centroids[i][col]);
                        R_Sqr += (r * r);
                    }

                    G.Add(Math.Exp((-1 * R_Sqr) / (2 * Variance[i] * Variance[i])));
                }
                List<double> actualOutput = this.neuralNetwork.feedforward(G);
                int actualClass = this.checkClass(actualOutput);
                int desiredClass = index / TestingNoInClass;

                if (actualClass == -1)
                    actualClass = 3;
                matrix[desiredClass, actualClass]++;
            }

            return matrix;
        }
        private int checkClass(List<double> actualOutput)
        {
            int index = 0;
            double max = 0;
            for (int i = 0; i < actualOutput.Count; ++i)
            {
                if (actualOutput[i] > max)
                {
                    max = actualOutput[i];
                    index = i;
                }
            }

            int counter = 0;
            for (int i = 0; i < actualOutput.Count; ++i)
            {
                if (max == actualOutput[i])
                {
                    counter++;
                }
                else
                    break;
            }
            if (counter == actualOutput.Count)
                return -1;
            return index;
        }
    }


}

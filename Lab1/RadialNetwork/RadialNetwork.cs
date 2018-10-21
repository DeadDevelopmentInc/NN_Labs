using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadialNetwork
{
    public class RadialNetwork
    {
        private static Random rnd = null;
        private int numInput;
        private int numHidden;
        private int numOutput;
        private double[] inputs;
        private double[][] centroids;
        private double[] widths;
        private double[][] hoWeights;
        private double[] oBiases;
        private double[] outputs;

        public RadialNetwork(int numInput, int numHidden, int numOutput)
        {
            rnd = new Random(0);
            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;
            this.inputs = new double[numInput];
            this.centroids = MakeMatrix(numHidden, numInput);
            this.widths = new double[numHidden];
            this.hoWeights = MakeMatrix(numHidden, numOutput);
            this.oBiases = new double[numOutput];
            this.outputs = new double[numOutput];
        } // ctor

        private static double[][] MakeMatrix(int rows, int cols) // helper for ctor
        {
            double[][] result = new double[rows][];
            for (int r = 0; r < rows; ++r)
                result[r] = new double[cols];
            return result;
        }

        // -- methods related to getting and setting centroids, widths, weights, bias values ----------

        public void SetWeights(double[] weights)
        {
            // this.hoWeights has numHidden row and numOutput cols
            // this.oBiases has numOutput values
            if (weights.Length != (numHidden * numOutput) + numOutput)
                throw new Exception("Bad weights length in SetWeights");
            int k = 0; // ptr into weights
            for (int i = 0; i < numHidden; ++i)
                for (int j = 0; j < numOutput; ++j)
                    this.hoWeights[i][j] = weights[k++];
            for (int i = 0; i < numOutput; ++i)
                this.oBiases[i] = weights[k++];
        }

        public double[] GetWeights()
        {
            double[] result = new double[numHidden * numOutput + numOutput];
            int k = 0;
            for (int i = 0; i < numHidden; ++i)
                for (int j = 0; j < numOutput; ++j)
                    result[k++] = this.hoWeights[i][j];
            for (int i = 0; i < numOutput; ++i)
                result[k++] = this.oBiases[i];
            return result;
        }

        // put GetAllWeights() and SetAllWeights() here to fetch and set
        // centroid, width, weight, and bias values as a group

        // -- methods related to training error and test classification accuracy ----------------------

        private double MeanSquaredError(double[][] trainData, double[] weights)
        {
            // assumes that centroids and widths have been set!
            this.SetWeights(weights); // copy the weights to valuate in

            double[] xValues = new double[numInput]; // inputs
            double[] tValues = new double[numOutput]; // targets
            double sumSquaredError = 0.0;
            for (int i = 0; i < trainData.Length; ++i) // walk through each trainingb data item
            {
                // following assumes data has all x-values first, followed by y-values!
                Array.Copy(trainData[i], xValues, numInput); // extract inputs
                Array.Copy(trainData[i], numInput, tValues, 0, numOutput); // extract targets
                double[] yValues = this.ComputeOutputs(xValues); // compute the outputs using centroids, widths, weights, bias values
                for (int j = 0; j < yValues.Length; ++j)
                    sumSquaredError += ((yValues[j] - tValues[j]) * (yValues[j] - tValues[j]));
            }
            return sumSquaredError / trainData.Length;
        }

        // consider MeanCrossEntropy() here as an alternative to MSE

        public double Accuracy(double[][] testData)
        {
            // percentage correct using winner-takes all
            int numCorrect = 0;
            int numWrong = 0;
            double[] xValues = new double[numInput]; // inputs
            double[] tValues = new double[numOutput]; // targets

            for (int i = 0; i < testData.Length; ++i)
            {
                Array.Copy(testData[i], xValues, numInput); // parse test data into x-values and t-values
                Array.Copy(testData[i], numInput, tValues, 0, numOutput);
                if (tValues[Predict(xValues)] == 1.0) // ugly. consider AreEqual(double x, double y)
                    ++numCorrect;
                else
                    ++numWrong;
            }
            return (numCorrect * 1.0) / (numCorrect + numWrong); // ugly 2 - check for divide by zero
        }

        public int Predict(double[] xValues)
        {
            double[] yValues; // computed Y
            yValues = this.ComputeOutputs(xValues);
            return MaxIndex(yValues);
        }

        private static int MaxIndex(double[] vector) // helper for Accuracy()
        {
            // index of largest value
            int bigIndex = 0;
            double biggestVal = vector[0];
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i] > biggestVal)
                {
                    biggestVal = vector[i]; bigIndex = i;
                }
            }
            return bigIndex;
        }

        // -- methods related to RBF network input-output mechanism -----------------------------------

        public double[] ComputeOutputs(double[] xValues)
        {
            // use centroids, widths, weights and input xValues to compute, store, and return numOutputs output values
            Array.Copy(xValues, this.inputs, xValues.Length); // place data inputs into RBF net inputs

            double[] hOutputs = new double[numHidden]; // hidden node outputs
            for (int j = 0; j < numHidden; ++j) // each hidden node
            {
                double d = EuclideanDist(inputs, centroids[j], inputs.Length); // could use a 'distSquared' approach
                                                                               //Console.WriteLine("\nHidden[" + j + "] distance = " + d.ToString("F4"));
                double r = -1.0 * (d * d) / (2 * widths[j] * widths[j]);
                double g = Math.Exp(r);
                //Console.WriteLine("Hidden[" + j + "] output = " + g.ToString("F4"));
                hOutputs[j] = g;
            }

            double[] tempResults = new double[numOutput];

            for (int k = 0; k < numOutput; ++k)
                for (int j = 0; j < numHidden; ++j)
                    tempResults[k] += (hOutputs[j] * hoWeights[j][k]); // accumulate

            for (int k = 0; k < numOutput; ++k)
                tempResults[k] += oBiases[k]; // add biases

            double[] finalOutputs = Softmax(tempResults); // scale the raw output so values sum to 1.0

            //Console.WriteLine("outputs:");
            //Helpers.ShowVector(finalOutputs, 3, finalOutputs.Length, true);
            //Console.ReadLine();

            Array.Copy(finalOutputs, this.outputs, finalOutputs.Length); // transfer computed outputs to RBF net outputs

            double[] returnResult = new double[numOutput]; // also return computed outputs for convenience
            Array.Copy(finalOutputs, returnResult, outputs.Length);
            return returnResult;
        } // ComputeOutputs



        private static double[] Softmax(double[] rawOutputs)
        {
            // helper for ComputeOutputs
            // does all output nodes at once so scale doesn't have to be re-computed each time
            // determine max output sum
            double max = rawOutputs[0];
            for (int i = 0; i < rawOutputs.Length; ++i)
                if (rawOutputs[i] > max) max = rawOutputs[i];

            // determine scaling factor -- sum of exp(each val - max)
            double scale = 0.0;
            for (int i = 0; i < rawOutputs.Length; ++i)
                scale += Math.Exp(rawOutputs[i] - max);

            double[] result = new double[rawOutputs.Length];
            for (int i = 0; i < rawOutputs.Length; ++i)
                result[i] = Math.Exp(rawOutputs[i] - max) / scale;

            return result; // now scaled so that all values sum to 1.0
        }

        // -- methods related to training: DoCentroids, DistinctIndices, AvgAbsDist, DoWidths,
        //    DoWeights, Train, Shuffle ----------------------------------------------------------

        private void DoCentroids(double[][] trainData)
        {
            // centroids are representative inputs that are relatively different
            // compute centroids using the x-vaue of training data
            // store into this.centroids
            int numAttempts = trainData.Length;
            int[] goodIndices = new int[numHidden];  // need one centroid for each hidden node
            double maxAvgDistance = double.MinValue; // largest average distance for a set of candidate indices
            for (int i = 0; i < numAttempts; ++i)
            {
                int[] randomIndices = DistinctIndices(numHidden, trainData.Length); // candidate indices
                double sumDists = 0.0; // sum of distances between adjacent candidates (not all candiates)
                for (int j = 0; j < randomIndices.Length - 1; ++j) // adjacent pairs only
                {
                    int firstIndex = randomIndices[j];
                    int secondIndex = randomIndices[j + 1];
                    sumDists += AvgAbsDist(trainData[firstIndex], trainData[secondIndex], numInput); // just the input terms
                }

                double estAvgDist = sumDists / numInput; // estimated average distance for curr candidates
                if (estAvgDist > maxAvgDistance) // curr candidates are far apart
                {
                    maxAvgDistance = estAvgDist;
                    Array.Copy(randomIndices, goodIndices, randomIndices.Length); // save curr candidates
                }
            } // now try a new set of candidates

            Console.WriteLine("The indices (into training data) of the centroids are:");
            Helpers.ShowVector(goodIndices, goodIndices.Length, true);

            // store copies of x-vales of data pointed to by good indices into this.centroids
            for (int i = 0; i < numHidden; ++i)
            {
                int idx = goodIndices[i]; // idx points to trainData
                for (int j = 0; j < numInput; ++j)
                {
                    this.centroids[i][j] = trainData[idx][j]; // make a copy of values
                }
            }
        } // DoCentroids

        private static double AvgAbsDist(double[] v1, double[] v2, int numTerms)
        {
            // average absolute difference distance between two vectors, first numTerms only
            // helper for computing centroids
            if (v1.Length != v2.Length)
                throw new Exception("Vector lengths not equal in AvgAbsDist()");
            double sum = 0.0;
            for (int i = 0; i < numTerms; ++i)
            {
                double delta = Math.Abs(v1[i] - v2[i]);
                sum += delta;
            }
            return sum / numTerms;
        }

        private int[] DistinctIndices(int n, int range)
        {
            // helper for ComputeCentroids()
            // generate n distinct numbers in [0, range-1] using reservoir sampling
            // assumes rnd exists
            int[] result = new int[n];
            for (int i = 0; i < n; ++i)
                result[i] = i;

            for (int t = n; t < range; ++t)
            {
                int m = rnd.Next(0, t + 1);
                if (m < n) result[m] = t;
            }
            return result;
        }

        private void DoWidths(double[][] centroids)
        {
            // compute widths based on centroids, store into this.widths
            // note the centroids parameter could be omitted - the intent is to make relationship clear
            // this version uses a common width which is the average dist between all centroids
            double sumOfDists = 0.0;
            int ct = 0; // could calculate number pairs instead
            for (int i = 0; i < centroids.Length - 1; ++i)
            {
                for (int j = i + 1; j < centroids.Length; ++j)
                {
                    double dist = EuclideanDist(centroids[i], centroids[j], centroids[i].Length);
                    sumOfDists += dist;
                    ++ct;
                }
            }
            double avgDist = sumOfDists / ct;
            double width = avgDist;

            Console.WriteLine("The common width is: " + width.ToString("F4"));

            for (int i = 0; i < this.widths.Length; ++i) // all widths the same
                this.widths[i] = width;
        }

        //private void DoWidths(double[][] centroids)
        //{
        //  // this version uses a common width which is dmax / sqrt(2*numHidden)
        //  double maxDist = double.MinValue;
        //  for (int i = 0; i < centroids.Length - 1; ++i)
        //  {
        //    for (int j = i + 1; j < centroids.Length; ++j)
        //    {
        //      double dist = EuclideanDist(centroids[i], centroids[j], centroids[i].Length);
        //      if (dist > maxDist)
        //        maxDist = dist;
        //    }
        //  }

        //  double width = maxDist / Math.Sqrt(2.0 * numHidden);
        //  Console.WriteLine("The common width is: " + width.ToString("F4"));
        //  for (int i = 0; i < this.widths.Length; ++i) // all widths the same
        //    this.widths[i] = width;
        //}

        private double[] DoWeights(double[][] trainData, int maxIterations)
        {
            // use PSO to find weights and bias values that produce a RBF network
            // that best matches training data
            int numberParticles = trainData.Length / 3;

            int Dim = (numHidden * numOutput) + numOutput; // dimensions is num weights + num biases
            double minX = -10.0; // implicitly assumes data has been normalizzed
            double maxX = 10.0;
            double minV = minX;
            double maxV = maxX;
            Particle[] swarm = new Particle[numberParticles];
            double[] bestGlobalPosition = new double[Dim]; // best solution found by any particle in the swarm. implicit initialization to all 0.0
            double smallesttGlobalError = double.MaxValue; // smaller values better

            // initialize swarm
            for (int i = 0; i < swarm.Length; ++i) // initialize each Particle in the swarm
            {
                double[] randomPosition = new double[Dim];
                for (int j = 0; j < randomPosition.Length; ++j)
                {
                    double lo = minX;
                    double hi = maxX;
                    randomPosition[j] = (hi - lo) * rnd.NextDouble() + lo; // 
                }

                double err = MeanSquaredError(trainData, randomPosition); // error associated with the random position/solution
                double[] randomVelocity = new double[Dim];

                for (int j = 0; j < randomVelocity.Length; ++j)
                {
                    double lo = -1.0 * Math.Abs(maxV - minV);
                    double hi = Math.Abs(maxV - minV);
                    randomVelocity[j] = (hi - lo) * rnd.NextDouble() + lo;
                }
                swarm[i] = new Particle(randomPosition, err, randomVelocity, randomPosition, err);

                // does current Particle have global best position/solution?
                if (swarm[i].error < smallesttGlobalError)
                {
                    smallesttGlobalError = swarm[i].error;
                    swarm[i].position.CopyTo(bestGlobalPosition, 0);
                }
            } // initialization

            // main PSO algorithm
            // compute new velocity -> compute new position -> check if new best

            double w = 0.729; // inertia weight
            double c1 = 1.49445; // cognitive/local weight
            double c2 = 1.49445; // social/global weight
            double r1, r2; // cognitive and social randomizations

            int[] sequence = new int[numberParticles]; // process particles in random order
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;

            int iteration = 0;
            while (iteration < maxIterations)
            {
                if (smallesttGlobalError < 0.060) break; // early exit (MSE)

                double[] newVelocity = new double[Dim]; // step 1
                double[] newPosition = new double[Dim]; // step 2
                double newError; // step 3

                Shuffle(sequence); // move particles in random sequence

                for (int pi = 0; pi < swarm.Length; ++pi) // each Particle (index)
                {
                    int i = sequence[pi];
                    Particle currP = swarm[i]; // for coding convenience

                    // 1. compute new velocity
                    for (int j = 0; j < currP.velocity.Length; ++j) // each x value of the velocity
                    {
                        r1 = rnd.NextDouble();
                        r2 = rnd.NextDouble();

                        // velocity depends on old velocity, best position of parrticle, and 
                        // best position of any particle
                        newVelocity[j] = (w * currP.velocity[j]) +
                          (c1 * r1 * (currP.bestPosition[j] - currP.position[j])) +
                          (c2 * r2 * (bestGlobalPosition[j] - currP.position[j]));

                        if (newVelocity[j] < minV)
                            newVelocity[j] = minV;
                        else if (newVelocity[j] > maxV)
                            newVelocity[j] = maxV;     // crude way to keep velocity in range
                    }

                    newVelocity.CopyTo(currP.velocity, 0);

                    // 2. use new velocity to compute new position
                    for (int j = 0; j < currP.position.Length; ++j)
                    {
                        newPosition[j] = currP.position[j] + newVelocity[j];  // compute new position
                        if (newPosition[j] < minX)
                            newPosition[j] = minX;
                        else if (newPosition[j] > maxX)
                            newPosition[j] = maxX;
                    }

                    newPosition.CopyTo(currP.position, 0);

                    // 3. use new position to compute new error
                    // consider cross-entropy error instead of MSE
                    newError = MeanSquaredError(trainData, newPosition); // makes next check a bit cleaner
                    currP.error = newError;

                    if (newError < currP.smallestError) // new particle best?
                    {
                        newPosition.CopyTo(currP.bestPosition, 0);
                        currP.smallestError = newError;
                    }

                    if (newError < smallesttGlobalError) // new global best?
                    {
                        newPosition.CopyTo(bestGlobalPosition, 0);
                        smallesttGlobalError = newError;
                    }

                    // consider using weight decay, particle death here

                } // each Particle

                ++iteration;

            } // while (main PSO processing loop)

            //Console.WriteLine("\n\nFinal training MSE = " + smallesttGlobalError.ToString("F4") + "\n\n");

            // copy best weights found into RBF network, and also return them
            this.SetWeights(bestGlobalPosition);
            double[] returnResult = new double[(numHidden * numOutput) + numOutput];
            Array.Copy(bestGlobalPosition, returnResult, bestGlobalPosition.Length);

            Console.WriteLine("The best weights and bias values found are:\n");
            Helpers.ShowVector(bestGlobalPosition, 3, 10, true);
            return returnResult;
        } // DoWeights

        private static void Shuffle(int[] sequence)
        {
            // helper for DoWeights to process particles in random order
            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = rnd.Next(i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        }

        public double[] Train(double[][] trainData, int maxIterations)
        {
            Console.WriteLine("\n1. Computing " + numHidden + " centroids");
            DoCentroids(trainData); // find representative data, store their x-values into this.centroids

            Console.WriteLine("\n2. Computing a common width for each hidden node");
            DoWidths(this.centroids); // measure of how far apart centroids are

            int numWts = (numHidden * numOutput) + numOutput;
            Console.WriteLine("\n3. Determining " + numWts + " weights and bias values using PSO algorithm");
            double[] bestWeights =
              DoWeights(trainData, maxIterations); // use PSO to find weights that best (lowest MSE) weights and biases

            return bestWeights;
        } // Train

        // -- The Euclidean Distance function is used by RBF ComputeOutputs and also DoWidths ----

        private static double EuclideanDist(double[] v1, double[] v2, int numTerms)
        {
            // Euclidean distance between two vectors, first numTerms only
            // helper for computing RBF outputs and computing hidden node widths
            if (v1.Length != v2.Length)
                throw new Exception("Vector lengths not equal in EuclideanDist()");
            double sum = 0.0;
            for (int i = 0; i < numTerms; ++i)
            {
                double delta = (v1[i] - v2[i]) * (v1[i] - v2[i]);
                sum += delta;
            }
            return Math.Sqrt(sum);
        }


    } // class RadialNetwork



}

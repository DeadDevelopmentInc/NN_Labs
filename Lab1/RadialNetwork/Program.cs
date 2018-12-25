using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RadialNetwork
{
    class Program
    {
        static int num_of_classes = 4;
        static int elemnts_file = 25;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new Exception("ERROR: missing xml file. Please choose xml confoguration file");
                }
                Console.WriteLine("Start reading xml confoguration file");
                string trainFile = args[0];
                XElement xElement = XElement.Load(trainFile);
                var elementsInXElement = xElement.Elements();
                if (elementsInXElement.Count() != 2)
                {
                    throw new Exception("ERROR: file does not match the pattern. Please choose valid xml confoguration file or check this file");
                }

                string dataPath = elementsInXElement.First().FirstAttribute.Value.ToString();
                string predictPath = elementsInXElement.Last().FirstAttribute.Value.ToString();

                Console.WriteLine("Start generating dataset");
                var allData = UploadFiles(dataPath);
                Console.WriteLine("Finish generating dataset");


                bool fl = true;

                Console.WriteLine("\nSplitting data into 80%-20% train and test sets");
                double[][] trainData = null;
                double[][] testData = null;
                int seed = 5;
                GetTrainTest(allData, seed, out trainData, out testData);

                Console.WriteLine("\nTraining data: \n");
                Helpers.ShowMatrix(trainData, trainData.Length, 3, true, false);
                Console.WriteLine("\nTest data:\n");
                Helpers.ShowMatrix(testData, testData.Length, 3, true, false);

                Console.WriteLine("\nCreating a 25-5-4 radial basis function network");
                int numInput = 25;
                int numHidden = 5;
                int numOutput = 4;
                RadialNetwork RBFN = new RadialNetwork(numInput, numHidden, numOutput);

                Console.WriteLine("\nBeginning RBF training\n");
                int maxIterations = 100;
                double[] bestWeights = RBFN.Train(trainData, maxIterations);

                Console.WriteLine("\nEvaluating result RBF classification accuracy on the test data");
                RBFN.SetWeights(bestWeights);

                double acc = RBFN.Accuracy(testData);
                Console.WriteLine("Classification accuracy = " + acc.ToString("F4"));

                while (fl)
                {
                    Console.WriteLine("Write path to your image:\n");
                    string data = Console.ReadLine();
                    var item = Helpers.GetBitmap(data);
                    if (item != null)
                    {
                        var res = RBFN.Predict(item.ToArray());
                        switch (res)
                        {
                            case 0:
                                Console.WriteLine("Top Arrow");
                                break;
                            case 1:
                                Console.WriteLine("Left Arrow");
                                break;
                            case 2:
                                Console.WriteLine("Rigth Arrow");
                                break;
                            case 3:
                                Console.WriteLine("Bot Arrow");
                                break;
                        }
                    }
                    else { Console.WriteLine("Incorrect image, try again with other image"); }

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }

        static void GetTrainTest(double[][] allData, int seed, out double[][] trainData, out double[][] testData)
        {
            // 80-20 hold-out validation
            int[] allIndices = new int[allData.Length];
            for (int i = 0; i < allIndices.Length; ++i)
                allIndices[i] = i;

            Random rnd = new Random(seed);
            for (int i = 0; i < allIndices.Length; ++i) // shuffle indices
            {
                int r = rnd.Next(i, allIndices.Length);
                int tmp = allIndices[r];
                allIndices[r] = allIndices[i];
                allIndices[i] = tmp;
            }

            int numTrain = (int)(0.80 * allData.Length);
            int numTest = allData.Length - numTrain;

            trainData = new double[numTrain][];
            testData = new double[numTest][];

            int j = 0;
            for (int i = 0; i < numTrain; ++i)
                trainData[i] = allData[allIndices[j++]];
            for (int i = 0; i < numTest; ++i)
                testData[i] = allData[allIndices[j++]];

        } // GetTrainTest

        static double[][] UploadFiles(string dataPath)
        {
            List<double[]> dataset = new List<double[]>();
            var directories = Directory.GetDirectories(dataPath);
            if(directories.Count() != num_of_classes)
            {
                throw new Exception($"ERROR: missing any folders with training data. Check directories in {dataPath}" );
            }

            for(int i = 0; i < num_of_classes; i++)
            {
                var files = Directory.GetFiles(directories[i]);
                if(files.Count() == 0)
                {
                    throw new Exception($"ERROR: missing any files in folder. Check directory {directories[i]}");
                }

                foreach(var file in files)
                {
                    var source = Helpers.GetBitmap(file);
                    if(source.Count() != 25)
                    {
                        throw new Exception($"ERROR: the file does not match the format. Check file {file}");
                    }
                    Array.Resize(ref source, 29);
                    switch(i)
                    {
                        case 0: source[25] = 1; break;
                        case 1: source[26] = 1; break;
                        case 2: source[27] = 1; break;
                        case 3: source[28] = 1; break;
                    }

                    dataset.Add(source);
                }

            }

            return dataset.ToArray();
        }
    }


    public static class XML
    {
        public static double[] GetDouble(this XElement element)
        {
            double[] vs = null;
            var data = Regex.Split(element.FirstAttribute.Value.ToString(), ", ");
            vs = new double[data.Count()];
            int i = 0;
            foreach (var t in data)
            {
                vs[i] = Convert.ToDouble(t);
                i++;
            }
            return vs;
        }

        public static string GetString(this double[] source)
        {
            return "";
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBNF
{
    public interface mathFunction
    {
        double function(double input);
        double derivative(double input);
    }

    public class Neuron
    {
        private List<double> weights;
        private double bias;
        private List<double> input;
        private double net;
        private double output;
        private mathFunction activationFunction;
        private double signalError;

        public List<double> Weights
        {
            get { return this.weights; }
        }
        public double Bias
        {
            get { return this.bias; }
        }
        public List<double> Input
        {
            get { return this.input; }
        }
        public double Net
        {
            get { return this.net; }
        }
        public double Output
        {
            get { return this.output; }
        }
        public double SignalError
        {
            set { this.signalError = value; }
            get { return this.signalError; }
        }
        public mathFunction ActivationFunction
        {
            get { return this.activationFunction; }
        }

        /*Constructors*/
        public Neuron(int numOfinput, mathFunction activationFunction, bool LMS)
        {
            this.activationFunction = activationFunction;
            if (!LMS)
                this.init(numOfinput);
            else
                this.initLMS(numOfinput);
        }
        public Neuron(List<double> weights, mathFunction activationFunction)
        {
            this.activationFunction = activationFunction;
            this.init(weights.Count);

            this.weights = weights;
        }
        public Neuron(List<double> weights, double bias, mathFunction activationFunction)
        {
            this.activationFunction = activationFunction;
            this.init(weights.Count);
            this.weights = weights;
            this.bias = bias;
        }

        private void init(int numOfinput)
        {
            this.weights = new List<double>();
            this.input = new List<double>();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < numOfinput; ++i)
            {
                this.weights.Add(0);
                this.input.Add(0);
            }
            this.bias = 0;
            this.output = 0;
            this.signalError = 0;
        }


        private void initLMS(int numOfinput)
        {
            this.weights = new List<double>();
            this.input = new List<double>();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < numOfinput; ++i)
            {
                this.weights.Add(rnd.Next(0, 256));
                this.input.Add(0);
            }
            this.bias = 0;
            this.output = 0;
            this.signalError = 0;
        }
        private double linearCalculation()
        {
            int size = this.weights.Count;
            double result = 0;

            for (int i = 0; i < size; ++i)
            {
                result += (this.input[i] * this.weights[i]);
            }
            //// Adding Bias
            result += bias;

            return result;
        }
        public double feedforward(List<double> input)
        {
            this.input = input;

            this.net = this.linearCalculation();
            this.output = this.activationFunction.function(this.net);

            return this.output;
        }
        public void update(List<double> weights, double bias)
        {
            this.weights = weights;
            this.bias = bias;
        }
    }
}

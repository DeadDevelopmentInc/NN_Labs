using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBNF
{
    public class Particle
    {
        public double[] position; // equivalent to x-Values and/or solution
        public double error; // error so smaller is better
        public double[] velocity;

        public double[] bestPosition; // best position found so far by this Particle
        public double smallestError;

        public Particle(double[] position, double error, double[] velocity, double[] bestPosition, double smallestError)
        {
            this.position = new double[position.Length];
            position.CopyTo(this.position, 0);
            this.error = error;
            this.velocity = new double[velocity.Length];
            velocity.CopyTo(this.velocity, 0);
            this.bestPosition = new double[bestPosition.Length];
            bestPosition.CopyTo(this.bestPosition, 0);
            this.smallestError = smallestError;
        }

        //public override string ToString()
        //{
        //  string s = "";
        //  s += "==========================\n";
        //  s += "Position: ";
        //  for (int i = 0; i < this.position.Length; ++i)
        //    s += this.position[i].ToString("F2") + " ";
        //  s += "\n";
        //  s += "Error = " + this.error.ToString("F4") + "\n";
        //  s += "Velocity: ";
        //  for (int i = 0; i < this.velocity.Length; ++i)
        //    s += this.velocity[i].ToString("F2") + " ";
        //  s += "\n";
        //  s += "Best Position: ";
        //  for (int i = 0; i < this.bestPosition.Length; ++i)
        //    s += this.bestPosition[i].ToString("F2") + " ";
        //  s += "\n";
        //  s += "Smallest Error = " + this.smallestError.ToString("F4") + "\n";
        //  s += "==========================\n";
        //  return s;
        //}

    } // class Particle
}

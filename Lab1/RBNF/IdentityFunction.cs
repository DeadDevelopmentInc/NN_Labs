namespace RBNF
{
    public class IdentityFunction : mathFunction
    {
        public IdentityFunction()
        {
        }

        /// <summary>
        /// Compute the sigmoid of the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>double</returns>
		public double function(double input)
        {
            return input;
        }

        /// <summary>
        /// Compute the derivative of the Sigmoid value.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>double</returns>
		public double derivative(double input)
        {
            return 1;
        }
    }
}
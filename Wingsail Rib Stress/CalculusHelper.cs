using System.Linq;
using System.Text;

namespace Wingsail_Rib_Stress
{
    public class PolynomialCalculus
    {
        List<double> coefficientList = new List<double>(); //the only field of this class

        // The following two constructors are used for unit testing.
        public PolynomialCalculus() //Needed for unit testing and for Main(). Do not remove or modify.
        {
            // Default constructor has an empty body
        }
        public PolynomialCalculus(string testInput) //Needed for unit testing. Do not remove or modify.
        {
            string[] coefficients = testInput.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in coefficients)
            {
                coefficientList.Add(Convert.ToDouble(item));
            }
        }

        /// <summary>
        /// Checks if the passed polynomial string is valid.
        /// The acceptable format of the coefficient string is a series of 
        /// numbers (one for each coefficient) separated by spaces. 
        /// Any number of extra spaces is allowed.
        /// </summary>
        /// <example>
        /// Examples of valid strings: 
        ///       "1 2 3", or " 2   3.5 0  ", or "-2 -3.547 0 0", or "0 .1 -1"
        /// Examples of invalid strings: 
        ///       "3 . 5", or "2x^2+1", or "a b c", or "3 - 5", or "1/2 2", or ""
        /// </example>
        /// <param name="polynomial">
        /// A string containing the coefficient of a polynomial. The first value is the
        /// highest order, and all coefficients exist (even 0's).
        /// </param> 
        /// <returns>True if a valid polynomial, false otherwise.</returns>
        public bool IsValidPolynomial(string polynomial)
        {
            string[] coeffeicient = polynomial.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // splits the strings by spaces and cleans it 

            double output;
            foreach (string coeff in coeffeicient)
            {
                if (!Double.TryParse(coeff, out output))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Evaluates this polynomial at the x passed to the method.
        /// </summary>
        /// <param name="x">The x at which we are evaluating the polynomial.</param>
        /// <returns>The result of the polynomial evaluation.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the coefficientList field is empty. 
        /// Exception message used: "No polynomial is set."
        /// </exception>
        public double EvaluatePolynomial(double x)
        {
            if (coefficientList.Count == 0)
            {
                throw new InvalidOperationException("No polynomial is set.");
            }

            double result = 0;
            int degree = coefficientList.Count - 1;
            double tempX;
            //loop through each coefficient entry and add its contribution
            foreach (double coefficient in coefficientList)
            {
                tempX = 1;
                //calculate x ^ degree
                for (int i = 0; i < degree; i++)
                    tempX *= x;

                result += coefficient * tempX; //add this term's contribution to the result
                degree--; //decrement degree for the next coefficient entry
            }

            return result;
        }

        /// <summary>
        /// Evaluates the 1st derivative of this polynomial at x (passed to the method).
        /// The method uses the exact numerical technique, since it is easy to obtain the 
        /// derivative of a polynomial.
        /// </summary>
        /// <param name="x">The x at which we are evaluating the polynomial derivative.</param>
        /// <returns>The result of the polynomial derivative evaluation.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the coefficientList field is empty.
        /// Exception message used: "No polynomial is set."
        /// </exception>
        public double EvaluatePolynomialDerivative(double x)
        {
            //exception
            if (coefficientList.Count == 0)
            {
                throw new InvalidOperationException("The list is empty.");
            }

            //finds the derivative of each term in coefficientList
            //add those to result.
            //The output is the sum of derivatives
            double result = 0;
            int degree = coefficientList.Count - 1;
            double tempX;
            //loop through each coefficient entry and add its contribution
            foreach (double coefficient in coefficientList)
            {
                tempX = 1;
                //calculate x ^ (degree - 1),
                //if the degree of the term is 0 (constant term), then the derivative contribution will just be zero, so it's ok that this loop breaks when degree = 0
                for (int i = 0; i < degree - 1; i++)
                    tempX *= x;

                result += coefficient * degree * tempX; //add this term's contribution to the result
                degree--; //decrement degree for the next coefficient entry
            }

            return result;
        }

        /// <summary>
        /// Evaluates the definite integral of this polynomial from a to b.
        /// The method uses the exact numerical technique, since it is easy to obtain the 
        /// indefinite integral of a polynomial.
        /// </summary>
        /// <param name="a">The lower limit of the integral.</param>
        /// <param name="b">The upper limit of the integral.</param>
        /// <returns>The result of the integral evaluation.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the coefficientList field is empty.
        /// Exception message used: "No polynomial is set."
        /// </exception>
        public double EvaluatePolynomialIntegral(double a, double b)
        {
            //exceptions
            if (Double.IsNaN(a) || Double.IsNaN(b))
            {
                throw new InvalidOperationException("No polynomial is set.");
            }

            //uses the EvaluateIntegralBound helper method to calculate the integral.
            return EvaluateIntegralBound(b) - EvaluateIntegralBound(a);
        }

        /// <summary>
        /// multiplies the polynomial by a constant
        /// </summary>
        /// <param name="c">the constant to multiply by</param>
        public void MultiplyByConstant(double c)
        {
            for (int i = 0; i < coefficientList.Count; i++)
                coefficientList[i] *= c;
        }

        public void AddPolynomial(PolynomialCalculus add)
        {
            if (add.coefficientList.Count <= coefficientList.Count)
            {
                for (int i = 0; i < add.coefficientList.Count; i++)
                {
                    coefficientList[coefficientList.Count - i - 1] += add.coefficientList[add.coefficientList.Count - i - 1];
                }
            }
            else
            {
                for (int i = 0; i < add.coefficientList.Count; i++)
                {
                    if (coefficientList.Count - i - 1 >= 0)
                        coefficientList[coefficientList.Count - i - 1] += add.coefficientList[add.coefficientList.Count - i - 1];
                    else
                        coefficientList.Insert(0, add.coefficientList[add.coefficientList.Count - i - 1]);
                }
            }
        }

        //--------------------------------------------------------------------------//
        //You may add helper methods below here. Follow the specs and document well.//
        //--------------------------------------------------------------------------//

        /// <summary>
        /// Helper method to give the integral of one bound of an integral.
        /// </summary>
        /// <param name="input"> the "x" value in the integral, a valid double </param>
        /// <returns> The definite integral between that number and 0.  </returns>
        double EvaluateIntegralBound(double input)
        {
            //calculates the indefinite integral at input assuming integration constant C = 0
            double result = 0;
            int degree = coefficientList.Count - 1;
            double tempX;
            //loop through each coefficient entry and add its contribution
            foreach (double coefficient in coefficientList)
            {
                tempX = 1;
                //calculate input ^ (degree + 1)
                for (int i = 0; i < degree + 1; i++)
                    tempX *= input;

                result += coefficient / (degree + 1) * tempX; //add this term's contribution to the result
                degree--; //decrement degree for the next coefficient entry
            }

            return result;
        }

        //------------------------//
        //IClonable implementation//
        //------------------------//

        public PolynomialCalculus Clone()
        {
            StringBuilder sb = new StringBuilder();
            foreach (double coefficient in coefficientList)
                sb.Append($"{coefficient} ");
            return new PolynomialCalculus(sb.ToString());
        }

    }
}
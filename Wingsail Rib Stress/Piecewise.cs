using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wingsail_Rib_Stress
{
    public class PiecewisePolynomial
    {
        List<PolynomialCalculus> functions = new List<PolynomialCalculus>();
        List<(double, double)> domains = new List<(double, double)>();

        static double scaleFactor = 1.33;

        public PiecewisePolynomial()
        {
            //empty default constructor
        }

        /// <summary>
        /// adds a new polynomial element to the function
        /// </summary>
        /// <param name="function">the polynomial being added</param>
        /// <param name="a">the start of the new domain</param>
        /// <param name="b">the end of the new domain</param>
        public void AddFunction(PolynomialCalculus function, double a, double b)
        {
            functions.Add(function);
            domains.Add((a, b));
        }

        /// <summary>
        /// Evaluates the piecewise function at a given x
        /// </summary>
        /// <param name="x">the point to evaluate the piecewise function at</param>
        /// <returns>the value of the piecewise function at x</returns>
        public double EvaluateFunction(double x)
        {
            double result = Double.NaN;

            for (int i = 0; i < domains.Count; i++)
            {
                if (domains[i].Item1 <= x && x <= domains[i].Item2)
                {
                    result = functions[i].EvaluatePolynomial(x);
                    break;
                }
            }

            return result * scaleFactor;
        }

        /// <summary>
        /// Multiplies each polynomial element of the function by a constant
        /// </summary>
        /// <param name="c">the constant to multiply by</param>
        public void MultiplyByConstant(double c)
        {
            for (int i = 0; i < functions.Count; i++)
            {
                functions[i].MultiplyByConstant(c);
            }

            return;
        }

        /// <summary>
        /// Evaluates the definite integral of the piecewise function
        /// </summary>
        /// <param name="a">the start of the domain of integration</param>
        /// <param name="b">the end of the domain of integration</param>
        /// <returns>the value of the definite integral</returns>
        public double EvaluateFunctionIntegral(double a, double b)
        {
            double result = Double.NaN;
            int startDomain = 0;
            int endDomain = domains.Count - 1;

            while (!(domains[startDomain].Item1 <= a && a <= domains[startDomain].Item2))
                startDomain++;
            while (!(domains[endDomain].Item1 <= b && b <= domains[endDomain].Item2))
                endDomain--;

            if (startDomain == endDomain)
            {
                result = functions[startDomain].EvaluatePolynomialIntegral(a, b);
            }
            else
            {
                result = functions[startDomain].EvaluatePolynomialIntegral(a, domains[startDomain].Item2);

                for (int i = startDomain + 1; i < endDomain; i++)
                {
                    result += functions[i].EvaluatePolynomialIntegral(domains[i].Item1, domains[i].Item2);
                }

                result += functions[endDomain].EvaluatePolynomialIntegral(domains[endDomain].Item1, b);
            }

            return result * scaleFactor * scaleFactor;
        }

        /// <summary>
        /// Creates a clone of this piecewise function
        /// </summary>
        /// <returns>a new PiecewisePolynomial object which is a clone of this object</returns>
        public PiecewisePolynomial Clone()
        {
            PiecewisePolynomial clone = new();

            clone.functions = new List<PolynomialCalculus>(functions);
            clone.domains = new List<(double, double)>(domains);
            
            return clone;
        }
    }
}

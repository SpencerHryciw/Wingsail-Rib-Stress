using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wingsail_Rib_Stress
{
    static class StressCaclulator
    {
        //---------------//
        //Key assumptions//
        //---------------//

        //the mass of the wingsail will be ignored
        //the only aerodynamic forces acting on the wingsail will be drag
        //the trim tab torque will be ignored
        //the loading conditions will be 40 knots of wind at 0 degrees or 90 degrees angle of attack
        //at 0 degrees we will use the NACA 0018 drag numbers
        //at 90 degrees we will approximate the airfoil with a rectangular cross section

        static double windSpeed = 20.5778; //m/s
        static double airPressure = 101325; //Pa
        static double airDensity = 1.225; //kg/m^3

        //the wing is ____ m tall
        static double wingHeight;

        //the NACA 0018 airfoil gives us max width = 0.18 times chord length
        static double airfoilWidthCoefficient = 0.18;

        //the drag coefficient of the NACA 0018 airfoil at 0 degrees is 0.04 in the worst case
        static double dragCoefficient0Deg = 0.04;

        //the leading edge of the wing follows the equation __________
        static PolynomialCalculus leadingEdge = new("1 1 1"); //m
        //the trailing edge of the wing follows the equation ___________
        static PolynomialCalculus trailingEdge = new("-1 1 1"); //m

        //the locations of the ribs where 0 is the bottom of the wing, and 1 is the top
        static double[] ribLocations = new double[] { 0, 1.0 / 6.0, 1.0 / 3.0, 0.5, 2.0 / 3.0, 5.0 / 6.0, 1 };

        public static void Main()
        {
            //initialize the chord length
            PolynomialCalculus chordLength = leadingEdge.Clone();
            chordLength.AddPolynomial(trailingEdge);

            //--------------------------------------//
            //0 degrees angle of attack calculations//
            //--------------------------------------//

            //im so lost in the sauce

            //---------------------------------------//
            //90 degrees angle of attack calculations//
            //---------------------------------------//

            //dynamic pressure = 1/2 * rho * V^2
            double dynamicPressure = 0.5 * airDensity * windSpeed * windSpeed;

            //F_in is inward force, F_shear is shear along the mast axis, L is chord length, P is pressure, c is the NACA width coefficicent
            //dF_in = P * L(x) * dx
            //dF_shear = P * c/2 * dL/dx * L(x) * dx

            double[] inwardForce90Deg = new double[ribLocations.Length];
            double[] shearForce90Deg = new double[ribLocations.Length];

            for (int i = 0; i < ribLocations.Length; i++)
            {
                if (i == 0)
                {
                    //a = 0
                    //b = wingHeight * 0.5 * ribLocations[i + 1]

                    inwardForce90Deg[i] = dynamicPressure * chordLength.EvaluatePolynomialIntegral(0, wingHeight * 0.5 * ribLocations[i + 1]);
                    shearForce90Deg[i] = airfoilWidthCoefficient * dynamicPressure * 0.5 * ((chordLength.EvaluatePolynomial(wingHeight * 0.5 * ribLocations[i + 1]) * chordLength.EvaluatePolynomial(wingHeight * 0.5 * ribLocations[i + 1])) - (chordLength.EvaluatePolynomial(0) * chordLength.EvaluatePolynomial(0)));
                }
                else if (i == ribLocations.Length - 1)
                {
                    //a = wingHeight * 0.5 * (1 + ribLocations[i - 1])
                    //b = wingHeight

                    inwardForce90Deg[i] = dynamicPressure * chordLength.EvaluatePolynomialIntegral(wingHeight * 0.5 * (1 + ribLocations[i - 1]), wingHeight);
                    shearForce90Deg[i] = airfoilWidthCoefficient * dynamicPressure * 0.5 * ((chordLength.EvaluatePolynomial(wingHeight) * chordLength.EvaluatePolynomial(wingHeight)) - (chordLength.EvaluatePolynomial(wingHeight * 0.5 * (1 + ribLocations[i - 1])) * chordLength.EvaluatePolynomial(wingHeight * 0.5 * (1 + ribLocations[i - 1]))));
                }
                else
                {
                    //a = wingHeight * 0.5 * (ribLocations[i] + ribLocations[i - 1])
                    //b = wingHeight * 0.5 * (ribLocations[i] + ribLocations[i + 1])

                    inwardForce90Deg[i] = dynamicPressure * chordLength.EvaluatePolynomialIntegral(wingHeight * 0.5 * (ribLocations[i] + ribLocations[i - 1]), wingHeight * 0.5 * (ribLocations[i] + ribLocations[i + 1]));
                    shearForce90Deg[i] = airfoilWidthCoefficient * dynamicPressure * 0.5 * ((chordLength.EvaluatePolynomial(wingHeight * 0.5 * (ribLocations[i] + ribLocations[i + 1])) * chordLength.EvaluatePolynomial(wingHeight * 0.5 * (ribLocations[i] + ribLocations[i + 1]))) - (chordLength.EvaluatePolynomial(wingHeight * 0.5 * (ribLocations[i] + ribLocations[i - 1])) * chordLength.EvaluatePolynomial(wingHeight * 0.5 * (ribLocations[i] + ribLocations[i - 1]))));
                }
            }
        }
    }
}

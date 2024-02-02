using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wingsail_Rib_Stress
{
    static class StressCaclulator
    {
        //---------------------//
        //   Key assumptions   //
        //---------------------//

        //the mass of the wingsail will be ignored
        //the only aerodynamic forces acting on the wingsail will be dynamic pressure / drag
        //the trim tab torque will be ignored
        //the loading conditions will be 40 knots of wind at 0 degrees or 90 degrees angle of attack
        //at 0 degrees we will use the NACA 0018 drag numbers
        //at 90 degrees we will approximate the airfoil with a rectangular cross section and use dynamic pressure

        //----------------------//
        //   input parameters   //
        //vvvvvvvvvvvvvvvvvvvvvv//

        //---------//
        //   air   //
        //---------//

        static double windSpeed = 20.5778; //m/s
        //static double airPressure = 101325; //Pa
        static double airDensity = 1.225; //kg/m^3

        //----------//
        //   wing   //
        //----------//

        //the wing is 2.762 m tall
        static double wingHeight = 2.762;

        //the NACA 0018 airfoil gives us max width = 0.18 times chord length
        static double airfoilWidthCoefficient = 0.18;

        //the drag coefficient of the NACA 0018 airfoil at 0 degrees is 0.04 in the worst case
        //static double dragCoefficient0Deg = 0.04;

        static PiecewisePolynomial leadingEdge = new(); //m
        static PiecewisePolynomial trailingEdge = new(); //m

        //------------//
        //   spline   //
        //------------//

        static double scaleFactor = 1.33;

        static double x1 = 0;
        static double x2 = 0.305;
        static double x3 = 1.686;
        static double x4 = 2.762;

        static double y1 = 0.31;
        static double y2 = 0.554;
        static double y3 = 0.554;
        static double y4 = 0.31;

        static double m1 = 0.8;
        static double m2 = 0;
        static double m3 = -61.0 / 289.0;

        static double a1 = 0.809011682971;
        static double a2 = 0.340549841562;
        static double a3 = -0.0431185788382;

        static double b1 = -1.61802336594;
        static double b2 = -0.145078184269;
        static double b3 = 0.0215592894191;

        //--------------//
        //   !!ribs!!   //
        //--------------//

        //the locations of the ribs where 0 is the bottom of the wing, and 1 is the top
        static double[] ribLocations = new double[] { 0, 1.0 / 6.0, 1.0 / 3.0, 0.5, 2.0 / 3.0, 5.0 / 6.0, 1 };

        public static void Main()
        {
            //-----------------------------//
            //   initializing the spline   //
            //-----------------------------//

            //the first linear interpolation
            PolynomialCalculus f1 = new PolynomialCalculus($"{m1}, {y1 - (m1 * x1)}");
            //adding the cubic correction
            f1.AddPolynomial(new PolynomialCalculus($"{a1 + b1}, {(-2 * a1 * x2) - (a1 * x1) - (2 * b1 * x1) - (b1 * x2)}, {(a1 * x2 * x2) + (2 * a1 * x1 * x2) + (b1 * x1 * x1) + (2 * b1 * x1 * x2)}, {-(a1 * x1 * x2 * x2) - (b1 * x1 * x1 * x2)}"));
            //adding the element to the piecewise equation
            trailingEdge.AddFunction(f1, x1, x2);

            //the first linear interpolation
            PolynomialCalculus f2 = new PolynomialCalculus($"{m2}, {y2 - (m2 * x2)}");
            //adding the cubic correction
            f2.AddPolynomial(new PolynomialCalculus($"{a2 + b2}, {-(2 * a2 * x3) - (a2 * x2) - (2 * b2 * x2) - (b2 * x3)}, {(a2 * x3 * x3) + (2 * a2 * x2 * x3) + (b2 * x2 * x2) + (2 * b2 * x2 * x3)}, {-(a2 * x2 * x3 * x3) - (b2 * x2 * x2 * x3)}"));
            //adding the element to the piecewise equation
            trailingEdge.AddFunction(f2, x2, x3);

            //the first linear interpolation
            PolynomialCalculus f3 = new PolynomialCalculus($"{m3}, {y3 - (m3 * x3)}");
            //adding the cubic correction
            f3.AddPolynomial(new PolynomialCalculus($"{a3 + b3}, {-(2 * a3 * x4) - (a3 * x3) - (2 * b3 * x3) - (b3 * x4)}, {(a3 * x4 * x4) + (2 * a3 * x3 * x4) + (b3 * x3 * x3) + (2 * b3 * x3 * x4)}, {-(a3 * x3 * x4 * x4) - (b3 * x3 * x3 * x4)}"));
            //adding the element to the piecewise equation
            trailingEdge.AddFunction(f3, x3, x4);

            //initialize leadingEdge
            //the leading edge of the wing is -1/3 times the leading edge
            //leadingEdge = trailingEdge.Clone();
            //leadingEdge.MultiplyByConstant(-1.0 / 3.0);

            //initialize the chord length
            //chord length = |leadingEdge - trailingEdge| = |-1/3 * trailingEdge - trailingEdge| = |-4/3 * trailingEdge| = 4/3 * trailingEdge
            PiecewisePolynomial chordLength = trailingEdge.Clone();
            chordLength.MultiplyByConstant(4.0 / 3.0);

            //--------------------------------------------//
            //   0 degrees angle of attack calculations   //
            //--------------------------------------------//

            //im so lost in the sauce

            //---------------------------------------------//
            //   90 degrees angle of attack calculations   //
            //---------------------------------------------//

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

                    inwardForce90Deg[i] = dynamicPressure * chordLength.EvaluateFunctionIntegral(0, wingHeight * 0.5 * ribLocations[i + 1]);
                    shearForce90Deg[i] = airfoilWidthCoefficient * dynamicPressure * 0.25 * ((chordLength.EvaluateFunction(wingHeight * 0.5 * ribLocations[i + 1]) * chordLength.EvaluateFunction(wingHeight * 0.5 * ribLocations[i + 1])) - (chordLength.EvaluateFunction(0) * chordLength.EvaluateFunction(0)));
                }
                else if (i == ribLocations.Length - 1)
                {
                    //a = wingHeight * 0.5 * (1 + ribLocations[i - 1])
                    //b = wingHeight

                    inwardForce90Deg[i] = dynamicPressure * chordLength.EvaluateFunctionIntegral(wingHeight * 0.5 * (1 + ribLocations[i - 1]), wingHeight);
                    shearForce90Deg[i] = airfoilWidthCoefficient * dynamicPressure * 0.25 * ((chordLength.EvaluateFunction(wingHeight) * chordLength.EvaluateFunction(wingHeight)) - (chordLength.EvaluateFunction(wingHeight * 0.5 * (1 + ribLocations[i - 1])) * chordLength.EvaluateFunction(wingHeight * 0.5 * (1 + ribLocations[i - 1]))));
                }
                else
                {
                    //a = wingHeight * 0.5 * (ribLocations[i] + ribLocations[i - 1])
                    //b = wingHeight * 0.5 * (ribLocations[i] + ribLocations[i + 1])

                    inwardForce90Deg[i] = dynamicPressure * chordLength.EvaluateFunctionIntegral(wingHeight * 0.5 * (ribLocations[i] + ribLocations[i - 1]), wingHeight * 0.5 * (ribLocations[i] + ribLocations[i + 1]));
                    shearForce90Deg[i] = airfoilWidthCoefficient * dynamicPressure * 0.25 * ((chordLength.EvaluateFunction(wingHeight * 0.5 * (ribLocations[i] + ribLocations[i + 1])) * chordLength.EvaluateFunction(wingHeight * 0.5 * (ribLocations[i] + ribLocations[i + 1]))) - (chordLength.EvaluateFunction(wingHeight * 0.5 * (ribLocations[i] + ribLocations[i - 1])) * chordLength.EvaluateFunction(wingHeight * 0.5 * (ribLocations[i] + ribLocations[i - 1]))));
                }
            }

            //-------------//
            //   results   //
            //-------------//

            //defining variables for the force sums and friction coefficients
            double shearSum = 0;
            double inwardSum = 0;
            double[] ribFrictionCoefficients = new double[ribLocations.Length];

            //print everything as well as find the sums and friction coefficients
            Console.WriteLine($"rib forces at 90 degrees angle of attack and dynamic pressure of {Math.Round(dynamicPressure, 3)} Pa\n");
            
            for (int i = 0; i < shearForce90Deg.Length; i++)
            {
                shearSum += shearForce90Deg[i];
                inwardSum += inwardForce90Deg[i];
                ribFrictionCoefficients[i] = shearForce90Deg[i] / inwardForce90Deg[i];
                Console.WriteLine($"the rib at {Math.Round(wingHeight * ribLocations[i] * scaleFactor, 3)} m has {Math.Round(inwardForce90Deg[i], 3)} N inward, {Math.Round(shearForce90Deg[i], 3)} N of shear, and a friction coefficient of {Math.Round(ribFrictionCoefficients[i], 3)}");
            }

            Console.WriteLine($"\nthe total forces are {Math.Round(inwardSum, 3)} N inward and {Math.Round(shearSum, 3)} N shear");
            Console.WriteLine($"the expected forces are {Math.Round(2.004 * scaleFactor * scaleFactor * dynamicPressure, 3)} N inward and 0 N shear");

            //-------------------//
            //   average chord   //
            //-------------------//

            Console.WriteLine(chordLength.EvaluateFunctionIntegral(0, wingHeight) / wingHeight);
            Console.WriteLine(wingHeight);
            Console.WriteLine(windSpeed);
            Console.WriteLine(dynamicPressure);
        }
    }
}

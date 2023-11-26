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

        //each cross section will be treated as a rectangle
        //the mass of the wingsail will be ignored
        //the only aerodynamic forces acting on the wingsail will be drag
        //the trim tab torque will be ignored
        //the loading conditions will be 40 knots of wind at 0 degrees or 90 degrees angle of attack
        //at 0 degrees we will use the NACA 0018 drag numbers
        //at 90 degrees we will approximate the airfoil with a smooth plate

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
        static double[] ribLocations = new double[] { 0, 0.5, 1 };

        public static void Main()
        {
            //initialize the chord length
            PolynomialCalculus chordLength = leadingEdge.Clone();
            chordLength.AddPolynomial(trailingEdge);

            //initialize the apparent width
            PolynomialCalculus apparentWidth = chordLength.Clone();
            apparentWidth.MultiplyByConstant(airfoilWidthCoefficient);

            //--------------------------------------//
            //0 degrees angle of attack calculations//
            //--------------------------------------//

            //drag force = Cd * density * 1/2 * V^2 * area -> drag pressure = drag force / area
            double dragPressure = dragCoefficient0Deg * airDensity * 0.5 * windSpeed * windSpeed;

            //get the drag force per length for a given location on the wing
            PolynomialCalculus dragForce = apparentWidth.Clone();
            dragForce.MultiplyByConstant(dragPressure); //N / m



            //---------------------------------------//
            //90 degrees angle of attack calculations//
            //---------------------------------------//

            //dynamic pressure = 1/2 * rho * V^2
            double dynamicPressure = 0.5 * airDensity * windSpeed * windSpeed;

            //y is inward force, x is shear along the mast axis, L is chord length, P is pressure
            //dFy = P * L * dx
            //dFx = dL/dx * dFy

            double[] inwardForce90Deg = new double[ribLocations.Length];
            double[] shearForce90Deg = new double[ribLocations.Length];

            //do the side facing the wind first
            for (int i = 0; i < ribLocations.Length; i++)
            {
                if (i == 0)
                {
                    inwardForce90Deg[0] = dynamicPressure * chordLength.EvaluatePolynomialIntegral(0, wingHeight * (ribLocations[1] + ribLocations[0]) / 2);
                    shearForce90Deg[0] = dynamicPressure * airfoilWidthCoefficient * 0.5 * ((chordLength.EvaluatePolynomial(wingHeight * (ribLocations[1] + ribLocations[0]) / 2) * chordLength.EvaluatePolynomial(wingHeight * (ribLocations[1] + ribLocations[0]) / 2)) - (chordLength.EvaluatePolynomial(0) * chordLength.EvaluatePolynomial(0)));
                }
                else if (i == ribLocations.Length - 1)
                {
                    inwardForce90Deg[i] = dynamicPressure * chordLength.EvaluatePolynomialIntegral(wingHeight * (ribLocations[i] + ribLocations[i - 1]) / 2, wingHeight);
                    shearForce90Deg[i] = dynamicPressure * airfoilWidthCoefficient * 0.5 * ((chordLength.EvaluatePolynomial(wingHeight) * chordLength.EvaluatePolynomial(wingHeight)) - (chordLength.EvaluatePolynomial(wingHeight * (ribLocations[i] + ribLocations[i - 1]) / 2) * chordLength.EvaluatePolynomial(wingHeight * (ribLocations[i] + ribLocations[i - 1]) / 2)));
                }
                else
                {
                    inwardForce90Deg[i] = dynamicPressure * chordLength.EvaluatePolynomialIntegral(wingHeight * (ribLocations[i] + ribLocations[i - 1]) / 2, wingHeight * (ribLocations[i] + ribLocations[i + 1]) / 2);
                    shearForce90Deg[i] = dynamicPressure * airfoilWidthCoefficient * 0.5 * ((chordLength.EvaluatePolynomial(wingHeight * (ribLocations[i] + ribLocations[i + 1]) / 2) * chordLength.EvaluatePolynomial(wingHeight * (ribLocations[i] + ribLocations[i + 1]) / 2)) - (chordLength.EvaluatePolynomial(wingHeight * (ribLocations[i] + ribLocations[i - 1]) / 2) * chordLength.EvaluatePolynomial(wingHeight * (ribLocations[i] + ribLocations[i - 1]) / 2)));
                }
            }

            double totalInwardForce90Deg = dynamicPressure * chordLength.EvaluatePolynomialIntegral(0, wingHeight);
            double totalShearForce90Deg = dynamicPressure * airfoilWidthCoefficient * 0.5 * ((chordLength.EvaluatePolynomial(wingHeight) * chordLength.EvaluatePolynomial(wingHeight)) - (chordLength.EvaluatePolynomial(0) * chordLength.EvaluatePolynomial(0)));

            //do the side facing away from the wind second
            //inwardForce90Deg += airPressure * chordLength.EvaluatePolynomialIntegral(0, wingHeight);
            //shearForce90Deg += airPressure * airfoilWidthCoefficient * 0.5 * ((chordLength.EvaluatePolynomial(wingHeight) * chordLength.EvaluatePolynomial(wingHeight)) - (chordLength.EvaluatePolynomial(0) * chordLength.EvaluatePolynomial(0)));


        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wingsail_Rib_Stress
{
    class StressCaclulator
    {
        //---------------//
        //Key assumptions//
        //---------------//

        //each cross section will be treated as a rectangle
        //the gravitational force on the wingsail will be constant for any number of ribs
        //the only aerodynamic forces acting on the wingsail will be ambient air pressure and "drag" from the air hitting the front or side cross sections

        //the loading conditions will be _____ m/s of wind at 0 degrees or 90 degrees angle of attack
        double windSpeed; //m/s
        double airPressure; //Pa

        //the NACA airfoil gives us width = _____ times chord length
        double airfoilWidthCoefficient;

        //the leading edge of the wing follows the equation __________
        PolynomialCalculus leadingEdge; //meters

        //the trailing edge of the wing follows the equation ___________
        PolynomialCalculus trailingEdge; //meters

        //the wing contains _____ kg of mass
        double wingMass; //kg

        public static void Main()
        {
            
        }
    }
}

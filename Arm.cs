using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using TestMySpline;

// This is a new namespace in .NET 2.0
// that contains the SerialPort class
using System.IO.Ports;

namespace ConsoleApplication3
{
    class Arm
    {
        
        /*
         * the Arm Class has a constructor which sets the 
         * limits for the arm 
         * sets the array which houses the arm 
         * im not sure if it will start the com port,
         * probably will. 
        */
        private int[] servoId;
        private float [] alphaHome;
        private float [] alphaHome2;
        private float [] alphaSleep;
        private float [,] musecLims;
        private float[,] alphaLims;
        private double [] linklen;
        private double mm2In =1/25.4 ; 

        public Arm()
        {
            //constructor 
            //set the values for all the variables. put them all in arrays. 
            servoId = new int [6] {0, 1, 2, 3, 4, 5};
            alphaHome = new float [6] {0, 0, 0, 0, 0, 0.9f};
            alphaHome2 = new float[6] { 0, 38, 30, 88, 0, 0.9f };
            alphaSleep= new float[6] { 0, 38, 30, 88, 0, 0.9f };
            musecLims = new float[3,6]
                {
                    {2490,  600,   1710,  635,   1880,  1000},
                    {1550,  1458,  888,   1510,  1460,  1331},
                    { 620,   2315,  615,   2385,  1040,  1640}
                };

            alphaLims = new float[3, 6]
                {
                    {-90, -90, -90, -90, -45, 0.00f},
                    {0,   0,   0,   0,   0, 1.37f},
                    { 90,  90,  30,  90,  45, 1.8f},
                };

            linklen = new double[5] { 110.6, 130, 130, 95, 30 };
            Console.WriteLine("Initialised");

            

        }



        public void gotoHome(int time, SerialPort port)
        {
            //goes to home position 
            setArm(alphaHome, 4000, port);
        }

        public void gotoSleep(int time, SerialPort port)
        {
            //goes to sleep position 
            setArm(alphaSleep, 4000, port);
        }

        public void setArm(float [] alpha, int time, SerialPort port)
        {
            string result;
            int servo;
            float[] angle = new float[6];
            float[] newMusecAngles;
            //sets all the servos of the arm 

            //checks to see if the angles lie within the limits 
            for (int i = 0; i < 6; i++)
            {
                if (alpha[i] < alphaLims[0, i])
                {
                    alpha[i] = alphaLims[0, i];
                }

                if (alpha[i] > alphaLims[2, i])
                {
                    alpha[i] = alphaLims[2, i];
                }
                

            } 

            //goes through each alpha converts it to musecs  
            //use spline stuff.
            // Create the test points.
            for (int i = 0; i < 6; i++)
            {
                float[] x = new float[] { alphaLims[0, i],alphaLims[1,i], alphaLims[2,i]};
                float[] y = new float[] { musecLims[0, i], musecLims[1, i], musecLims[2, i]};

                float[] xs = new float[1];
                xs[0] = alpha[i];

                CubicSpline spline = new CubicSpline();
                newMusecAngles = spline.FitAndEval(x, y, xs, false);
                angle[i] = newMusecAngles[0];
                //string check = string.Format("{0}", newMusecAngles[0]);
                //Console.WriteLine(check);
                //Console.ReadLine();
            }
            //DEBUG
            //Console.WriteLine("Done conversion");
            //Console.Write("conversion check: ");
            //string check2 = string.Format("{0}", angle);
            //Console.WriteLine(check2);

            //cycles through all of the Angles sending them to the serial port one by one.
            for (int i = 0; i < 6; i++)
            {
                //angle = conversion 
                
                servo = servoId[i];
                result = string.Format("#{0} P{1}  T{2} {3}", servo, angle[i], time, Environment.NewLine);
                port.Write(result);
            }

            //prints a statement when finished
            //Console.WriteLine("Done and dusted");
            //string name = Console.ReadLine();

        }

        public void forwardKin(double[] alpha, int toTip)
        {
            //sets the forward kinematics 
        }

        public void inverseKin(double xw, double yw, double zw)
        {
            /*float [] alpha = new float[6];
            //sets the inverse kinematics 
            float [,] g6 = new float[4,4]
                {
                    {1, 0, 0, 0}, 
                    {0, 1, 0, 0},
                    {0, 0, 1, (float)linklen[4]},
                    {0, 0, 0, 1},

                };
            float gw = //find the inverse of g6 and multiply it by this 
            double r = //squarefoot of xw squared and yw squared
            */
            

        }

        public void setHome(float[] newHome)
        {
            alphaHome = newHome; 
        }

        public float[] getHome()
        {
            return alphaHome; 
        }

        public void setSleep(float [] newSleep)
        {
            alphaSleep = newSleep; 
        }

        public void setAngleTo(int mNum, float angle, SerialPort port)
        {
            alphaHome2[mNum] = angle;
            setArm(alphaHome2, 1000, port);
            

        }

        public float[] getSleep()
        {
            return alphaSleep;
        }

        public void setLims()
        {
        }

    }
}

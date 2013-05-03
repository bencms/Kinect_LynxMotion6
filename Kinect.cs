using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using Microsoft.Kinect;
using System.IO;
using ConsoleApplication3;
using System.IO.Ports;
using TestMySpline;

namespace ConsoleApplication4
{
    class Kinect
    {

        KinectSensor kinectSensor;
        float rightHandX =0;
        float rightHandY;
        float rightHandZ;
        float leftHandX;
        float leftHandZ;
        float kneeRightX;
        float kneeLeftX;
        Joint rightHand;
        double set0 = 0;
        Joint rightShoulder;
        Joint rightElbow;
        Joint leftHand;
        Joint leftShoulder;
        Joint leftElbow;
        Joint hipCenter;
        Joint wristRight;
        double firstangEl = 0;
        double firstLx = 0;
        double firstangAp = 0;
        double firstangWr = 0;
        double firstangEll = 0;
        double firstrhZ = 0;
        double firstKrx = 0;
        double jointangEl = 0;
        double jointangAp = 0;
        double jointangWr = 0;
        double jointangEll = 0;
        double[] brotlims;
        double[] xslidelims;
        double[] finglims;
        double[] kneexlims;
        float[] angle;
        float[] distance;
        public float Zcoord;
        public float Angl;
        public int initflag = 0;
        private double firstB;
        private Arm lynx;
        private SerialPort port;
        
        public  Kinect(Arm arm, SerialPort p)
        {
            angle = new float[1];
            finglims = new double[3] { 0.0, 1.37, 1.8 };
            //kneexlims = new double[3] { -0.07, 0, 0.07 };
            kneexlims = new double[3] { 0.1, 0, 0.3 };
            brotlims =new double[3]{-90, 0, 90};
            xslidelims = new double[3]{-0.5, 0, 0.5};

            //Kinect p = new Kinect();
            port = p; 
            lynx = arm;
            kinectInit();


            //string tests = string.Format("a :{0} , b : {1}", a, b);
           
            Console.WriteLine("Kinect ïnitialised");
            Console.ReadLine();
            
        }

        private Int32 FractionalPart(double n)
        {
            string s = n.ToString("#.#########", System.Globalization.CultureInfo.InvariantCulture);
            return Int32.Parse(s.Substring(s.IndexOf(".") + 1));
        }

        public void kinectInit()
        {
            KinectSensor.KinectSensors.StatusChanged += (object sender, StatusChangedEventArgs e) =>
            {
                if (e.Sensor == kinectSensor)
                {
                    if (e.Status != KinectStatus.Connected)
                    {
                        SetSensor(null);
                    }
                }
                else if ((kinectSensor == null) && (e.Status == KinectStatus.Connected))
                {
                    SetSensor(e.Sensor);
                }
            };

            foreach (var sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    SetSensor(sensor);
                }
            }
        }

        private void SetSensor(KinectSensor newSensor)
        {
            if (kinectSensor != null)
            {
                kinectSensor.Stop();
                Console.WriteLine("inside set sensor stop");
                Console.ReadLine();
            }

            kinectSensor = newSensor;

            if (kinectSensor != null)
            {
                kinectSensor.SkeletonStream.Enable();
                //kinectSensor.SkeletonFrameReady += OnSkeletonFrameReady;
                kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(OnSkeletonFrameReady); // Get Ready for Skeleton Ready Events
                //kinectSensor.SkeletonFrameReady += new EventHandler<AllFramesReadyEventArgs>(OnSkeletonFrameReady);
                kinectSensor.Start();
                Console.Write("started kinect");
                Console.ReadLine();
            }
        }

        
        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            SkeletonFrame skelFrame = e.OpenSkeletonFrame();
            Skeleton [] skeletons = new Skeleton[kinectSensor.SkeletonStream.FrameSkeletonArrayLength];
            Console.WriteLine("Skeletonfraame ready");

            if (skelFrame != null)
            {
                Console.WriteLine("Skeletonfraame ready not nul");
                string test6 = string.Format("xinit {0}  xcurrent{1} angle {2}", firstLx, leftHandX, set0);
                Console.WriteLine(test6);
                skelFrame.CopySkeletonDataTo(skeletons);
                foreach (Skeleton skel in skeletons)
                {

                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        Console.WriteLine("tracked");
                       
                         Getpos(skel);
                      
                         if (initflag < 3)
                         {

                             initflag++;
                         }
                         if (initflag==3)
                        {
                            //set b to the permanent fixture one time 
                             firstB = firstgetb();
                             initflag = 4;
                             firstangAp = jointangAp;
                             firstangEl = jointangEl;
                             firstangWr = jointangWr;
                             firstangEll = jointangEll;
                             firstLx = leftHandX;
                             firstKrx = kneeRightX;
                        }
                         else
                         {  
                             // get the new b 
                             //double set5 = kneeRightX - firstKrx;
                             double set5 = kneeRightX - kneeLeftX;
                             set5 = mapDist(set5);
                             set0 = leftHandX - firstLx;
                             set0 = mapAngle(set0);
                             double set2 = (jointangEl - firstangEl) * 2;
                             double set1 = jointangAp - firstangAp;
                             double set3 = (jointangEll - firstangEll)*1.5;
                             //set1 = -set1;
                             set0 = -set0;
                             //set3 = -set3;
                             //send this to the arm. 
                             string test2 = string.Format("movement motor 2 {0} movement:{1} jointang:{2}",set1, set2, jointangEl);
                             Console.WriteLine(test2);
                             lynx.setAngleTo(1, (float)set1, port);
                             System.Threading.Thread.Sleep(100);
                             lynx.setAngleTo(2, (float)set2, port);
                             System.Threading.Thread.Sleep(100);
                             lynx.setAngleTo(0, (float)set0, port);
                             System.Threading.Thread.Sleep(100);
                             lynx.setAngleTo(3, (float)set3, port);
                             System.Threading.Thread.Sleep(100);
                             lynx.setAngleTo(5, (float)set5, port);

                         }

                    }
                    else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                    {

                        Getpos(skel);
                        if (initflag < 3)
                        {

                            initflag++;
                        }
                         if (initflag==3)
                        {
                            //set b to the permanent fixture one time 
                             firstB = firstgetb();
                             firstangEl = jointangEl;
                             firstangAp = jointangAp;
                             firstangWr = jointangWr;
                             firstangEll = jointangEll;
                             firstLx = leftHandX;
                             firstKrx = kneeRightX;
                             initflag = 3;
                        }
                         else
                         {  
                             // get the new b 
                             //double set5 = kneeRightX - firstKrx;
                             double set5 = kneeRightX - kneeLeftX;
                             set5 = mapDist(set5);
                             set0 = leftHandX - firstLx;
                             set0 = mapAngle(set0);
                             double set2 = (jointangEl - firstangEl)*1.5;
                             double set1 = jointangAp - firstangAp;
                             double set3 = jointangEll -firstangEll;
                             //set1 = -set1;
                             set0 = -set0;
                             //send this to the arm. 
                             string test3 = string.Format("movement motor2 {0} movement 1:{1} jointang:{2}", set2, set1, jointangEl);
                             Console.WriteLine(test3);
                             lynx.setAngleTo(1, (float)set1, port);
                             System.Threading.Thread.Sleep(100);
                             lynx.setAngleTo(2, (float)set2, port);
                             System.Threading.Thread.Sleep(100);
                             lynx.setAngleTo(0, (float)set0, port);
                             System.Threading.Thread.Sleep(100);
                             lynx.setAngleTo(3, (float)set3, port);
                             System.Threading.Thread.Sleep(100);
                             lynx.setAngleTo(5, (float)set5, port);


                         }

                        Console.WriteLine("tracked 2");
                    }
                }
            }

        }

        private void Getpos(Skeleton skel)
        {
            Console.WriteLine("tracked");
            //here's get the joints for each tracked skeleton
            rightHandX = skel.Joints[JointType.HandRight].Position.X;
            rightHandY = skel.Joints[JointType.HandRight].Position.Y;
            rightHandZ = skel.Joints[JointType.HandRight].Position.Z;
            leftHandZ = skel.Joints[JointType.HandLeft].Position.Z;
            leftHandX = skel.Joints[JointType.HandLeft].Position.X;
            rightHand = skel.Joints[JointType.HandRight];
            rightShoulder = skel.Joints[JointType.ShoulderRight];
            leftElbow = skel.Joints[JointType.ElbowLeft];
            leftHand = skel.Joints[JointType.HandLeft];
            leftShoulder = skel.Joints[JointType.ShoulderLeft];
            rightElbow = skel.Joints[JointType.ElbowRight];
            hipCenter = skel.Joints[JointType.HipCenter];
            wristRight = skel.Joints[JointType.WristRight];
            kneeRightX = skel.Joints[JointType.KneeRight].Position.X;
            kneeLeftX = skel.Joints[JointType.KneeLeft].Position.X;

            jointangEl  = AngleBetweenJoints( rightHand, rightElbow,  rightShoulder);
            jointangAp = AngleBetweenJoints(rightElbow, rightShoulder, hipCenter);
            jointangWr = AngleBetweenJoints(rightHand, wristRight, rightShoulder);
            jointangEll = AngleBetweenJoints(leftHand, leftElbow, leftShoulder);


            //jointang = -jointang;

            string test = string.Format("righthand X: {0} Righthand Y:{1} righthand Z: {2} ", rightHandX, rightHandY, rightHandZ);
            Console.WriteLine(test);
            System.Threading.Thread.Sleep(200);
                      
             
        }

        public void SetAngle(float ang)
        {
            Angl = ang;
        }

        public float GetZ()
        {
            if (leftHandZ == 0)
            {
                leftHandZ = 0.00001f;
            }

            Zcoord = leftHandZ;
            return Zcoord;
        }

        private double  firstgetb()
        {
            float a = GetZ();
            double test = FractionalPart(a + 0.11);
            double digit = Math.Floor(Math.Log10(test) + 1);
            double b = FractionalPart(a);
            double dig = Math.Floor(Math.Log10(b) + 1);
            dig = dig - 2;
            //bool flagCheck = true;
            if ((digit - dig >= 2) == true)
            {
                b = 0;
            }
            else
            {
                b = b / (Math.Pow(10, dig));
               double c = FractionalPart(b);
               double d = Math.Floor(Math.Log10(c) + 1);
                b = b - c / (Math.Pow(10, d));
            }

            return b;
        }

         private double  getNewB(double firstB)
        {
           // float a = GetZ();
            //double a = 2.00;
            float a = GetZ();
            double test = FractionalPart(a + 0.11);
            double digit = Math.Floor(Math.Log10(test) + 1);
            double b = FractionalPart(a);
            double dig = Math.Floor(Math.Log10(b) + 1);
            dig = dig - 2;
            //bool flagCheck = true;
            if ((digit - dig >= 2) == true)
            {
                b = 0;
            }
            else
            {
                b = b / (Math.Pow(10, dig));
                double c = FractionalPart(b);
                double d = Math.Floor(Math.Log10(c) + 1);
                b = b - c / (Math.Pow(10, d));
            }


            return b - firstB;
        }

         public static double AngleBetweenJoints(Joint j1, Joint j2, Joint j3)
         {
             double Angulo = 0;
             double shrhX = j1.Position.X - j2.Position.X;
             double shrhY = j1.Position.Y - j2.Position.Y;
             double shrhZ = j1.Position.Z - j2.Position.Z;
             double hsl = vectorNorm(shrhX, shrhY, shrhZ);
             double unrhX = j3.Position.X - j2.Position.X;
             double unrhY = j3.Position.Y - j2.Position.Y;
             double unrhZ = j3.Position.Z - j2.Position.Z;
             double hul = vectorNorm(unrhX, unrhY, unrhZ);
             double mhshu = shrhX * unrhX + shrhY * unrhY + shrhZ * unrhZ;
             double x = mhshu / (hul * hsl);
             if (x != Double.NaN)
             {
                 if (-1 <= x && x <= 1)
                 {
                     double angleRad = Math.Acos(x);
                     Angulo = angleRad * (180.0 / Math.PI);
                 }
                 else
                     Angulo = 0;


             }
             else
                 Angulo = 0;


             return Angulo;

         }


         /// <summary>
         /// Euclidean norm of 3-component Vector
         /// </summary>
         /// <param name="x"></param>
         /// <param name="y"></param>
         /// <param name="z"></param>
         /// <returns></returns>
         private static double vectorNorm(double x, double y, double z)
         {

             return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

         }

         private double mapAngle(double coordinate)
         {

             float[] x = new float[] {(float)xslidelims[0], (float)xslidelims[1], (float)xslidelims[2] };
             float[] y = new float[] { (float)brotlims[0], (float)brotlims[1], (float)brotlims[2] };

             float[] xs = new float[1];
             xs[0] = (float)coordinate;

             CubicSpline spline = new CubicSpline();
             angle= spline.FitAndEval(x, y, xs, false);
             return angle[0];
         }

         private double mapDist(double coordinate)
         {

             float[] x = new float[] { (float)kneexlims[0], (float)kneexlims[1], (float)kneexlims[2] };
             float[] y = new float[] { (float)finglims[0], (float)finglims[1], (float)finglims[2] };

             float[] xs = new float[1];
             xs[0] = (float)coordinate;

             CubicSpline spline = new CubicSpline();
             distance = spline.FitAndEval(x, y, xs, false);
             return distance[0];
         }
       

    }
}

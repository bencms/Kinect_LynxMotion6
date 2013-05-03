using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// This is a new namespace in .NET 2.0
// that contains the SerialPort class
using System.IO.Ports;
using ConsoleApplication4;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            Arm lynx = new Arm(); 
            //open ports 

            // Instantiate the communications
            // port with some basic settings
            SerialPort port = new SerialPort("COM5", 115200, Parity.None, 8, StopBits.One);

            // Open the port for communications
            port.Open();
            Console.WriteLine("Port open Sending first pos");

           
            lynx.gotoSleep(4000, port);
            Console.ReadLine();
            lynx.gotoSleep(4000, port);

           
            Console.WriteLine("First sequence sent");
            Console.ReadLine();
            Console.WriteLine("opening Link");
            Kinect vision = new Kinect(lynx,port);


            port.Close();
            Console.ReadLine();
  

        }

         public void Test(string[] args)
        {

            // Declare three variables that we will use in the format method.
            // ... The values they have are not important.
            //
            //Arm lynx = new Arm();

            int value1 = 0;
            int value2 = 1400;
            int value3 = 4000;
            int value4= 0;
            int value5 = 2200;
            int value6 = 2;
            int value7 = 1500;
            //
            // Use string.Format method with four arguments.
            // ... The first argument is the formatting string.
            // ... It specifies how the next three arguments are formatted.
            //
            string result = string.Format("#{0} P{1}  T{2} {3}", value1, value2, value3, Environment.NewLine);
            string result2 = string.Format("#{0} P{1}  T{2} {3}", value4, value5, value3, Environment.NewLine);
            string result3 = string.Format("#{0} P{1}  T{2} {3}", value6, value7, value3, Environment.NewLine);
            //
            // Write the result.
            //
            Console.Write(result);
            string name = Console.ReadLine();

            // Instantiate the communications
            // port with some basic settings
            SerialPort port = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One);
            
            // Open the port for communications
            port.Open();
  
            // Write a string
            port.Write(result);
            port.Write(result2);
            port.Write(result3);

            name = Console.ReadLine();
            // Close the port
            port.Close(); 
        }
    }
}





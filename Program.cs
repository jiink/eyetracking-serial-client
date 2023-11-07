using System;
using System.IO.Ports;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static SerialPort port = new SerialPort("COM9", 115200, Parity.None, 8, StopBits.One);
        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Show all the incoming data in the port's buffer
            Console.WriteLine(port.ReadExisting());
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            port.Handshake = Handshake.None;
            port.DataReceived += port_DataReceived;
            port.Open();
            Console.ReadLine();
        }

        
    }
}
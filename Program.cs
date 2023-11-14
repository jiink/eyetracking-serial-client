using Raylib_cs;
using System;
using System.IO.Ports;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static SerialPort port = new SerialPort("COM9", 115200, Parity.None, 8, StopBits.One);
        static int windowWidth = 640;
        static int windowHeight = 640;
        static int x = 0;
        static int y = 0;
        static void drawLocation(int x, int y)
        {
            Raylib.DrawCircle(x, y, 32, Color.RED);
        }
        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = port.ReadExisting();
            Console.Write(data);

            // Parse the received data to extract X and Y coordinates
            string[] coordinates = data.Split(',');

            foreach (string coord in coordinates)
            {
                if (coord.StartsWith("X_Location"))
                {
                    try
                    { 
                    int xLoc = int.Parse(coord.Split(':')[1]);
                    x = (int)(xLoc * ((float)windowWidth / 96.0));
                    } catch { }
                }
                else if (coord.StartsWith("Y_Location"))
                {
                    try
                    {
                        int yLoc = int.Parse(coord.Split(':')[1]);
                        y = (int)(yLoc * ((float)windowHeight / 96.0));
                    }   catch { }
                    
                }
            }
        }
        static void Main(string[] args)
        {
            Raylib.InitWindow(windowWidth, windowHeight, "Hello World");

            Console.WriteLine("Hello World!");
            port.Handshake = Handshake.None;
            port.DataReceived += port_DataReceived;
            try
            {
            port.Open();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
            while (!Raylib.WindowShouldClose())
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Q)
                        break; // Quit the program

                    char characterToSend = key.KeyChar;
                    port.Write(characterToSend.ToString());
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                Raylib.DrawText("Hello, world!", 12, 12, 20, Color.BLACK);
                
                drawLocation(x, y);
                Raylib.DrawText($"pos: {x}, {y}", 12, 30, 20, Color.BLUE);

                Raylib.EndDrawing();
            }
            port.Close();
            Raylib.CloseWindow();
        }

        
    }
}
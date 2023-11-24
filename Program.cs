using Raylib_cs;
using System;
using System.IO.Ports;
using System.Numerics;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static SerialPort port = new SerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
        static int windowWidth = 1280;
        static int windowHeight = 720;
        static int x = 0;
        static int y = 0;
        static Vector2 pupilPos = new Vector2(0.0f, 0.0f);
        static int millis = 0;
        static int t = 0;
        static void drawLocation(int x, int y)
        {
            Raylib.DrawCircle(x, y, 32, Color.RED);
        }

        static void drawGooglyEye(int x, int y, int radius, Vector2 pupil)
        {
            Raylib.DrawCircle(x, y, radius+4, Color.BLACK);
            // For outlining the white part of the eye
            Raylib.DrawCircle(x, y, radius, Color.WHITE); // Draw the white part of the eye

            // Calculate the position of the pupil within the eye
            int pupilX = x + (int)(pupil.X * radius * 0.5f);
            int pupilY = y + (int)(pupil.Y * radius * 0.5f);

            // Draw the pupil
            Raylib.DrawCircle(pupilX, pupilY, radius / 2, Color.BLACK);
        }

        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = port.ReadExisting();
            Console.Write(data);

            // Parse the received data to extract X and Y coordinates
            string[] coordinates = data.Split(',');

            foreach (string coord in coordinates)
            {
                if (coord.StartsWith("X"))
                {
                    try
                    { 
                    pupilPos.X = float.Parse(coord.Split(':')[1]);
                    x = (int)(pupilPos.X * 640/2 + 640/2);
                    } catch { }
                }
                else if (coord.StartsWith("Y"))
                {
                    try
                    {
                        pupilPos.Y = float.Parse(coord.Split(':')[1]);
                        y = (int)(pupilPos.Y * 640 / 2 + 640 / 2);
                    }   catch { }
                    
                }
                else if (coord.StartsWith("ms"))
                {
                    try
                    {
                        millis = int.Parse(coord.Split(':')[1]);
                    } catch { }
                }
            }
        }
        static void Main(string[] args)
        {
            Raylib.InitWindow(windowWidth, windowHeight, "Eye tracking");
            Raylib.SetTargetFPS(60);

            Texture2D fishTex = Raylib.LoadTexture("fish.png");

            Console.WriteLine("Begin!");
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
                t++;
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

                Raylib.DrawTexture(fishTex, 560, 200, Color.WHITE);
                drawGooglyEye(670, 365, 32, new Vector2(-pupilPos.X, pupilPos.Y));

                Raylib.DrawText("Hello, world!", 12, 12, 20, Color.BLACK);
                
                drawLocation(x, y);
                Raylib.DrawText($"pos: {x}, {y}", 12, 30, 20, Color.BLUE);
                Raylib.DrawText($"ms: {millis}", 12, 50, 20, Color.BLACK);
                Raylib.DrawText($"{Raylib.GetMouseX()}, {Raylib.GetMouseY()}", 16, Raylib.GetScreenHeight() - 16, 20, Color.GREEN);

                Raylib.EndDrawing();
            }
            port.Close();
            Raylib.CloseWindow();
        }

        
    }
}
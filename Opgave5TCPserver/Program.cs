using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Opgave1UnitTest;

namespace Opgave5TCPserver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, the Server is ready");
            TcpListener listener = new TcpListener(IPAddress.Any, 2121);
            listener.Start();

            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Console.WriteLine("Welcome client: "  + socket.Client.RemoteEndPoint.ToString());

                Task.Run(() => { HandleClient(socket); });
            }
        }

        public static List<FootballPlayer> fbOjects = new List<FootballPlayer>
        {
            new FootballPlayer {Id = 1, Name = "Ole Hansen", Price = 3000, ShirtNumber = 10},
            new FootballPlayer {Id = 2, Name = "Mark Andersen", Price = 2500, ShirtNumber = 6},
            new FootballPlayer {Id = 3, Name = "Patrick Pedersen", Price = 4000, ShirtNumber = 16}

        };

        private static void HandleClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            writer.WriteLine(
                "Hej, hvad kunne du tænke dig? skriv hentalle, for at hente alle, Hent for ID , gem for at gemme");
            writer.Flush();
            string message = reader.ReadLine();
            string messageID = reader.ReadLine();

            if (message.ToLower() == "hentalle")
            {
                foreach (var fbplayerinfo in fbOjects)
                {
                    writer.WriteLine(
                        $"Her er alle fodboldspillere:  {fbplayerinfo.Name} + {fbplayerinfo.Price} + {fbplayerinfo.ShirtNumber}");
                    writer.Flush();
                }
            }

            else if (message.ToLower() == "hent")
            {
                int id = -1;
                if (int.TryParse(messageID, out id))
                {
                    foreach (var fbplayerinfo in fbOjects)
                    {
                        if (fbplayerinfo.Id == id)
                        {
                            writer.WriteLine($"Her er spilleren ud fra ID: spiller id: {fbplayerinfo.Id}, spiller navn: {fbplayerinfo.Name}, spiller pris: {fbplayerinfo.Price}, spiller shirtnumber: {fbplayerinfo.ShirtNumber}");
                            writer.WriteLine("Spillerens info er udleveret");
                            writer.Flush();
                        }
                    }
                }
            }
            else if (message.ToLower() == "gem")
            {
                FootballPlayer spiller = JsonSerializer.Deserialize<FootballPlayer>(messageID);
                fbOjects.Add(spiller);
                writer.WriteLine("Played saved");
                writer.Flush();

            }
            
            socket.Close();
        }

    }
}


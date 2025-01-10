using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static TCPServerTemplate1.Server;

namespace TCPServerTemplate1
{
    public class Server
    {



        private const int PORT = 7531;

        //Liste af playgrounds

        public readonly List<Playground> playgrounds = new List<Playground>()
        {
            new Playground {Id = 1, Name = "Millpark", MaxChildren = 10, MinAge = 5 },
            new Playground { Id = 2, Name = "Secret Playground", MaxChildren = 12, MinAge = 4 },
            new Playground { Id = 3, Name = "Library", MaxChildren = 8, MinAge = 3 },
            new Playground { Id = 4, Name = "School", MaxChildren = 15, MinAge = 7 }
        };
    
        



        public void start()
        {
            //Definerer server med port nummer
            //
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, PORT);

            server.Start();

            while (true)
            {
                //Venter på klient
                TcpClient socket = server.AcceptTcpClient();

                //Her kører flere tasks samtidigt(concurrent).
                Task.Run(() =>
                {
                    DoOneClient(socket);
                });
            }

        }

        private void DoOneClient(TcpClient socket)
        {
            Console.WriteLine($"Min egen (IP, port) = {socket.Client.LocalEndPoint}");
            Console.WriteLine($"Accepteret client (IP, port = {socket.Client.RemoteEndPoint})");

            //Åbner for tekst strenge
            StreamReader sr = new StreamReader(socket.GetStream());
            StreamWriter sw = new StreamWriter(socket.GetStream()) { AutoFlush = true };


            //Serverfunktionalitet
            DoServer(sr, sw);


            


            socket.Close();
            sr?.Close();
            sw?.Close();


        }

        private void DoServer(StreamReader sr, StreamWriter sw)
        {




            //Step 1: Modtag Alder fra klient

            string input = sr.ReadLine();
            Console.WriteLine($"Revieved input: {input}");

            //Forsøger at konvertere til int age 
            if (!int.TryParse(input, out int age))
            {
                sw.WriteLine("Invalid input.");
                return;
            }


            // Step 2: Filtrér listen over legepladser baseret på alderen
            var allowedPlaygrounds = playgrounds.FindAll(p => p.MinAge <= age);

            // Step 4: Konverter listen til JSON-format
            string result = JsonSerializer.Serialize(allowedPlaygrounds);

            // Step 5: Send resultatet tilbage til klienten
            sw.WriteLine(result);
            Console.WriteLine($"Result sent to client: {result}");

        }


        public class Playground
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int MaxChildren { get; set; }
            public int MinAge { get; set; }

             
        }
    }
}

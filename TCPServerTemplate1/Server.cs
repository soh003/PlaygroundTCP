using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServerTemplate1
{
    public class Server
    {
        private const int PORT = 5000;

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
            DoServerUdenJson(sr, sw);


            


            socket.Close();
            sr?.Close();
            sw?.Close();


        }

        private void DoServerUdenJson(StreamReader sr, StreamWriter sw)
        {
            //Step 1: Modtag kommando (Random, Add, Subtract)

            string command = sr.ReadLine();
            Console.WriteLine($"Revieved command: {command}");

            //Step 2: Sender instruks til klienten
            sw.WriteLine("Input Numbers");

            //step 2: Modtag tal fra klienten
            string numbersInput = sr.ReadLine();
            Console.WriteLine($"Revieved numbers: {numbersInput}");



            //Modtager input
            string[] numbers = numbersInput.Split(' ');
            int num1 = int.Parse(numbers[0]);
            int num2 = int.Parse(numbers[1]);

            //Behandler input
            //her returneres et random tal mellem num1 og num2, begge inkluderet
            string result = string.Empty;

            if (command == "Random")
            {
                Random random = new Random();
                result = random.Next(num1, num2 + 1).ToString();
            }
            //her lægges num1 og num2 sammen og returneres som et resultat
            else if (command == "Add")
            {
                result = (num1 + num2).ToString();
            }
            //Her subtraktion af num1 og num2
            else if (command == "Subtract")
            {
                result = (num1 - num2).ToString();
            }
            else
            {
                result = "Unknown command";
            }
            //Sender resultat retur til klient
            sw.WriteLine(result);
            Console.WriteLine($"result: {result}");

        }
    }
}

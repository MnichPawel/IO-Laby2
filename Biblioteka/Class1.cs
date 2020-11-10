using System;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Biblioteka
{
    /// <summary>
    /// Klasa służąca do tworzenia serwera TCP który przyjmuje liczby i zwraca ich pierwiastek
    /// </summary>
    public class ServerTCP
    {
        public delegate void TransmissionDataDelegate(NetworkStream stream);

        TcpListener listener;
        //TcpClient client;
        //NetworkStream stream;

        IPAddress ip_address;
        int port_number;

        /// <summary>
        /// Konstruktor pobierający adres IP oraz port
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public ServerTCP(IPAddress ip, int port)
        {
            ip_address = ip;
            port_number = port;

            listener = new TcpListener(ip, port);
        }
        /// <summary>
        /// Funkcja która kończy się dopiero gdy do serwera podepnie się klient
        /// </summary>
        public void WaitForClient()
        {
            while (true)
            {
                listener.Start();
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(ServerLoop);

                transmissionDelegate.BeginInvoke(stream, TransmissionCallback, client);
            }
        }

        private void TransmissionCallback(IAsyncResult ar)
        {
            TcpClient tcpClient = (TcpClient)ar.AsyncState;
            tcpClient.Close();
        }

        /// <summary>
        /// Funkcja która oczekuje na wiadomości oraz na nie odpowiada, działa aż do przesłania ciągu znaków EXIT
        /// </summary>
        public void ServerLoop(NetworkStream stream)
        {
            stream.ReadTimeout = 100000;
            byte[] buffer_get = new byte[1024];
            byte[] buffer_send;

            buffer_send = Encoding.UTF8.GetBytes("Wprowadzaj liczby pojedynczo. W odpowiedzi otrzymasz te liczby spierwiastkowane. Napisz EXIT jeśli chcesz wyjść. Zależnie od ustawień komputera jako przecinek stosuje się , lub .\r\n");
            stream.Write(buffer_send, 0, buffer_send.Length);

            while (true)
            {
                try
                {
                    int size_recv = stream.Read(buffer_get, 0, 1024);

                    string str = Encoding.UTF8.GetString(buffer_get, 0, size_recv);

                    //Ignorowane są przejścia do następnej lini
                    if (str.Equals("\r\n")) { continue; }

                    //Zakończ pętle jeśli otrzymano string EXIT
                    if (str.Equals("EXIT")) { break; }

                    string output_string;
                    double input_number;

                    //Jeśli konwersja na typ double się uda wyślij wynik, jeśli nie to poinformuj użytkownika
                    if (Double.TryParse(str, out input_number))
                    {
                        double result = Math.Sqrt(input_number);

                        output_string = "sqrt(" + str + ") = " + result.ToString() + "\r\n";
                    }
                    else
                    {
                        output_string = "Nie wprowadzono liczby.\r\n";
                    }

                    buffer_send = Encoding.UTF8.GetBytes(output_string);

                    stream.Write(buffer_send, 0, buffer_send.Length);
                }
                catch (IOException e)
                {
                    break;
                }
            }
        }
    }
}

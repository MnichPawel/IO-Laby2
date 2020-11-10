using Biblioteka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;

namespace Laby1
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerTCP server = new ServerTCP(IPAddress.Parse("127.0.0.1"), 2000);

            server.WaitForClient(); //Serwer czeka na klienta

            //server.ServerLoop(); //Serwer czeka i odpowiada na wiadomości
        }
    }
}

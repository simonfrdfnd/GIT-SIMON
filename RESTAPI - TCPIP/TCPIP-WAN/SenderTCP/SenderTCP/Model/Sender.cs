using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterTCP
{
  public static class Sender
  {
    public static void Send(IPEndPoint ipRecipient, byte[] data)
    {
      try // Try connecting and send the message bytes  
      {
        System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(ipRecipient.Address.ToString(), ipRecipient.Port); // Create a new connection  
        NetworkStream stream = client.GetStream();
        stream.Write(data, 0, data.Length); // Write the bytes  
        stream.Dispose();
        client.Close();
      }
      catch (Exception e) // Catch exceptions  
      {
        Console.WriteLine(e.Message);
      }
    }
  }
}

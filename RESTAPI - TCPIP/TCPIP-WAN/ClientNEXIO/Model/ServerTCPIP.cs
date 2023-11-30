using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientNEXIO
{
  public class ServerTCPIP
  {
    public ServerTCPIP(IPEndPoint ipServer)
    {
      this.IpServer = ipServer;
    }

    public IPEndPoint IpServer { get; }

    public event EventHandler OnCommandReceived;

    public bool CommandTreated { get; set; }
    public string Answer { get; set; }

    public void Run(int bufferSize, CancellationToken cancelToken)
    {
      TcpListener listener = new TcpListener(this.IpServer);
      NetworkStream stream;
      Task.Run(() =>
      {
        try
        {
          listener.Start();
          cancelToken.Register(listener.Stop); // THIS IS IMPORTANT!
          while (!cancelToken.IsCancellationRequested)
          {
            TcpClient client = listener.AcceptTcpClient();
            // Once each client has connected, start a new task with included parameters.
            var task = Task.Run(async () =>
            {
              // Get a stream object for reading and writing
              stream = client.GetStream();
              Byte[] bytes = new Byte[bufferSize]; // Bytes variable
              int i;
              cancelToken.Register(client.Close); // THIS IS IMPORTANT!
              if (stream.CanRead)
              {
                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0 & !cancelToken.IsCancellationRequested)
                {
                  this.Answer = "";
                  this.CommandTreated = false;
                  string command = CleanMessage(bytes); //Clean la réponse
                  CommandReceivedEventArgs e = new CommandReceivedEventArgs();
                  e.Command = command;
                  OnCommandReceived?.Invoke(this, e);
                  if (command.Contains('?'))
                  {
                    while (!this.CommandTreated)
                    {
                      Thread.Sleep(100);
                    }
                      stream.Write(Encoding.ASCII.GetBytes(this.Answer + "\n"), 0, Encoding.ASCII.GetBytes(this.Answer + "\n").Length);
                    Console.WriteLine("Answer: " + this.Answer);
                  }
                }
                // Shutdown and end connection
                client.Close();
              }
            }, cancelToken);
          }
        }
        catch (SocketException ex)
        {
          Console.WriteLine();
        }
        finally
        {
          listener.Stop();
        }
      });
    }

    private static string CleanMessage(byte[] bytes)
    {
      string message = System.Text.Encoding.ASCII.GetString(bytes);

      string messageToPrint = null;
      foreach (var nullChar in message)
      {
        if (nullChar != '\0')
        {
          messageToPrint += nullChar;
        }
      }

      return messageToPrint.Replace('\n', ' ');
    }
  }

  public class CommandReceivedEventArgs : EventArgs
  {
    public string Command { get; set; }
  }
}
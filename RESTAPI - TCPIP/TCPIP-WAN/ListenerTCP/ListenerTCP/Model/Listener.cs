using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RepeaterTCP
{
  public class Listener
  {
    public Listener(IPEndPoint ipListener)
    {
      this.IpListener = ipListener;
    }

    public IPEndPoint IpListener { get; }

    public event EventHandler DataReceived;

    public void ListenerRun(int bufferSize, CancellationToken cancelToken)
    {
      TcpListener sender = new TcpListener(this.IpListener);
      NetworkStream streamSender;
      Task.Run(() =>
      {
        try
        {
          sender.Start();
          cancelToken.Register(sender.Stop); // THIS IS IMPORTANT!
          while (!cancelToken.IsCancellationRequested)
          {
            TcpClient clientSender = sender.AcceptTcpClient();
            // Once each client has connected, start a new task with included parameters.
            var task = Task.Run(async () =>
            {
              // Get a stream object for reading and writing
              streamSender = clientSender.GetStream();
              Byte[] bytes = new Byte[bufferSize]; // Bytes variable
              int i;
              cancelToken.Register(clientSender.Close); // THIS IS IMPORTANT!
              if (streamSender.CanRead)
              {
                // Loop to receive all the data sent by the client.
                while ((i = streamSender.Read(bytes, 0, bytes.Length)) != 0 & !cancelToken.IsCancellationRequested)
                {
                  DataReceivedEventArgs e = new DataReceivedEventArgs();
                  e.Data = bytes;
                  DataReceived?.Invoke(this, e);
                }

                // Shutdown and end connection
                clientSender.Close();
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
          sender.Stop();
        }
      });
    }
  }

  public class DataReceivedEventArgs : EventArgs
  {
    public byte[] Data { get; set; }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NexioMax.Helpers;
using System.Windows;

namespace NexioMax.Models
{
  internal class VisaServer
  {
    public TcpListenerWrapper listener { get; set; }

    private CancellationTokenSource tokenSource { get; set; }

    private CancellationToken token { get; set; }

    public VisaServer(int port)
    {
      this.listener = new TcpListenerWrapper();
      this.tokenSource = new CancellationTokenSource();
      this.token = tokenSource.Token;
      IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
      this.listener.TcpServerRun(endpoint, 2048, this.token);
    }

    public void CommandTreatment(string cmd)
    {
      this.listener.Write(cmd);
    }
  }

  public class TcpListenerWrapper
  {
    public bool IsClientConnected { get; internal set; } = false;

    private TcpListenerActive server;

    private NetworkStream stream;

    public event EventHandler ClientConnected;

    public event EventHandler ClientDisconnected;

    public event EventHandler CommandReceived;

    public async Task StartAsync(IPEndPoint iPEndPoint, CancellationToken token)
    {
      if (server != null)
      {
        server.Stop();
      }

      server = new TcpListenerActive(iPEndPoint.Address, iPEndPoint.Port);
      server.Start();
      token.Register(() => server.Stop());
      while (server.Active)
      {
        try
        {
          await ProcessConnection();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
        }
      }
    }

    public void Write(string msg)
    {
      byte[] bMsg;
      bMsg = System.Text.Encoding.ASCII.GetBytes(msg.Replace(',', '.') + "\n");
      stream.Write(bMsg, 0, bMsg.Length);
    }

    public void WriteHex(byte[] bMsg)
    {
      if (bMsg == null)
      {
        return;
      }
      if (bMsg.Last() == 0x0A)
      {
        stream.Write(bMsg, 0, bMsg.Length);
      }
      else
      {
        byte[] bMsg2 = new byte[bMsg.Length + 1];
        bMsg.CopyTo(bMsg2, 0);
        bMsg2[bMsg2.Length - 1] = 0x0A;
        stream.Write(bMsg2, 0, bMsg2.Length);
      }
    }

    public void TcpServerRun(IPEndPoint iPEndPoint, int bufferSize, CancellationToken cancelToken)
    {
      TcpListener listener = new TcpListener(iPEndPoint);

      Task.Run(() =>
      {
        try
        {
          listener.Start();
          cancelToken.Register(listener.Stop); // THIS IS IMPORTANT!

          while (!cancelToken.IsCancellationRequested)
          {
            TcpClient client = listener.AcceptTcpClient();
            this.IsClientConnected = true;
            ClientConnected?.Invoke(null, null);
            // Once each client has connected, start a new task with included parameters.
            var task = Task.Run(async () =>
            {
              // Get a stream object for reading and writing
              stream = client.GetStream();

              // Buffer for reading data
              Byte[] bytes = new Byte[bufferSize]; // Bytes variable

              String data = null;
              int i;

              cancelToken.Register(client.Close); // THIS IS IMPORTANT!

              // Checks CanRead to verify that the NetworkStream is readable.
              if (stream.CanRead)
              {
                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0 & !cancelToken.IsCancellationRequested)
                {
                  data = Encoding.ASCII.GetString(bytes, 0, i);
                  CommandReceivedEventArgs e = new CommandReceivedEventArgs();
                  e.Command = data;
                  CommandReceived?.Invoke(this, e);
                }

                // Shutdown and end connection
                client.Close();
                ClientDisconnected?.Invoke(null, null);
                this.IsClientConnected = false;
              }
            }, cancelToken);
          }
        }
        catch (SocketException ex)
        {
          Console.WriteLine(ex.ToString());
          listener.Stop();
          this.TcpServerRun(iPEndPoint, bufferSize, cancelToken);
          Console.WriteLine("Redémarrage du serveur...");
        }
        finally
        {
          listener.Stop();
        }
      });
    }

    private async Task ProcessConnection()
    {
      using (TcpClient client = await server.AcceptTcpClientAsync())
      {
        Byte[] bytes = new Byte[256];
        int i;
        String data = null;

        stream = client.GetStream();
        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
          // Translate data bytes to a ASCII string.
          data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
          Console.WriteLine("Received: {0}", data);

          // Process the data sent by the client.
          data = data.ToUpper();
          data = data.Substring(0, data.Length - 1);
          string[] cmd = data.Split('\n');
          foreach (string c in cmd)
          {
            CommandReceivedEventArgs e = new CommandReceivedEventArgs();
            e.Command = c;
            CommandReceived?.Invoke(this, e);
          }
        }
      }
    }

    private class TcpListenerActive : TcpListener, IDisposable
    {
      public TcpListenerActive(IPEndPoint localEP) : base(localEP)
      {
      }

      public TcpListenerActive(IPAddress localaddr, int port) : base(localaddr, port)
      {
      }

      public new bool Active => base.Active;

      public void Dispose()
      {
        Stop();
      }
    }
  }

  public class CommandReceivedEventArgs : EventArgs
  {
    public string Command { get; set; }
  }
}
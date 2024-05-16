namespace NexioMax3.Domain.Configuration.Service
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using Nexio;
  using NexioMax3.Domain.Configuration.Model;

  internal class VisuV3PrefProvider
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(VisuV3PrefProvider));

    /// <summary>
    /// Récupère la couleur depuis un COLORREF
    /// </summary>
    /// <param name="colorref">code couleur</param>
    /// <returns>la couleur</returns>
    public static System.Drawing.Color ToColor(int colorref)
    {
      var r = colorref & 0xFF;

      var g = (colorref >> 8) & 0xFF;

      var b = (colorref >> 16) & 0xFF;

      return System.Drawing.Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
    }

    internal static bool Read(string xmlFile, VisuV3PrefFile prefFile)
    {
      try
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(xmlFile);
        prefFile.MapPrefCourbe.Clear();
        prefFile.MapPrefCourbeExt.Clear();
        prefFile.MapPrefLimits.Clear();

        // lecture des prescans
        XmlNode curvesNode = doc.DocumentElement.SelectSingleNode("/root/curves");
        foreach (XmlNode posnode in curvesNode.ChildNodes)
        {
          // loop on positions
          int position = posnode.Attributes["value"].InnerText.ToInt();
          if (!prefFile.MapPrefCourbe.ContainsKey(position))
          {
            prefFile.MapPrefCourbe.Add(position, new Dictionary<int, PrefV3>());
          }

          var signList = prefFile.MapPrefCourbe[position];
          foreach (XmlNode signode in posnode.ChildNodes)
          {
            // loop on signature
            int signature = signode.Attributes["value"].InnerText.ToInt();
            if (signList.ContainsKey(signature))
            {
              // second definition => skip
              break;
            }

            signList[signature] = ReadPref(signode.FirstChild);
          }
        }

        // lecture des courbes exts
        var extCurvesNode = doc.DocumentElement.SelectSingleNode("/root/external_curves");
        foreach (XmlNode posnode in extCurvesNode.ChildNodes)
        {
          // loop on positions
          int position = posnode.Attributes["value"].InnerText.ToInt();
          if (!prefFile.MapPrefCourbeExt.ContainsKey(position))
          {
            prefFile.MapPrefCourbeExt.Add(position, new Dictionary<int, PrefV3>());
          }

          var signList = prefFile.MapPrefCourbeExt[position];
          foreach (XmlNode signode in posnode.ChildNodes)
          {
            // loop on signature
            int signature = signode.Attributes["value"].InnerText.ToInt();
            if (signList.ContainsKey(signature))
            {
              // second definition => skip
              break;
            }

            signList[signature] = ReadPref(signode.FirstChild);
          }
        }

        // lecture des limites
        var limitsNode = doc.DocumentElement.SelectSingleNode("/root/limits");
        foreach (XmlNode limnode in limitsNode.ChildNodes)
        {
          var pref = ReadLimit(limnode);
          prefFile.MapPrefLimits.Add(pref);
        }

        // lecture de l'échelle verticale
        var scaleNode = doc.DocumentElement.SelectSingleNode("/root/scale");
        if (scaleNode != null)
        {
          var min = scaleNode.Attributes["minY"].InnerText.ToDouble();
          var max = scaleNode.Attributes["maxY"].InnerText.ToDouble();
          prefFile.YRange = new Engine.AxisRange(min, max);
          var scaleAttr = scaleNode.Attributes["echelle_log"];
          if (scaleAttr != null)
          {
            prefFile.EchelleLog = scaleAttr.InnerText.ToInt() != 0; // Attribut avec la convention C (0 ou 1)
          }
        }

        // lecture de l'enveloppe max hold
        var enveloppeNode = doc.DocumentElement.SelectSingleNode("/root/enveloppe");
        if (enveloppeNode != null)
        {
          prefFile.IsMaxHoldTrace = enveloppeNode.Attributes["maxhold"].InnerText.ToBool();
        }
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("Failed to load {0}", xmlFile));
        Log.Error(ex);
        prefFile.MapPrefCourbe.Clear();
        prefFile.MapPrefCourbeExt.Clear();
        prefFile.MapPrefLimits.Clear();
        return false;
      }

      return true;
    }

    private static PrefV3 ReadPref(XmlNode node)
    {
      var name = node.Attributes["name"].InnerText.ToString();
      var color = ToColor(node.Attributes["color"].InnerText.ToInt());
      var style = node.Attributes["style"].InnerText.ToInt();
      var width = node.Attributes["width"].InnerText.ToDouble();
      var symbol = node.Attributes["symbol"].InnerText.ToInt();
      var function = node.Attributes["function"].InnerText.ToInt();
      var visible = node.Attributes["visible"].InnerText.ToInt() != 0;
      var visibleBE = node.Attributes["visibleBE"].InnerText.ToInt() != 0;
      var visibleBL = node.Attributes["visibleBL"].InnerText.ToInt() != 0;
      return new PrefV3()
      {
        Name = name,
        Color = color,
        Style = style,
        Width = width,
        Symbol = symbol,
        Function = function,
        Visible = visible,
        VisibleBE = visibleBE,
        VisibleBL = visibleBL,
      };
    }

    private static PrefLimitV3 ReadLimit(XmlNode node)
    {
      var name = node.Attributes["name"].InnerText.ToString();
      var color = ToColor(node.Attributes["color"].InnerText.ToInt());
      var style = node.Attributes["style"].InnerText.ToInt();
      var width = node.Attributes["width"].InnerText.ToDouble();
      var distance = node.Attributes["distance"].InnerText.ToDouble();
      var classe = node.Attributes["classe"].InnerText;
      var signature = node.Attributes["signature"].InnerText.ToInt();
      var visible = node.Attributes["visible"].InnerText.ToInt() != 0;
      var visibleBE = node.Attributes["visibleBE"].InnerText.ToInt() != 0;
      var visibleBL = node.Attributes["visibleBL"].InnerText.ToInt() != 0;
      var typeSignal = node.Attributes["type_signal"].InnerText.ToInt();
      var guid = node.Attributes["guid"].InnerText;
      return new PrefLimitV3()
      {
        Name = name,
        Color = color,
        Style = style,
        Width = width,
        Distance = distance,
        Classe = classe,
        Signature = signature,
        Visible = visible,
        VisibleBE = visibleBE,
        VisibleBL = visibleBL,
        TypeSignal = typeSignal,
        GuidLimite = guid,
      };
    }
  }
}
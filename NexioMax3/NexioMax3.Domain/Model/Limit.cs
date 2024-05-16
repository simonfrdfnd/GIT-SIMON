namespace NexioMax3.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using NexioMax3.Domain.Configuration.Model;
  using Nexio.Enums;

  public class Limit
  {
    private List<(double x, double y)> dataPoints = new List<(double x, double y)>();

    /// <summary>
    /// Point en Mhz et niveau
    /// </summary>
    public List<(double x, double y)> DataPoints
    {
      get
      {
        if (!this.FullLoaded)
        {
          if (NexioMax3.Domain.Service.Provider.Instance.GetLimiteNiveaux(this.Index, out int iNbVal, out double[] fFreq, out double[] fNiveaux))
          {
            // Transmet les valeurs de niveaux pour cette limite au graphe
            for (int j = 0; j < iNbVal; j += 2)
            {
              var pt1 = (x: fFreq[j], y: fNiveaux[j]);
              var pt2 = (x: fFreq[j + 1], y: fNiveaux[j + 1]);

              this.dataPoints.Add(pt1);
              this.dataPoints.Add(pt2);
            }
          }

          this.FullLoaded = true;
        }

        return this.dataPoints;
      }
    }

    public string Name { get; set; }

    public Detector Detector { get; set; }

    public double Distance { get; internal set; }

    public string Classe { get; internal set; }

    public InterpolationEnum Interpolation { get; internal set; }

    public string Guid { get; internal set; }

    public int TypeSignal { get; internal set; }

    public Guid Id { get; set; }

    public string Unit { get; set; }

    public string Description { get; set; }

    public bool IsVisible { get; set; }

    public int Index { get; internal set; }

    public int ClasseId { get; set; }

    public bool FullLoaded { get; set; } = false;

    /// <summary>
    /// Format limit name to add some information
    /// </summary>
    /// <param name="useShowLimitDirectoryOption">Si cette option est utilisé alors on utlise la case à cocher dans les options de VisuV4 ShowLimitDirectory pour définir comment on doit afficher le nom des limites dans les graphes</param>
    /// <returns>Limit name</returns>
    public string GetFormatedName(bool useShowLimitDirectoryOption = false)
    {
      int optionDesc = Nexio.Bat.Common.Domain.Infrastructure.Service.OptionProvider.GetOptionValue(Nexio.Bat.Common.Domain.Infrastructure.Service.OptionProvider.Options.DescLimitInsteadName);
      if (optionDesc == 1)
      {
        return this.Description;
      }

      // %s - Class:%s - %s/%.1fm/%s, info.strNom, info.strClasse, info.strSignature, info.fDistance, info.strTypeSignal
      // % s - Class:% s - % s /% s, info.strDesc, info.strClasse, info.strSignature, info.strTypeSignal // no dist
      string str = this.Name;

      // Suivant l'option Show limit folder dans visuv4 on modifie le nom de la limite affiché comme dans ExecutionViewModel.cs -> UpdateLimitsText()
      if (useShowLimitDirectoryOption)
      {
        // On fait un reload sinon on a pas la bonne valeur :) c'est pas ouf mais bon.
        VisuV4Config.Reload();
        if (!VisuV4Config.Instance.Chart.ShowLimitDirectory)
        {
          if (this.Name.Contains("-"))
          {
            var firstPart = this.Name.Split('-').First();

            str = firstPart.Contains("/") ? this.Name.Substring(firstPart.IndexOf('/') + 1) : this.Name;
          }
          else
          {
            str = this.Name.Contains("/") ? this.Name.Substring(this.Name.IndexOf('/') + 1) : this.Name;
          }
        }
      }

      if (!string.IsNullOrEmpty(this.Classe))
      {
        str += " - Class:" + this.Classe;
      }

      str += " " + this.Detector.ToStringDesc();

      if (this.Distance > 0)
      {
        var dist = this.Distance.ToString("#.0m").Replace(',','.');
        str += "/" + dist;
      }

      var signal = (TypeSignals)this.TypeSignal;

      switch (signal)
      {
        case TypeSignals.TYPE_SIGNAL_BE:
          str += "/Narrow Band";
          break;
        case TypeSignals.TYPE_SIGNAL_BL:
          str += "/Broad Band";
          break;
        case TypeSignals.NB_TYPES_SIGNAUX:
          break;
        case TypeSignals.TYPE_SIGNAL_NON_DEF:
        default:
          break;
      }

      return str;
    }

    public override string ToString()
    {
      return this.GetFormatedName();
    }
  }
}
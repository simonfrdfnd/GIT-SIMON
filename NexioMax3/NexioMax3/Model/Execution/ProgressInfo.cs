namespace Nexio.Bat.Emi.VisuV4.Domain.Model.Execution
{
  using System.Collections.Generic;
  using System.Linq;

  public class ProgressInfo
  {
    public bool IsProgression { get; set; }

    public double CurrentFreq { get; set; }

    public int CurrentBalayage { get; set; }

    public double FreqMin { get; set; }

    public double FreqMax { get; set; }

    public double FreqMinTraitee { get; set; }

    public double FreqMaxTraitee { get; set; }

    public int IModeAutomatique { get; set; }

    public double FFreqMinExecManu { get; set; }

    public double FFreqMaxExecManu { get; set; }

    public string SNomDLLManu { get; set; }

    public int[] ListeIDPointFreq { get; set; }

    public int NbPoint { get; set; }

    public int NbPointsTraites { get; set; }

    public bool IsReprise { get; set; }

    public string Message { get; set; }

    public string Montage { get; set; }

    public int TotalBalayage { get; set; } = -1;

    public void SetReprise(bool bReprise)
    {
      this.IsReprise = bReprise;
    }

    public void SetMessage(string message)
    {
      this.Message = message;
    }

    public void SetCurrentFreq(double fFreq)
    {
      this.CurrentFreq = fFreq;
      this.IsProgression = true;
    }

    public void SetFreqRange(double fMin, double fMax)
    {
      // mise a jour de la barre de progression
      this.FreqMin = fMin;
      this.FreqMax = fMax;
      this.IsProgression = true;
    }

    public void SetFreqRangeTraitees(double fMin, double fMax)
    {
      // mise a jour de la barre de progression
      this.FreqMinTraitee = fMin;
      this.FreqMaxTraitee = fMax;
      this.IsProgression = true;
    }

    public void SetNbPointsTraites(int nbrPoints)
    {
      // mise a jour de la barre de progression
      this.NbPointsTraites = nbrPoints;
      this.IsProgression = true;
    }

    public void SetCurrentBalayage(int balayage)
    {
      this.CurrentBalayage = balayage;
    }

    public void Start()
    {
      this.IsProgression = true;
      this.ClearInfos();
    }

    public void Finish()
    {
      this.IsProgression = false;
      this.ClearInfos();
    }

    public void SetTotalBalayage(int total)
    {
      this.TotalBalayage = total;
    }

    private void ClearInfos()
    {
      this.CurrentFreq = 0;
      this.FreqMin = 0;
      this.FreqMax = 0;
      this.FreqMinTraitee = 0;
      this.FreqMaxTraitee = 0;
      this.IModeAutomatique = 0;
      this.FFreqMinExecManu = 0;
      this.FFreqMaxExecManu = 0;
      this.SNomDLLManu = string.Empty;

      // ListeIDPointFreq
      this.NbPoint = 0;
      this.NbPointsTraites = 0;
      this.IsReprise = false;
      this.Message = string.Empty;
      this.Montage = string.Empty;
    }
  }
}
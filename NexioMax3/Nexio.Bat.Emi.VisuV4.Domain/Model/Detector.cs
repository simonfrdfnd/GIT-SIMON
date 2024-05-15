namespace NexioMax3.Domain.Model
{
  using System;

  // Signature des détecteurs (masque pour signature)
  public enum Detector
  {
    RMS = 1,

    AVERAGE = 2,

    QPEAK = 4,

    PEAK = 8,
    [Obsolete]
    CHANNEL_POWER = 16,
    [Obsolete]
    UMTS = 32,
    [Obsolete]
    QPE_AVG = 64,

    CISPR_RMS = 128,

    CISPR_AVERAGE = 256,
  }
}
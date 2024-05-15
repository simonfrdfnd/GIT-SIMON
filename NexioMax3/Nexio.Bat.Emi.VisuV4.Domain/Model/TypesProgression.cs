namespace NexioMax3.Domain.Model
{
  using System.ComponentModel;
  using NexioMax3.Domain.Service;

  [TypeConverter(typeof(DomainEnumConverter<TypesProgression>))]
  public enum TypesProgression
  {
    PROG_LINEAIRE = 0,

    PROG_POURCENTAGE = 1,

    PROG_LOG = 2,

    PROG_AUCUNE = 3,

    PROG_LOG_OCTAVE = 4,

    PROG_ESU_FFT = 5,

    PROG_ESU_FFT_DATA_REDUCTION = 6,

    PROG_HARMONIQUES = 7,

    PROG_LIST_FREQ = 8,
  }
}
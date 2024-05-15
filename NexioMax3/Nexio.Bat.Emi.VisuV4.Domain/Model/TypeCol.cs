namespace NexioMax3.Domain.Model
{
  /// <summary>
  /// Copie de ce fichier dans Nexio.Bat.Report.EmiWithoutBand.Enum si l'un change penser à changer l'autre
  /// </summary>
  public enum TypeCol
  {
    TYPECOL_OTHER = 0,

    TYPECOL_MESURE = 1,

    TYPECOL_LIMITE = 2,

    TYPECOL_DELTA = 3,

    TYPECOL_POSITION = 4,

    TYPECOL_LIMITE_RELATIVE = 5,   // Données relative a une limite (recalc ajoutera la difference de la nouvelle limite par rapport à l'ancienne)

    TYPECOL_MEAS_MINUS_LIMIT = 6,   // Données relative a un delta  (recalc retrancera la difference de la nouvelle limite par rapport à l'ancienne)

    TYPECOL_LIMIT_MINUS_MEAS = 7,

    TYPECOL_RBW = 8,

    TYPECOL_MEAS_TIME = 9,

    TYPECOL_MONITORING = 10,
  }
}
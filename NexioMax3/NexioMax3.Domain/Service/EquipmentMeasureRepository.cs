namespace NexioMax3.Domain.Service
{
  using System.Data.OleDb;
  using Nexio.Bat.Common.Domain.Infrastructure.AccessDataBase;

  public class EquipmentMeasureRepository
  {
    public static string GetNameById(int id)
    {
      string eqMeasureFamilyRequest = string.Format(@"SELECT DISTINCT
                                                      DRV.id_Mesureur,
                                                      DRV.Nom
                                                      FROM g_ClassesMateriels CLA, EMI_DEFMesureur DRV
                                                      WHERE DRV.id_Mesureur = {0}
                                                      AND CLA.idInstrument = DRV.id_Mesureur
                                                      AND CLA.idTypeMetaMateriel = 5210
                                                      ORDER BY DRV.Nom", id);

      using (OleDbDataReader measureFamilyReader = DataBase.Instance.ExecuteReader(eqMeasureFamilyRequest))
      {
        if (measureFamilyReader.Read())
        {
          return measureFamilyReader["Nom"].ToString();
        }
      }

      return string.Empty;
    }
  }
}
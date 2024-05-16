namespace NexioMax3.Domain.Repository
{
  using System;
  using System.Data.OleDb;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using Nexio.Bat.Common.Domain.Infrastructure.AccessDataBase;
  using NexioMax3.Domain.Model;

  public class TestRepository
  {
    public static Conclusion GetConclusion(Guid testId)
    {
      string getTestConclusion = string.Format(@"SELECT iConclusion FROM Emi4_Essai WHERE guidEssai = {0}", testId.ToSQL());
      using (OleDbDataReader testConclusionReader = DataBase.Instance.ExecuteReader(getTestConclusion))
      {
        if (testConclusionReader.Read())
        {
          string sConclusion = testConclusionReader["iConclusion"].ToString();
          return (Conclusion)Enum.Parse(typeof(Conclusion), sConclusion);
        }
        else
        {
          return Conclusion.INCONCLUSIVE;
        }
      }
    }

    public static void SetConclusion(Guid testId, Conclusion conclusion)
    {
      string deleteEmi4Essai = string.Format("DELETE FROM Emi4_Essai WHERE guidEssai = {0}", testId.ToSQL());
      DataBase.Instance.ExecuteNoQuery(deleteEmi4Essai);
      string insertEmi4Essai = string.Format("INSERT INTO Emi4_Essai(guidEssai, iConclusion) VALUES ({0},{1})", testId.ToSQL(), (int)conclusion);
      DataBase.Instance.ExecuteNoQuery(insertEmi4Essai);
    }
  }
}

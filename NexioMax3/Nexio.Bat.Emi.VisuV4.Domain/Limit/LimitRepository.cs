namespace Nexio.Bat.Emi.VisuV4.Domain.Limit
{
  using System;
  using System.Collections.Generic;
  using System.Data.OleDb;
  using System.Linq;
  using Nexio.Bat.Common.Domain.ATDB.Model;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using Nexio.Bat.Common.Domain.Infrastructure.AccessDataBase;

  public class LimitRepository
  {
    public const int RootLimitTypeObjectId = 40;

    public const int FolderTypeObjectId = 45;

    private const int METATYPE_LIMITE = 20;

    private const int METATYPE_LIMITE_FOLDER = 120;

    /// <summary>
    /// Gets the list of limit filtered by test type.
    /// </summary>
    public List<ATBObject> GetListByIdTypeTestWithFolders(int testTypeID)
    {
      if (testTypeID == 0)
      {
        return new List<ATBObject>();
      }

      /*
la structure des limites est la suivante :
   Folder
     SubFolder (autant qu'on veut)
       Limite par type de test
         Limit 1
         Limit 2
         ...
*/
      // niveau "Limite par type de test"
      int typeObjectLimitGroupOfTestType = 0;

      string sql1 = string.Format(@"SELECT DEF.idTypeFriend, DEF.idTypeObject, OBJ.guidObject, OBJ.sName
                                    FROM g_DEFJoinFriends DEF
                                    INNER JOIN g_ListeObjets OBJ ON DEF.idTypeFriend = OBJ.idTypeObject
                                    WHERE ((DEF.idTypeObject = {0}) AND (DEF.iMetaType = {1}))
                                    ORDER BY OBJ.sName", testTypeID.ToSQL(), METATYPE_LIMITE.ToSQL());
      var limits = new List<Limite>();
      using (OleDbDataReader reader = DataBase.Instance.ExecuteReader(sql1))
      {
        while (reader.Read())
        {
          typeObjectLimitGroupOfTestType = int.Parse(reader["idTypeFriend"].ToString());
          var l = new Limite()
          {
            ObjectType = (ObjectType)testTypeID,
            Guid = Guid.Parse(reader["guidObject"].ToString()),
            Name = reader["sName"].ToString(),
          };
          limits.Add(l);
        }

        if (limits.Count == 0)
        {
          return new List<ATBObject>();
        }
      }

      // type d'objet du niveau "Limit 1"
      int typeObjectLimitTestType = 0;
      string sql2 = string.Format(@"SELECT idTypeFriend
	                                  FROM g_DEFJoinFriends
	                                  WHERE idTypeObject = {0}
	                                  AND iMetaType = {1}", testTypeID.ToSQL(), METATYPE_LIMITE.ToSQL());
      using (OleDbDataReader reader = DataBase.Instance.ExecuteReader(sql2))
      {
        if (reader.Read())
        {
          typeObjectLimitTestType = Int32.Parse(reader["idTypeFriend"].ToString());
        }
        else
        {
          throw new DomainException("Invalid query");
        }
      }

      string sql = string.Format(@"SELECT OBJ.guidObject, OBJ.guidParent, OBJ.sName, OBJ.idTypeObject
                                  FROM g_ListeObjets OBJ
                                  WHERE OBJ.guidRoot IN (SELECT guidroot
                                                        FROM g_ListeObjets
                                                        WHERE idTypeObject = {0})
                                  AND (OBJ.idTypeObject IN ({1}, {2}, {3}, {4}))",
                    RootLimitTypeObjectId.ToSQL(),
                    FolderTypeObjectId.ToSQL(),
                    typeObjectLimitGroupOfTestType.ToSQL(),
                    typeObjectLimitTestType.ToSQL(),
                    RootLimitTypeObjectId.ToSQL());

      List<ATBObject> values = new List<ATBObject>();
      using (OleDbDataReader reader = DataBase.Instance.ExecuteReader(sql))
      {
        while (reader.Read())
        {
          Guid guidObject = new Guid(reader["guidObject"].ToString());
          Guid guidParent = new Guid(reader["guidParent"].ToString());
          string name = reader["sName"].ToString();
          int idTypeObject = int.Parse(reader["idTypeObject"].ToString());

          var value = new ATBObject() { GuidObjet = guidObject, GuidParent = guidParent, Name = name, TypeObject = idTypeObject };
          values.Add(value);
        }
      }

      // on supprime le niveau "Limite par type de test"
      foreach (var typeLimit in values.Where(v => v.TypeObject == typeObjectLimitGroupOfTestType))
      {
        foreach (var item in values.Where(v => v.GuidParent == typeLimit.GuidObjet))
        {
          item.GuidParent = typeLimit.GuidParent;
        }
      }

      return values;
    }

    public void Save(Model.Limit entityBase)
    {
      throw new NotImplementedException();
    }

    public List<int> GetLimitFolderIds()
    {
      var sql = string.Format(@"SELECT idTypeObject
                               FROM g_DEFTypesObjets
                               WHERE iMetaType={0}", METATYPE_LIMITE_FOLDER.ToSQL());

      var ids = new List<int>();
      using (var reader = DataBase.Instance.ExecuteReader(sql))
      {
        while (reader.Read())
        {
          if (int.TryParse(reader["idTypeObject"].ToString(), out var idTypeObject))
          {
            ids.Add(idTypeObject);
          }
        }
      }

      return ids;
    }

    public class ATBObject
    {
      public Guid GuidObjet { get; set; }

      public Guid GuidParent { get; set; }

      public string Name { get; set; }

      public int TypeObject { get; set; }

      public override string ToString()
      {
        return this.GuidObjet.ToString("B") + " " + this.GuidParent.ToString("B") + " " + this.Name + " " + this.TypeObject;
      }
    }

    public class Limite
    {
      public Guid Guid { get; set; }

      public string Name { get; set; }

      public ObjectType ObjectType { get; set; }
    }
  }
}

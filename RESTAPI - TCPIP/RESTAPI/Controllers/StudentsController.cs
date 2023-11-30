using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using StudentDataAccess;

namespace RESTAPI.Controllers
{
  public class StudentsController : ApiController
  {
    public IEnumerable<Student> Get()
    {
      using (StudentDetailsEntities entities = new StudentDetailsEntities())
      {
        return entities.StudentSet.ToList();
      }
    }

    public Student Get(int id)
    {
      using (StudentDetailsEntities entities = new StudentDetailsEntities())
      {
        return entities.StudentSet.FirstOrDefault(x => x.Id == id);
      }
    }

    public void Post()
    {
      using (StudentDetailsEntities entities = new StudentDetailsEntities())
      {
        entities.StudentSet.Add(new Student { Id=1, Age=2, FirstName="coucou", Gender="caca", LastName="wtf"});
        return;
      }
    }
  }
}
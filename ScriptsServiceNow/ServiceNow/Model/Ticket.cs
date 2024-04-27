using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceNow.Model
{
  public class Ticket
  {
    public Ticket(string number, string id, string updatedBy)
    {
      this.Number = number;
      this.Id = id;
      this.UpdatedBy = updatedBy;
    }

    public string Number { get; set; }
    public string Id { get; set; }
    public string UpdatedBy { get; set; }
  }
}

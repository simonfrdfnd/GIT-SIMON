using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceNow.Model
{
  public class Attachment
  {
    public Attachment(string name, string extension)
    {
      Name = name;
      Extension = extension;
    }

    public string Name { get; set; }
    public string Extension { get; set; }
  }
}

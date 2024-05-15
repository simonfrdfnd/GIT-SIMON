namespace NexioMax3.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;

  public class Position
  {
    public Position(int id, string name)
    {
      this.Id = id;
      this.Name = name;
    }

    public int Id { get; set; }

    public string Name { get; set; }
  }
}
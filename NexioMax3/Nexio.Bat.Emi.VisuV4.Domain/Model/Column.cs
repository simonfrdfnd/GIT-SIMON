namespace NexioMax3.Domain.Model
{
  public class Column
  {
    public string Name { get; set; }

    public TypeCol Type { get; set; }

    public bool Used { get; set; }

    public int Param1 { get; set; }

    public int Param2 { get; set; }

    public double Offset { get; set; }

    public Function Function { get; set; }

    public override bool Equals(object obj)
    {
      if (obj == null)
      {
        return false;
      }

      if (obj is Column c)
      {
        return this.Name == c.Name
            && this.Used == c.Used
            && this.Type == c.Type
            && this.Offset == c.Offset
            && this.Param1 == c.Param1
            && this.Param2 == c.Param2
            && this.Function?.ID == c.Function?.ID
            && this.Function?.Name == c.Function?.Name;
      }

      return false;
    }
  }
}
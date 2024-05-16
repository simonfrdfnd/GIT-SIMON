namespace NexioMax3.Domain.Model
{
  public class TypeSignal
  {
    public TypeSignal(int id, string name)
    {
      this.Name = name;
      this.Id = id;
    }

    public string Name { get; set; }

    public int Id { get; set; }

    public override string ToString()
    {
      return this.Name;
    }
  }
}

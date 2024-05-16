namespace NexioMax3.Definition.ViewModel
{
  using System.Collections.Generic;
  using Nexio.Validation;

  public class ValidationMessageComparer : IEqualityComparer<ValidationMessage>
  {
    private static ValidationMessageComparer _instance;

    public static ValidationMessageComparer Instance => _instance ?? (_instance = new ValidationMessageComparer());

    public bool Equals(ValidationMessage x, ValidationMessage y)
    {
      if (ReferenceEquals(x, y))
      {
        return true;
      }

      if (ReferenceEquals(x, null))
      {
        return false;
      }

      if (ReferenceEquals(y, null))
      {
        return false;
      }

      if (x.GetType() != y.GetType())
      {
        return false;
      }

      return x.Level == y.Level && x.Message == y.Message && Equals(x.Sender, y.Sender) && Equals(x.MemberNames, y.MemberNames);
    }

    public int GetHashCode(ValidationMessage obj)
    {
      unchecked
      {
        var hashCode = (int)obj.Level;
        hashCode = (hashCode * 397) ^ (obj.Message != null ? obj.Message.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (obj.Sender != null ? obj.Sender.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (obj.MemberNames != null ? obj.MemberNames.GetHashCode() : 0);

        return hashCode;
      }
    }
  }
}
namespace NexioMax3.Markups
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Markup;

  public class EnumBindingSourceExtension : MarkupExtension
  {
    private Type enumType;

    public EnumBindingSourceExtension()
    {
    }

    public EnumBindingSourceExtension(Type enumType, bool removeObselete = false)
    {
      this.EnumType = enumType;
      this.RemoveObselete = removeObselete;
    }

    public Type EnumType
    {
      get => this.enumType;
      set
      {
        if (value != this.enumType)
        {
          if (value != null)
          {
            var et = Nullable.GetUnderlyingType(value) ?? value;

            if (!et.IsEnum)
            {
              throw new ArgumentException("The given type isn't an enum");
            }
          }
        }

        this.enumType = value;
      }
    }

    public bool RemoveObselete { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (this.EnumType == null)
      {
        throw new InvalidOperationException("No enum type given");
      }

      var actualType = Nullable.GetUnderlyingType(this.EnumType) ?? this.EnumType;

      Array enumValues = Enum.GetValues(actualType);

      if (!this.RemoveObselete)
      {
        return enumValues;
      }

      var result = new List<object>();
      foreach (var enumValue in enumValues)
      {
        if (!IsObsolete(enumValue))
        {
          result.Add(enumValue);
        }
      }

      return result;
    }

    private static bool IsObsolete(object value)
    {
      var fi = value.GetType().GetField(value.ToString());
      var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
      return attributes.Length > 0;
    }
  }
}

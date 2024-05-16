namespace NexioMax3.Domain.Model
{
  using System;
  using System.ComponentModel;
  using Nexio.Bat.Common.Domain.Infrastructure;

  [TypeConverter(typeof(DomainEnumConverter<Mode>))]
  [Flags]
  public enum Mode
  {
    None = 0,

    AUTO = 1,

    MANUALOperations = 2,

    MANUALPoint = 4,

    MANUAL = MANUALOperations | MANUALPoint,

    VIEW = 8,
  }
}
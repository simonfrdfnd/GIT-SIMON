namespace Nexio.Bat.Emi.VisuV4.Domain.Model
{
  using System.ComponentModel;
  using Nexio.Bat.Emi.VisuV4.Domain.Service;

  [TypeConverter(typeof(DomainEnumConverter<SpecialFilter>))]
  public enum SpecialFilter
  {
    None,
    Mil_ND,
    Mil_BB,
    Mil_NB,
    CRBM_Prec,
    CRBM_IL,
    GTEM_Correlation,
    GTEM_Axis,
  }
}

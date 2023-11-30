using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NexioMax.Models
{
  public class VirtualInstrumentCallRow : CallRow
  {
    public VirtualInstrumentCallRow(int number, string description, string answer, string address)
    {
      this.Number = number;
      this.Description = description;
      this.Address = address;
      this.Answer = answer;
    }

    public string Answer { get; set; }
  }
}

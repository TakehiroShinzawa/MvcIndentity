using System.ComponentModel.DataAnnotations.Schema;

namespace MvcIdentity.Models
{
  [ComplexType]
  public class Address
  {
    //[Column("Prefecture")]
    public string Prefecture { get; set; }

    public string City { get; set; }
    public string Street { get; set; } 
  }
}
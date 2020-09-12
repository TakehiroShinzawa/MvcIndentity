using System.ComponentModel.DataAnnotations.Schema;

namespace MvcIdentity.Models
{
  public class Person
  {
    public int Id { get; set; } 
    public string Name { get; set; } 
    public Address Address { get; set; }
  }
}
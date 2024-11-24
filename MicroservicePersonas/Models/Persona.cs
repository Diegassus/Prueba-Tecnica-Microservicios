namespace MicroservicePersonas.Models;

public class Persona {
  public int Id { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public DateTime? BirthDate { get; set; }
  public bool? HasInsurance { get; set; }
  public IList<int>? Movies { get; set; }
}
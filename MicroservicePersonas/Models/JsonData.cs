namespace MicroservicePersonas.Models;

public class JsonData
{
  public string? error {get; set;}
  public dynamic? content {get; set;}
  public int? count {get; set;}
  public Result result {get; set;}

  public enum Result
  {
    OK = 0,
    ERROR = -1
  }
}


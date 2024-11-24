using Microsoft.AspNetCore.Mvc;
using MicroservicePersonas.Database;
using MicroservicePersonas.Models;
using System.Net.Http;
using System.Text.Json;

namespace MicroservicePersonas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonasController : ControllerBase
{
  private readonly AppDbContext _context;
  private readonly HttpClient _httpClient;
  private readonly string MOVIES_URL = "http://localhost:5127/api/movies";
  private const int MAX_MOVIES = 3;

  public PersonasController(AppDbContext context, HttpClient httpClient)
  {
    _context = context;
    _httpClient = httpClient;
  }

  [HttpGet]
  public JsonData GetPersonas(){ // Devuelve un listado de personas para una grilla
    JsonData jsonData = new JsonData();
    try
    {
      var personas = _context.Personas
                                .Select(p => new { p.Id , p.FirstName, p.LastName, p.BirthDate, p.HasInsurance })
                                .ToList()
                                .OrderBy(p => p.LastName)
                                .ThenBy(p => p.FirstName)
                                .ToList();
      jsonData.content = new { items = personas };
      jsonData.count = personas.Count;
      jsonData.result = JsonData.Result.OK;
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex.Message); // Mas optimo guardar en un .txt para hacer un debug.
      jsonData.error = "Ocurrio un problema al obtener las personas. Contacte al area de sistemas"; // Tambien lo ideal es que los textos esten en un Global.resx
      jsonData.result = JsonData.Result.ERROR;
    }
    return jsonData;
  }

  private async Task<JsonData> GetPersonaById(int id){ // Devuelve el detalle de una persona segun su id
    JsonData jsonData = new JsonData();
    try
    {
      #region Validations
      if(id < 1){
        jsonData.error = "El ID brindado no es valido.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      var persona = _context.Personas.Find(id);

      if(persona == null){
        jsonData.error = "No se pudo encontrar a la persona.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }
      #endregion
      
      if(persona.Movies.Count() != 0){
        var json = JsonSerializer.Serialize(persona.Movies);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{MOVIES_URL}", content);
        response.EnsureSuccessStatusCode();

        var moviesJson = await response.Content.ReadFromJsonAsync<JsonData>();

        if(moviesJson.result != JsonData.Result.OK)
        {
          jsonData.error = "No se pudo obtener la pelicula solicitada.";
          jsonData.result = JsonData.Result.ERROR;
          return jsonData;
        }

        jsonData.content = new { items = new {
          persona.Id,
          persona.FirstName,
          persona.LastName,
          persona.BirthDate,
          persona.HasInsurance,
          Movies = moviesJson.content
        } };
      } else 
      {
        jsonData.content = new { items = persona };
      }
      
      jsonData.count = 1;
      jsonData.result = JsonData.Result.OK;
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex.Message);
      jsonData.error = "Ocurrio un problema al obtener la persona. Contacte al area de sistemas";
      jsonData.result = JsonData.Result.ERROR;
    }
    return jsonData;
  }

  [HttpGet]
  [Route("{value}")]
  public async Task<JsonData> GetPersonaByName(string value){ // Obtiene una persona segun su nombre. Verifica que el dato no corresponda al ID
    JsonData jsonData = new JsonData();
    int id = 0;
    string name = "";

    if(int.TryParse(value, out id))
    {
      return await GetPersonaById(id);
    }
    else
      name = value;

    try
    {

      if(string.IsNullOrEmpty(name))
      {
        jsonData.error = "Los datos proporcionados no son correctos.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      var persona = _context.Personas.Where(p => p.FirstName == name || p.LastName == name).FirstOrDefault();

      if(persona == null){
        jsonData.error = "No se pudo encontrar a la persona solicitada.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      if(persona.Movies.Count() != 0){
        var json = JsonSerializer.Serialize(persona.Movies);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{MOVIES_URL}", content);
        response.EnsureSuccessStatusCode();

        var moviesJson = await response.Content.ReadFromJsonAsync<JsonData>();

        if(moviesJson.result != JsonData.Result.OK)
        {
          jsonData.error = "No se pudo obtener la pelicula solicitada.";
          jsonData.result = JsonData.Result.ERROR;
          return jsonData;
        }

        jsonData.content = new { items = new {
          persona.Id,
          persona.FirstName,
          persona.LastName,
          persona.BirthDate,
          persona.HasInsurance,
          Movies = moviesJson.content
        } };
      } else 
      {
        jsonData.content = new { items = persona };
      }
      jsonData.count = 1;
      jsonData.result = JsonData.Result.OK;
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex.Message);
      jsonData.error = "Ocurrio un problema al obtener la persona. Contacte al area de sistemas";
      jsonData.result = JsonData.Result.ERROR;
    }
    return jsonData;
  }

  [HttpPost]
  public JsonData CreatePersona([FromBody] Persona persona){
    JsonData jsonData = new JsonData();
    try
    {
      if(string.IsNullOrEmpty(persona.FirstName) 
        || string.IsNullOrEmpty(persona.LastName) 
        || persona.Id < 1 // El ID no deberia ser el que se envía en el request. Deberia generarse en el controller o al hacer el insert en la DB
        || persona.BirthDate == null 
        || persona.HasInsurance == null)
      {
        jsonData.error = "Los datos envidos son invalidos";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      _context.Personas.Add(persona);
      _context.SaveChanges();

      jsonData.content = new { items = persona };
      jsonData.count = 1;
      jsonData.result = JsonData.Result.OK;
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex.Message);
      jsonData.error = "Ocurrio un problema al intentar crear a la persona. Contacte al area de sistemas";
      jsonData.result = JsonData.Result.ERROR;
    }
    return jsonData;
  }

  [HttpPatch]
  [Route("{id}")]
  public JsonData UpdatePersona(string id,[FromBody] Persona persona){
    JsonData jsonData = new JsonData();
    try
    {
      var personaOld = _context.Personas.Find(int.Parse(id));

      if(!string.IsNullOrEmpty(persona.FirstName))
        personaOld.FirstName = persona.FirstName;

      if(!string.IsNullOrEmpty(persona.LastName))
        personaOld.LastName = persona.LastName;

      if(persona.BirthDate != null)
        personaOld.BirthDate = persona.BirthDate;

      if(persona.HasInsurance != null)
        personaOld.HasInsurance = persona.HasInsurance;

      _context.Personas.Update(personaOld);
      _context.SaveChanges();

      jsonData.content = new { items = personaOld };
      jsonData.count = 1;
      jsonData.result = JsonData.Result.OK;
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex.Message);
      jsonData.error = "Ocurrio un problema al intentar modificar a la persona. Contacte al area de sistemas";
      jsonData.result = JsonData.Result.ERROR;
    }
    return jsonData;
  }

  [HttpDelete]
  [Route("{id}")]
  public JsonData DeletePersona(string id){
    JsonData jsonData = new JsonData();
    try
    {
      if(int.Parse(id) < 1){
        jsonData.error = "Id inválido";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      var persona = _context.Personas.Find(int.Parse(id));
      if(persona == null){
        jsonData.error = "No hay personas que coincidan con los datos enviados.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      _context.Personas.Remove(persona);
      _context.SaveChanges();

      
      jsonData.content = new { message = "Se elimino exitosamente a la persona" };
      jsonData.result = JsonData.Result.OK;
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex.Message);
      jsonData.error = "Ocurrio un problema al intentar eliminar a la person. Contacte al area de sistemas";
      jsonData.result = JsonData.Result.ERROR;
    }

    return jsonData;
  }

  // CREAR MICROSERVICIO DE PELICULAS ANTES DE AVANZAR

  [HttpPost]
  [Route("{id}")]
  public async Task<JsonData> AddMovie(string id, [FromBody] int movie){ 
    JsonData jsonData = new JsonData();
    try
    {
      if(int.Parse(id) < 1 || movie < 1)
      {
        jsonData.error = "Los datos enviados son invalidos";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      var persona = _context.Personas.Find(int.Parse(id));
      if(persona == null)
      {
        jsonData.error = "No se pudo recuperar a la persona.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      if(persona.Movies.Count() >= MAX_MOVIES)
      {
        jsonData.error = "La persona ya tiene el maximo de peliculas permitidas.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      var response = await _httpClient.GetAsync($"{MOVIES_URL}/{movie}");
      response.EnsureSuccessStatusCode();

      var moviesJson = await response.Content.ReadFromJsonAsync<JsonData>();

      if(moviesJson.result != JsonData.Result.OK)
      {
        jsonData.error = "No se pudo obtener la pelicula solicitada.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      persona.Movies.Add(movie);

      _context.Personas.Update(persona);
      _context.SaveChanges();

      jsonData.content = new { message = "Se agrego exitosamente la pelicula a la persona" };
      jsonData.count = 0;
      jsonData.result = JsonData.Result.OK;

    }
    catch(Exception ex)
    {
      Console.WriteLine(ex.Message);
      jsonData.error = "Ocurrio un problema al intentar agregar la pelicula a la persona. Contacte al area de sistemas";
      jsonData.result = JsonData.Result.ERROR;
    }
    return jsonData;
  }

  [HttpDelete]
  [Route("{id}/{movie}")]
  public async Task<JsonData> DeleteMovie(string id, string movie){
    JsonData jsonData = new JsonData();
    try
    {
      if(int.Parse(id) < 1 || int.Parse(movie) < 1)
      {
        jsonData.error = "Los datos enviados son invalidos";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      var persona = _context.Personas.Find(int.Parse(id));
      if(persona == null)
      {
        jsonData.error = "No se pudo recuperar a la persona.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      if(persona.Movies.Count == 0)
      {
        jsonData.error = "No existen peliculas para eliminar";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      if(!persona.Movies.Contains(int.Parse(movie)))
      {
        jsonData.error = "La pelicula seleccionada no se encuentra en la lista de peliculas de la persona.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      var response = await _httpClient.GetAsync($"{MOVIES_URL}/{movie}");
      response.EnsureSuccessStatusCode();

      var moviesJson = await response.Content.ReadFromJsonAsync<JsonData>();

      if(moviesJson.result != JsonData.Result.OK)
      {
        jsonData.error = "No se pudo obtener la pelicula solicitada.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      persona.Movies.Remove(int.Parse(movie));

      _context.Personas.Update(persona);
      _context.SaveChanges();

      jsonData.content = new { message = "Se elimino exitosamente la pelicula de la persona" };
      jsonData.count = 0;
      jsonData.result = JsonData.Result.OK;
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex.Message);
      jsonData.error = "Ocurrio un problema al intentar eliminar la pelicula de la persona. Contacte al area de sistemas";
      jsonData.result = JsonData.Result.ERROR;
    }
    return jsonData;
  }
}
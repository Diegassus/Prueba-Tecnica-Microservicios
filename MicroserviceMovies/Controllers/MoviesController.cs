using Microsoft.AspNetCore.Mvc;
using MicroserviceMovies.Models;
using MicroserviceMovies.Database;

namespace MicroserviceMovies.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
  private readonly AppDbContext _context;

  public MoviesController(AppDbContext context){
    _context = context;
  }

  [HttpPost]
  public JsonData GetMovies([FromBody]List<int> ids){
    JsonData jsonData = new JsonData();
    try
    {
      if(ids.Count == 0){
        jsonData.error = "Los datos proporcionados no son validos.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      var movies = _context.Movies.Where(m => ids.Contains(m.Id)).ToList();

      jsonData.content = new { items = movies };
      jsonData.count = movies.Count;
      jsonData.result = JsonData.Result.OK;
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex.Message);
      jsonData.error = "Ocurrio un problema al obtener las peliculas. Contacte al area de sistemas";
      jsonData.result = JsonData.Result.ERROR;
    }
    return jsonData;
  }

  [HttpGet]
  [Route("{id}")]
  public JsonData ValidateMovie(string id){
    JsonData jsonData = new JsonData();
    try
    {
      if(int.Parse(id) < 1){
        jsonData.error = "Los datos enviados son invalidos.";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      var movie = _context.Movies.Find(int.Parse(id));
      if(movie == null){
        jsonData.error = "No hay peliculas que coincidan con los datos enviados";
        jsonData.result = JsonData.Result.ERROR;
        return jsonData;
      }

      jsonData.content = new { items = movie };
      jsonData.count = 1;
      jsonData.result = JsonData.Result.OK;
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex.Message);
      jsonData.error = "Ocurrio un problema al obtener la pelicula. Contacte al area de sistemas";
      jsonData.result = JsonData.Result.ERROR;
    }
    return jsonData;
  }

  [HttpGet] // Esto es para testear
  public JsonData CargarPeliculas(){
    JsonData jsonData = new JsonData();
    var peli1 = new Movie { Id = 1, Title = "Pelicula 1", Genre = "Accion" };
    var peli2 = new Movie { Id = 2, Title = "Pelicula 2", Genre = "Comedia" };

    _context.Movies.AddRange(peli1, peli2);
    _context.SaveChanges();

    jsonData.content = new { items = new List<Movie> { peli1, peli2 } };
    jsonData.result = JsonData.Result.OK;

    return jsonData;
  }
}
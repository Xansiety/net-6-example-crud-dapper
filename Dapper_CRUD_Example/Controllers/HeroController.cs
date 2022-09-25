using Dapper;
using Dapper_CRUD_Example.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Dapper_CRUD_Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HeroController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var db = new SqlConnection(connectionString: _configuration.GetConnectionString("DefaultConnection"));
            IEnumerable<SuperHero> heroes = await SelectAllHeroes(db);
            return Ok(heroes);
        }



        [HttpGet("{id:int}", Name = "GetHeroById")]
        public async Task<ActionResult<SuperHero>> GetHero(int id)
        {
            using var db = new SqlConnection(connectionString: _configuration.GetConnectionString("DefaultConnection"));
            var heroe = await db.QueryFirstOrDefaultAsync<SuperHero>("select * from SuperHeroes where id=@id",
                new
                {
                    id
                });

            if (heroe is null) return NotFound();

            return Ok(heroe);
        }



        [HttpPost]
        public async Task<ActionResult> CreateHero(SuperHero hero)
        {
            using var db = new SqlConnection(connectionString: _configuration.GetConnectionString("DefaultConnection"));
            var id = await db.ExecuteScalarAsync("insert into SuperHeroes (Name, FirstName, LastName, Place) values (@Name, @FirstName, @LastName, @Place)  SELECT SCOPE_IDENTITY();", hero);

            return CreatedAtRoute("GetHeroById", new { id = id }, hero); ; //devuelve una URL para acceder al autor creado
            //return Ok(await SelectAllHeroes(db));
        }


        [HttpPut]
        public async Task<ActionResult> UpdateHero(SuperHero hero)
        {
            using var db = new SqlConnection(connectionString: _configuration.GetConnectionString("DefaultConnection"));
            var id = await db.ExecuteAsync("update SuperHeroes set  Name= @Name,FirstName= @FirstName,LastName= @LastName, Place= @Place where id=@id",
                new
                {
                    id = hero.Id,
                    Name = hero.Name,
                    FirstName = hero.FirstName,
                    LastName = hero.LastName,
                    Place = hero.Place
                });

            return NoContent();
        }



        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteHero(int id)
        {
            using var db = new SqlConnection(connectionString: _configuration.GetConnectionString("DefaultConnection"));
            await db.ExecuteAsync("delete from SuperHeroes where id=@id",
                new
                {
                    id
                });


            return NoContent();
        }


        [HttpGet]
        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection db)
        {
            return await db.QueryAsync<SuperHero>("select * from SuperHeroes");
        }


    }
}

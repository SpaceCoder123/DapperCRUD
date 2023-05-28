using Dapper;
using DapperCRUD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace DapperCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;
        public SuperHeroController(IConfiguration config)
        {
            this._config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var heroes = await connection.QueryAsync<SuperHero>("select * from SuperHeroes");
            return Ok(heroes);
        }

        [HttpGet("id")]
        public async Task<ActionResult<SuperHero>> GetSingleHero(int HeroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var hero = await connection.QueryFirstAsync<SuperHero>($"select * from SuperHeroes where id = @Id", new {Id = HeroId});
            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> AddSingleHero( SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"    ));
            await connection.ExecuteAsync("insert into SuperHeroes (Id, Name, FirstName, LastName, Place) values (@Id, @Name, @FirstName, @LastName, @Place)", hero);
            return Ok(await SelectAllSuperHeroes(connection));
        }

        private static async Task<IEnumerable<SuperHero>> SelectAllSuperHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("select * from SuperHeroes");
        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero Hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update SuperHeroes set Id = @Id, Name = @Name, FirstName = @FirstName, LastName = @LastName, Place = @Place where Id = @Id",Hero);
            return Ok(await SelectAllSuperHeroes(connection));
        }

        

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        public PalavrasController(IPalavraRepository repository)
        {
            _repository = repository;
        }

        // api/palavras
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {
            var item = _repository.ObterPalavras(query);
            if(query.PagNumero > item.Paginacao.TotalPaginas)
            {
                return NotFound();
            } 
            else
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));
                return Ok(item.ToList());
            }
        }

        // api/palavras/1
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _repository.Obter(id);
            if(obj == null)
            {
                return StatusCode(404);
            } else
            {
                return Ok(obj);
            }            
        }

        // api/palavras (POST: id, nome )
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _repository.Cadastrar(palavra);
            return Created($"api/palavras/{palavra.Id}", palavra);
        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            var obj = _repository.Obter(id);
            if (obj == null)
            {
                return StatusCode(404);
            }
            else
            {
                palavra.Id = id;
                _repository.Atualizar(palavra);
                return Created($"api/palavras/{palavra.Id}", palavra);
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _repository.Obter(id);
            if (palavra == null)
            {
                return StatusCode(404);
            }
            else
            {
                _repository.Deletar(id);
                return NoContent();
            }
        }
    }
}

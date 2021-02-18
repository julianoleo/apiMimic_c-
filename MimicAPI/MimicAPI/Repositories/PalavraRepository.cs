﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MimicAPI.Repositories
{
    public class PalavraRepository : IPalavraRepository
    {
        private readonly MimicContext _banco;

        public PalavraRepository(MimicContext banco)
        {
            _banco = banco;
        }

        public PaginationList<Palavra> ObterPalavras(PalavraUrlQuery query)
        {
            var lista = new PaginationList<Palavra>();
            var item = _banco.Palavras.AsNoTracking().AsQueryable();
            if (query.Data.HasValue)
            {
                item = item.Where(a => a.Criado > query.Data.Value || a.Atualizado > query.Data.Value);
            }
            if (query.PagNumero.HasValue)
            {
                var qtdeTotalRegistros = item.Count();
                item = item.Skip((query.PagNumero.Value - 1) * query.QtdeReg.Value).Take(query.QtdeReg.Value);

                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.PagNumero.Value;
                paginacao.RegistroPorPagina = query.QtdeReg.Value;
                paginacao.TotalRegistros = qtdeTotalRegistros;
                paginacao.TotalPaginas = (int)Math.Ceiling(((double)qtdeTotalRegistros / query.QtdeReg.Value));
                lista.Paginacao = paginacao;
            }
            lista.AddRange(item.ToList());
            return lista;
        }

        public Palavra Obter(int id)
        {
            return _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public void Cadastrar(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }

        public void Atualizar(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public void Deletar(int id)
        {        
            var palavra = Obter(id);
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }
    }
}
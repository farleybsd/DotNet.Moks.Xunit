using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Castle.Core.Logging;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.Extensions.Logging;
using Alura.CoisasAFazer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Alura.CoisasAFazer.Core.Models;

namespace Alura.CoisasAFazer.Testes
{
    public class TarefasControllerEndpointCadastraTarefa
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveRetornar200()
        {
            //arrange
            var mokLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;

            var contexto = new DbTarefasContext(options);
            contexto.Categorias.Add(new Categoria(20,"Estudo"));
            contexto.SaveChanges();

            var repo = new RepositorioTarefa(contexto);

            var controlador = new TarefasController(repo,mokLogger.Object);

            var model = new CadastraTarefaVM()
            {
                IdCategoria = 20,
                Titulo= "Estudar Xunit",
                Prazo = new DateTime(2019,12,31)
            };
            //act
            var retorno = controlador.EndpointCadastraTarefa(model);
            //assert
            Assert.IsType<OkResult>(retorno); //200
        }

        [Fact]
        public void QuandoExcecaoLancadaDeveRetorna500()
        {
            //arrange
            var mokLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var mok = new Mock<IRepositorioTarefas>();
            mok.Setup(r => r.ObtemCategoriaPorId(20)).Returns(new Categoria(20,"Estudo"));
            mok.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa>())).Throws(new Exception("Houve um Erro"));
            var repo = mok.Object;
            var controlador = new TarefasController(repo, mokLogger.Object);

            var model = new CadastraTarefaVM()
            {
                IdCategoria = 20,
                Titulo = "Estudar Xunit",
                Prazo = new DateTime(2019, 12, 31)
            };
            //act
            var retorno = controlador.EndpointCadastraTarefa(model);
            //assert
            Assert.IsType<StatusCodeResult>(retorno); //400
            var statusCodeRetornado = (retorno as StatusCodeResult).StatusCode;
            Assert.Equal(500,statusCodeRetornado);
        }
    }
}

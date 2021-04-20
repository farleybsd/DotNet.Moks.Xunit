using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using Moq;

namespace Alura.CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveIncluirNoBD()
        {
            //arrange
            var comando = new CadastraTarefa("Estudar Xunit",new Categoria("Estudo"),new DateTime(2019,12,31));
            //var repo = new RepositorioFake(); //Dublê para o teste
            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                                                                        .UseInMemoryDatabase("DbTarefasContext")
                                                                        .Options;
                                                                        
            var contexto = new DbTarefasContext(options);
             var repo = new RepositorioTarefa(contexto);

             var handler = new CadastraTarefaHandler(repo);

            //act
            handler.Execute(comando);

            //assert
            var tarefas = repo.ObtemTarefas(t => t.Titulo == "Estudar Xunit").FirstOrDefault();
            Assert.NotNull(tarefas);
        }

        [Fact]
        public void QuandoExceptionForLancadaResultadoIsSucceesDeveSerFalse()
        {
            //arrange
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2019, 12, 31));
            //var repo = new RepositorioFake(); //Dublê para o teste

            var mock = new Mock<IRepositorioTarefas>(); 

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))// parametro com os dados para o metodo IncluirTarefas
                .Throws(new Exception("Houve Um Eroo na Inclusao de Tarefas"));

            var repo = mock.Object;
            var handler = new CadastraTarefaHandler(repo);

            //act
          CommandResult resultado = handler.Execute(comando);

            //assert
            Assert.False(resultado.IsSuccess);
        }

        [Fact]
        public void QuandoExeceptionForLancadaDeveLogarAMensagemDaExcecao()
        {
            //arrange
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2019, 12, 31));
            //var repo = new RepositorioFake(); //Dublê para o teste

            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))// parametro com os dados para o metodo IncluirTarefas
                .Throws(new Exception("Houve Um Eroo na Inclusao de Tarefas"));

            var repo = mock.Object;
            var handler = new CadastraTarefaHandler(repo);

            //act
            CommandResult resultado = handler.Execute(comando);

            //Assert

        }
    }
}

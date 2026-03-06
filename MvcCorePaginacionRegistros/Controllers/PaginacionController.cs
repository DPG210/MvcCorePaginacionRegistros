using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;
        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> RegistroVistaDepartamento(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            //PRIMERO = 1
            //ULTIMO = 4
            //ANTERIOS = posicion-1
            //SIGUIENTE = posicion+1
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
               siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ULTIMO"] = numRegistros;
            ViewData["ANTERIOR"] = anterior;
            VistaDepartamento departamento =
                await this.repo.GetVistaDepartamentoAsync(posicion.Value);
            return View(departamento);
        }
        public async Task<IActionResult> GrupoVistaDepartamento(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            //LO SIGUIENTE SERA QUE DEBEMOS DIBUJAR LOS NUMEROS DE PAGINAS EN LOS LINKS
            //<a href='grupodepts?posicion=1'>Pagina 1</a>
            //<a href='grupodepts?posicion=3'>Pagina 3</a>
            //<a href='grupodepts?posicion=5'>Pagina 5</a>
            //NECESITAMOS UNA VARIABLE PARA EL NUMERO DE PAGINA
            // VOY A REALIZAR EL DIBUJO DESDE AQUI, NO DESDE RAZOR
            int numPagina = 1;
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();

            
            ViewData["POSICION_ACTUAL"] = numPagina;
            ViewData["NUMEROREGISTROS"] = numRegistros;
            
            List<VistaDepartamento> departamentos =
               await this.repo.GetVistaDepartamentosEnGrupoAsync(posicion.Value);
            return View(departamentos);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

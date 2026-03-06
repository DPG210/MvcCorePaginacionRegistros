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
        public IActionResult Index()
        {
            return View();
        }
    }
}

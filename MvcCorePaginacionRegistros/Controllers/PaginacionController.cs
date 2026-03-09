using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
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

            
            //ViewData["POSICION_ACTUAL"] = numPagina;
            ViewData["NUMEROREGISTROS"] = numRegistros;
            
            List<VistaDepartamento> departamentos =
               await this.repo.GetVistaDepartamentosEnGrupoAsync(posicion.Value);
            return View(departamentos);
        }
        public async Task<IActionResult> GrupoDepartamentos(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
     
            
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();


            ViewData["NUMEROREGISTROS"] = numRegistros;

            List<Departamento> departamentos =
               await this.repo.GetGrupoDepartamentosAsync(posicion.Value);
            return View(departamentos);
        }
        public async Task<IActionResult> GrupoEmpleados(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetEmpleadosCountAsync();
            ViewData["REGISTROS"] = numRegistros;
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosAsync(posicion.Value);
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosOficio(int? posicion, string oficio)
        {
            if(posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(oficio, posicion.Value);
                int registros = await this.repo.GetEmpleadosOficiosCount(oficio);
                ViewData["REGISTROS"] = registros;
                ViewData["OFICIO"] = oficio;
                return View(empleados);
            }
                
        }
        [HttpPost]
        public async Task<IActionResult> EmpleadosOficio( string oficio)
        {
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(oficio, 1);
            int registros = await this.repo.GetEmpleadosOficiosCount(oficio);
            ViewData["REGISTROS"] = registros;
            ViewData["OFICIO"] = oficio;
            return View(empleados);
        }
        public async Task<IActionResult> EmpleadosOficioOut(int? posicion, string oficio)
        {
            
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                ModelEmpleadosOficio model = await this.repo.GetGrupoEmpleadosOficioOutAsync(oficio, posicion.Value);
                
                ViewData["REGISTROS"] = model.NumeroRegistros;
                ViewData["OFICIO"] = oficio;
                return View(model.Empleados);
            }

        }
        [HttpPost]
        public async Task<IActionResult> EmpleadosOficioOut(string oficio)
        {
            ModelEmpleadosOficio model = await this.repo.GetGrupoEmpleadosOficioOutAsync(oficio, 1);
            
            ViewData["REGISTROS"] = model.NumeroRegistros;
            ViewData["OFICIO"] = oficio;
            return View(model.Empleados);
        }
        public async Task<IActionResult> DetailsDepartamento(int iddept, int? posicion)
        {
            int numRegistros = await this.repo.EmpleadosCountAsync(iddept);
            
            if(posicion == null)
            {
                posicion = 1;
            }
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
            Departamento dept = await this.repo.FindDepartamento(iddept);
            ModelEmpleadosOficio model = await this.repo.GetEmpleadosDepartamentoAsync(iddept, posicion.Value);
            ViewData["Empleados"] = model;
            return View(dept);
            
        }
        [HttpPost]
        public async Task<IActionResult> DetailsDepartamento(int iddept)
        {
            int numRegistros = await this.repo.EmpleadosCountAsync(iddept);


            Departamento dept = await this.repo.FindDepartamento(iddept);
            ModelEmpleadosOficio model = await this.repo.GetEmpleadosDepartamentoAsync(iddept, 1);
            ViewData["Empleados"] = model;
            return View(dept);
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> DetailsDepartamentoV2(int iddept, int? posicion)
        {
            int pActual = posicion ?? 1;

            // Llamamos al repositorio que usa Skip() y Take()
            ModelEmpleadosOficio model = await this.repo.GetEmpleadosDepartamentoAsync(iddept, pActual);

            // Obtenemos el departamento para los detalles de la cabecera
            Departamento dept = await this.repo.FindDepartamento(iddept);

            // Configuramos la navegación para los botones
            ViewData["ULTIMO"] = model.NumeroRegistros;
            ViewData["SIGUIENTE"] = Math.Min(pActual + 1, model.NumeroRegistros);
            ViewData["ANTERIOR"] = Math.Max(pActual - 1, 1);
            ViewData["POSICION_ACTUAL"] = pActual;
            ViewData["Empleados"] = model;

            return View(dept);
        }
    }
}

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;

namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    #region Vistas
    /*
        alter view V_Departamentos_Individual
        as
        select cast(
        ROW_NUMBER() over (order by dept_NO) as int) 
        as posicion
        , dept_no, dnombre, loc from DEPT
        go*/
    #endregion
    #region STORED PROCEDURE
    /*
     create procedure SP_GRUPO_DEPARTAMENTOS
        (@posicion int)
        as
        select dept_no, dnombre, loc from V_Departamentos_Individual
        where posicion>= @posicion and posicion <(@posicion +2)
        go
     */
    #endregion
    {
        private HospitalContext context;
        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<int> GetNumeroRegistrosVistaDepartamentosAsync()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }
        public async Task<VistaDepartamento> GetVistaDepartamentoAsync(int posicion)
        {

            VistaDepartamento departamento =
                await this.context.VistaDepartamentos
                .Where(z => z.Posicion == posicion)
                .FirstOrDefaultAsync();
            return departamento;
        }
        public async Task<List<VistaDepartamento>> GetVistaDepartamentosEnGrupoAsync(int posicion)
        {
            //select * from v_departamentos_individual where posicion>=1 and posicion<(1+2)
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= posicion
                           && datos.Posicion < (posicion + 2)
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<List<Departamento>> GetGrupoDepartamentosAsync (int posicion)
        {
            string sql = "sp_grupo_departamentos @posicion";
            SqlParameter pamPos = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Departamentos.FromSqlRaw(sql, pamPos);
            return await consulta.ToListAsync();
        }
    }
}

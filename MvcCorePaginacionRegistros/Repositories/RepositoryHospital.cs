using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System.Data;

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
        go*
    create view V_GRUPO_EMPLEADOS
        AS
	        SELECT CAST(ROW_NUMBER() OVER (ORDER BY APELLIDO)AS INT)
	        AS POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	        FROM EMP
        GO*/
    #endregion
    #region STORED PROCEDURE
    /*
     create procedure SP_GRUPO_DEPARTAMENTOS
        (@posicion int)
        as
        select dept_no, dnombre, loc from V_Departamentos_Individual
        where posicion>= @posicion and posicion <(@posicion +2)
        go
    CREATE PROCEDURE SP_GRUPO_EMPLEADOS
    (@POSICION INT)
    AS
	    SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	    FROM V_GRUPO_EMPLEADOS
	    WHERE POSICION >= @POSICION AND POSICION < (@POSICION + 3)
    GO
    alter procedure SP_GRUPO_EMPLEADOS_OFICIO
(@posicion int, @oficio nvarchar(50))
as
SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from 
(select cast(row_number() over (order by apellido) as int)
as posicion,
EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	FROM EMP
	where OFICIO= @oficio) query
	where (query.POSICION >= @posicion AND POSICION < (@posicion + 3))
	go
    Añadir registros
    alter procedure SP_GRUPO_EMPLEADOS_OFICIO
(@posicion int, @oficio nvarchar(50), @registros int out)
as
	select @registros= count(EMP_NO) from emp
	where OFICIO = @oficio
	SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO 
	from 
	(select cast(row_number() over (order by apellido) as int)
	as posicion,EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	FROM EMP
	where OFICIO= @oficio) query
	where (query.POSICION >= @posicion AND POSICION < (@posicion + 3))
go
    alter procedure SP_GRUPO_EMPLEADOS_DEPARTAMENTO
(@posicion int, @departamento int, @registros int out)
as
	select @registros= count(EMP_NO) from emp
	where DEPT_NO = @departamento
	SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO 
	from 
	(select cast(row_number() over (order by apellido) as int)
	as posicion,EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	FROM EMP
	where DEPT_NO= @departamento) query
	where (query.POSICION = @posicion )
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
        public async Task<int> GetEmpleadosCountAsync()
        {
            return await this.context.Empleados.CountAsync();
        }
        public async Task<List<Empleado>> GetGrupoEmpleadosAsync(int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }
        public async Task<int> GetEmpleadosOficiosCount(string oficio)
        {
            return await this.context.Empleados
                .Where(z => z.Oficio == oficio).CountAsync();
        }
        public async Task<List<Empleado>> GetGrupoEmpleadosOficioAsync(string oficio, int posicion)
        {
            string sql = "sp_grupo_empleados_oficio @posicion,@oficio";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            var consulta= this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio);
            return await consulta.ToListAsync();
        }

        public async Task<ModelEmpleadosOficio> GetGrupoEmpleadosOficioOutAsync(string oficio, int posicion)
        {
            string sql = "sp_grupo_empleados_oficio @posicion,@oficio, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            SqlParameter pamReg = new SqlParameter("@registros", 0);
            pamReg.DbType = DbType.Int32;
            pamReg.Direction = ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio, pamReg);

            List<Empleado> empleados = await consulta.ToListAsync();
            //HASTA QUE NO HEMOS EXTRAIDO LOS DATOS (Empleados)
            // NO SE LIBERAN LOS PARAMETROS DE SALIDA
            int registros = (int)pamReg.Value;
            return new ModelEmpleadosOficio
            {
                Empleados = empleados,
                NumeroRegistros = registros
            };
        }
        public List<Departamento> GetDepartamentos()
        {
            return  this.context.Departamentos.ToList();
        }
        public async Task<Departamento> FindDepartamento(int idDepartamento)
        {
            return await this.context.Departamentos.Where(z => z.IdDepartamento == idDepartamento).FirstOrDefaultAsync();
        }
        public async Task<int> EmpleadosCountAsync(int idDepartamento)
        {
            return await this.context.Empleados.Where(z => z.IdDepartamento == idDepartamento).CountAsync();
        }
        public async Task<ModelEmpleadosOficio> GetEmpleadosDepartamentoAsync(int idDepartamento, int posicion)
        {
            string sql = "sp_grupo_empleados_departamento @posicion,@departamento, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamDept = new SqlParameter("@departamento", idDepartamento);
            SqlParameter pamReg = new SqlParameter("@registros", 0);
            pamReg.DbType = DbType.Int32;
            pamReg.Direction = ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamDept, pamReg);

            List<Empleado> empleados = await consulta.ToListAsync();
            //HASTA QUE NO HEMOS EXTRAIDO LOS DATOS (Empleados)
            // NO SE LIBERAN LOS PARAMETROS DE SALIDA
            int registros = (int)pamReg.Value;
            return new ModelEmpleadosOficio
            {
                Empleados = empleados,
                NumeroRegistros = registros
            };
        }
        public async Task<ModelEmpleadosOficio> GetEmpleadosDepartamentoEFAsync(int iddept, int posicion)
        {
            int numeroRegistros = await this.context.Empleados
                .Where(x => x.IdDepartamento == iddept)
                .CountAsync();

            List<Empleado> empleados = await this.context.Empleados
                .Where(x => x.IdDepartamento == iddept)
                .OrderBy(x => x.IdEmpleado) 
                .Skip(posicion - 1)
                .Take(1)
                .ToListAsync();

            ModelEmpleadosOficio model = new ModelEmpleadosOficio
            {
                NumeroRegistros = numeroRegistros,
                Empleados = empleados
            };

            return model;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para préstamos de profesores.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLTeacher
    {
        // Crea un registro de préstamo para profesor.
        public async Task<long> CreateTeacherAsync(LoansTeacher pTeacher)
        {
            return await DALLoansTeacher.CreateTeacherAsync(pTeacher);
        }

        // Obtiene un préstamo de profesor por id.
        public async Task<LoansTeacher> GetLoanTeacherByIdAsync(long id)
        {
            return await DALLoansTeacher.GetLoanTeacherByIdAsync(id);
        }

        // Actualiza un préstamo de profesor.
        public async Task<int> UpdateLoansTeacherAsync(LoansTeacher pLoan)
        {
            return await DALLoansTeacher.UpdateLoansTeacherAsync(pLoan);
        }

        // Variante de actualización (se mantiene la firma existente).
        public async Task<int> UpdateLoansTeacher02Async(LoansTeacher pLoan)
        {
            return await DALLoansTeacher.UpdateLoansTeacherAsync(pLoan);
        }

        // Elimina un registro de préstamo de profesor.
        public async Task<int> DeleteTeacherAsync(LoansTeacher pTeacher)
        {
            return await DALLoansTeacher.DeleteTeacherAsync(pTeacher);
        }

        // Obtiene todos los préstamos de profesores.
        public async Task<List<LoansTeacher>> GetAllTeacherAsync()
        {
            return await DALLoansTeacher.GetAllTeacherAsync();
        }

        // Consulta préstamos de profesores según criterios en el objeto.
        public async Task<List<LoansTeacher>> GetTeacherAsync(LoansTeacher pTeacher)
        {
            return await DALLoansTeacher.GetTeacherAsync(pTeacher);
        }

        // Obtiene préstamos incluyendo propiedades relacionadas.
        public async Task<List<LoansTeacher>> GetIncludePropertiesAsync(LoansTeacher pTeacher)
        {
            return await DALLoansTeacher.GetIncludePropertiesAsync(pTeacher);
        }

        // Obtiene el último préstamo según criterios.
        public async Task<LoansTeacher> GetLastTeacher(LoansTeacher pTeacher)
        {
            return await DALLoansTeacher.GetLastTeacher(pTeacher);
        }

        // Obtiene préstamos por id del prestamista.
        public async Task<List<LoansTeacher>> GetTeacherByIdLender(long IdLender)
        {
            return await DALLoansTeacher.GetTeacherByIdLender(IdLender);
        }

        // Obtiene préstamos vencidos para un prestamista.
        public async Task<List<LoansTeacher>> GetExpiredLoansByIdLenderAsync(LoansTeacher pTeacher)
        {
            return await DALLoansTeacher.GetExpiredTeacherByIdLenderAsync(pTeacher);
        }

        // Obtiene préstamos vencidos a partir de una lista de fechas vencidas.
        public async Task<List<LoansTeacher>> GetExpiredTeacherAsync(List<LoanDates2> loanDates)
        {
            return await DALLoansTeacher.GetExpiredTeacherAsync(loanDates);
        }

        // Obtiene un profesor por su email.
        public async Task<LoansTeacher> GetTeacherByEmailAsync(string email)
        {
            return await DALLoansTeacher.GetTeacherByEmailAsync(email);
        }

        // Nota: se eliminó el método que usaba Teacher2 para evitar errores de compilación.
    }
}
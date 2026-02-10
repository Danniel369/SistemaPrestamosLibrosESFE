using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para Loans.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLLoans
    {
        // Crea un préstamo.
        public async Task<int> CreateLoansAsync(Loans pLoans)
        {
            return await DALLoans.CreateLoansAsync(pLoans);
        }

        // Actualiza un préstamo.
        public async Task<int> UpdateLoansAsync(Loans pLoans)
        {
            return await DALLoans.UpdateLoansAsync(pLoans);
        }

        // Actualiza un préstamo usando el DTO Loans2.
        public async Task<int> UpdateLoans02Async(Loans2 pLoans)
        {
            return await DALLoans.UpdateLoans02Async(pLoans);
        }

        // Elimina un préstamo.
        public async Task<int> DeleteLoansAsync(Loans pLoans)
        {
            return await DALLoans.DeleteLoansAsync(pLoans);
        }

        // Obtiene un préstamo por id (u otros campos que use el DAL).
        public async Task<Loans> GetLoansByIdAsync(Loans pLoans)
        {
            return await DALLoans.GetLoansByIdAsync(pLoans);
        }

        // Obtiene todos los préstamos.
        public async Task<List<Loans>> GetAllLoansAsync()
        {
            return await DALLoans.GetAllLoansAsync();
        }

        // Consulta préstamos según criterios en el objeto.
        public async Task<List<Loans>> GetLoansAsync(Loans pLoans)
        {
            return await DALLoans.GetLoansAsync(pLoans);
        }

        // Obtiene préstamos incluyendo propiedades relacionadas.
        public async Task<List<Loans>> GetIncludePropertiesAsync(Loans pLoans)
        {
            return await DALLoans.GetIncludePropertiesAsync(pLoans);
        }

        // Obtiene el último préstamo según los criterios.
        public async Task<Loans> GetLastLoan(Loans pLoans)
        {
            return await DALLoans.GetLastLoan(pLoans);
        }

        // Obtiene préstamos por id del prestamista.
        public async Task<List<Loans>> GetLoanByIdLender(long IdLender)
        {
            return await DALLoans.GetLoanByIdLender(IdLender);
        }

        // Obtiene préstamos vencidos para un prestamista (según filtro).
        public async Task<List<Loans>> GetExpiredLoansByIdLenderAsync(Loans pLoan)
        {
            return await DALLoans.GetExpiredLoansByIdLenderAsync(pLoan);
        }

        // Obtiene préstamos vencidos a partir de una lista de fechas vencidas.
        public async Task<List<Loans>> GetExpiredLoansAsync(List<LoanDates2> loanDates)
        {
            return await DALLoans.GetExpiredLoansAsync(loanDates);
        }
    }
}

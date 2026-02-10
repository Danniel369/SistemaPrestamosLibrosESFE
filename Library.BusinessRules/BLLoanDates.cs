using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para LoanDates.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLLoanDates
    {
        // Crea una fecha de préstamo.
        public async Task<int> CreateLoanDatesAsync(LoanDates pLoanDates)
        {
            return await DALLoanDates.CreateLoandDateAsync(pLoanDates);
        }

        // Actualiza una fecha de préstamo.
        public async Task<int> UpdateLoanDatesAsync(LoanDates pLoandDates)
        {
            return await DALLoanDates.UpdateLoandDatesAsync(pLoandDates);
        }

        // Obtiene una fecha de préstamo por id (nota: nombre original del método conserva su firma).
        public async Task<LoanDates> GetLoanDatessByIdAsync(LoanDates pLoanDates)
        {
            return await DALLoanDates.GetLoanDatesByIdAsync(pLoanDates);
        }

        // Obtiene las fechas (tipo LoanDates2) asociadas a un préstamo por id de préstamo.
        public async Task<List<LoanDates2>> GetLoanDatesByIdLoanAsync(LoanDates pLoanDates)
        {
            return await DALLoanDates.GetLoanDatesByIdLoanAsync(pLoanDates);
        }

        // Obtiene todas las fechas de préstamo.
        public async Task<List<LoanDates>> GetAllLoanDatesAsync()
        {
            return await DALLoanDates.GetAllLoanDatesAsync();
        }

        // Obtiene las fechas vencidas (tipo LoanDates2).
        public async Task<List<LoanDates2>> GetExpiredDatesAsync()
        {
            return await DALLoanDates.GetExpiredDatesAsync();
        }

        // Obtiene las fechas vencidas filtradas por id de préstamo.
        public async Task<List<LoanDates2>> GetExpiredDatesByIdLoanAsync(Loans pLoan)
        {
            return await DALLoanDates.GetExpiredDatesByIdLoanAsync(pLoan);
        }

        // Obtiene las fechas que vencen próximamente.
        public async Task<List<LoanDates2>> GetDatesToExpireSoonAsync()
        {
            return await DALLoanDates.GetDatesToExpireSoonAsync();
        }
    }
}

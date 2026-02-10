using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para LoanTypes.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLLoanTypes
    {
        // Crea un tipo de préstamo.
        public async Task<int> CreateLoanTypesAsync(LoanTypes pLoanTypes)
        {
            return await DALLoanTypes.CreateLoanTypesAsync(pLoanTypes);
        }

        // Actualiza un tipo de préstamo.
        public async Task<int> UpdateLoanTypesAsync(LoanTypes pLoanTypes)
        {
            return await DALLoanTypes.UpdateLoanTypesAsync(pLoanTypes);
        }

        // Elimina un tipo de préstamo.
        public async Task<int> DeleteLoanTypesAsync(LoanTypes pLoanTypes)
        {
            return await DALLoanTypes.DeleteLoanTypesAsync(pLoanTypes);
        }

        // Obtiene todos los tipos de préstamo.
        public async Task<List<LoanTypes>> GetAllLoanTypesAsync()
        {
            return await DALLoanTypes.GetAllLoanTypesAsync();
        }

        // Obtiene un tipo de préstamo por id (u otros campos que use el DAL).
        public async Task<LoanTypes> GetLoanTypesByIdAsync(LoanTypes pLoanTypes)
        {
            return await DALLoanTypes.GetLoanTypesByIdAsync(pLoanTypes);
        }

        // Consulta tipos de préstamo según criterios en el objeto.
        public async Task<List<LoanTypes>> GetLoanTypesAsync(LoanTypes pLoanTypes)
        {
            return await DALLoanTypes.GetLoanTypesAsync(pLoanTypes);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para Countries.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLCountries
    {
        // Crea un país.
        public async Task<int> CreateCountriesAsync(Countries pCountries)
        {
            return await DALCountries.CreateCountriesAsync(pCountries);
        }

        // Actualiza un país.
        public async Task<int> UpdateCountriesAsync(Countries pCountries)
        {
            return await DALCountries.UpdateCountriesAsync(pCountries);
        }

        // Elimina un país.
        public async Task<int> DeleteCountriesAsync(Countries pCountries)
        {
            return await DALCountries.DeleteCountriesAsync(pCountries);
        }

        // Obtiene todos los países.
        public async Task<List<Countries>> GetAllCountriesAsync()
        {
            return await DALCountries.GetAllCountriesAsync();
        }

        // Obtiene un país por id (u otros campos que use el DAL).
        public async Task<Countries> GetCountriesByIdAsync(Countries pCountries)
        {
            return await DALCountries.GetCountriesByIdAsync(pCountries);
        }

        // Consulta países según criterios en el objeto.
        public async Task<List<Countries>> GetCountriesAsync(Countries pCountries)
        {
            return await DALCountries.GetCountriesAsync(pCountries);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para Catalogs.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLCatalogs
    {
        // Crea un catálogo.
        public async Task<int> CreateCatalogsAsync(Catalogs pCatalogs)
        {
            return await DALCatalogs.CreateCatalogsAsync(pCatalogs);
        }

        // Actualiza un catálogo.
        public async Task<int> UpdateCatalogsAsync(Catalogs pCatalogs)
        {
            return await DALCatalogs.UpdateCatalogsAsync(pCatalogs);
        }

        // Elimina un catálogo.
        public async Task<int> DeleteCatalogsAsync(Catalogs pCatalogs)
        {
            return await DALCatalogs.DeleteCatalogsAsync(pCatalogs);
        }

        // Obtiene todos los catálogos.
        public async Task<List<Catalogs>> GetAllCatalogsAsync()
        {
            return await DALCatalogs.GetAllCatalogsAsync();
        }

        // Obtiene un catálogo por id (u otros campos que use el DAL).
        public async Task<Catalogs> GetCatalogsByIdAsync(Catalogs pCatalogs)
        {
            return await DALCatalogs.GetCatalogsByIdAsync(pCatalogs);
        }

        // Consulta catálogos según criterios en el objeto.
        public async Task<List<Catalogs>> GetCatalogsAsync(Catalogs pCatalogs)
        {
            return await DALCatalogs.GetCatalogsAsync(pCatalogs);
        }
    }
}

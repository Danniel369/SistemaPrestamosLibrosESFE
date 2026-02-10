using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para Categories.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLCategories
    {
        // Crea una categoría.
        public async Task<int> CreateCategoriesAsync(Categories pCategories)
        {
            return await DALCategories.CreateCategoriesAsync(pCategories);
        }

        // Actualiza una categoría.
        public async Task<int> UpdateCategoriesAsync(Categories pCategories)
        {
            return await DALCategories.UpdateCategoriesAsync(pCategories);
        }

        // Elimina una categoría.
        public async Task<int> DeleteCategoriesAsync(Categories pCategories)
        {
            return await DALCategories.DeleteCategoriesAsync(pCategories);
        }

        // Obtiene todas las categorías.
        public async Task<List<Categories>> GetAllCategoriesAsync()
        {
            return await DALCategories.GetAllCategoriesAsync();
        }

        // Obtiene una categoría por id (u otros campos que use el DAL).
        public async Task<Categories> GetCategoriesByIdAsync(Categories pCategories)
        {
            return await DALCategories.GetCategoriesByIdAsync(pCategories);
        }

        // Consulta categorías según criterios en el objeto.
        public async Task<List<Categories>> GetCategoriesAsync(Categories pCategories)
        {
            return await DALCategories.GetCategoriesAsync(pCategories);
        }
    }
}

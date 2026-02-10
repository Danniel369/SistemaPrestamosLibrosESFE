using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para Editorials.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLEditorials
    {
        // Crea una editorial.
        public async Task<int> CreateEditorialsAsync(Editorials pEditorials)
        {
            return await DALEditorials.CreateEditorialsAsync(pEditorials);
        }

        // Actualiza una editorial.
        public async Task<int> UpdateEditorialsAsync(Editorials pEditorials)
        {
            return await DALEditorials.UpdateEditorialsAsync(pEditorials);
        }

        // Elimina una editorial.
        public async Task<int> DeleteEditorialsAsync(Editorials pEditorials)
        {
            return await DALEditorials.DeleteEditorialsAsync(pEditorials);
        }

        // Obtiene todas las editoriales.
        public async Task<List<Editorials>> GetAllEditorialsAsync()
        {
            return await DALEditorials.GetAllEditorialsAsync();
        }

        // Obtiene una editorial por id (u otros campos que use el DAL).
        public async Task<Editorials> GetEditorialsByIdAsync(Editorials pEditorials)
        {
            return await DALEditorials.GetEditorialsByIdAsync(pEditorials);
        }

        // Consulta editoriales según criterios en el objeto.
        public async Task<List<Editorials>> GetEditorialsAsync(Editorials pEditorials)
        {
            return await DALEditorials.GetEditorialsAsync(pEditorials);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para Authors.
    // Actualmente actúa como fachada que delega las operaciones al DAL.
    public class BLAuthors
    {
        // Crea un autor.
        public async Task<int> CreateAuthorsAsync(Authors pAuthors)
        {
            return await DALAuthors.CreateAuthorsAsync(pAuthors);
        }

        // Actualiza un autor.
        public async Task<int> UpdateAuthorsAsync(Authors pAuthors)
        {
            return await DALAuthors.UpdateAuthorsAsync(pAuthors);
        }

        // Elimina un autor.
        public async Task<int> DeleteAuthorsAsync(Authors pAuthors)
        {
            return await DALAuthors.DeleteAuthorsAsync(pAuthors);
        }

        // Obtiene todos los autores.
        public async Task<List<Authors>> GetAlAuthorsAsync()
        {
            return await DALAuthors.GetAllAuthorsAsync();
        }

        // Obtiene un autor por id (u otros campos que use el DAL).
        public async Task<Authors> GetAuthorsByIdAsync(Authors pAuthors)
        {
            return await DALAuthors.GetAuthorsByIdAsync(pAuthors);
        }

        // Consulta autores según criterios en el objeto.
        public async Task<List<Authors>> GetAuthorsAsync(Authors pAuthors)
        {
            return await DALAuthors.GetAuthorsAsync(pAuthors);
        }
    }
}

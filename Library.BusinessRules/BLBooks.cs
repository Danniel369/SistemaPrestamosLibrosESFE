using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para Books.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLBooks
    {
        // Crea un libro.
        public async Task<int> CrearAsync(Books pBooks)
        {
            return await DALBooks.CreateBooksAsync(pBooks);
        }

        // Actualiza un libro.
        public async Task<int> UpdateBooksAsync(Books pBooks)
        {
            return await DALBooks.UpdateBooksAsync(pBooks);
        }

        // Actualiza las existencias (cantidad) de un libro (usa Books2 como DTO).
        public async Task<int> UpdateExistencesBooksAsync(Books2 pBooks)
        {
            return await DALBooks.UpdateExistencesBooksAsync(pBooks);
        }

        // Elimina un libro.
        public async Task<int> DeleteBooksAsync(Books pBooks)
        {
            return await DALBooks.DeleteBooksAsync(pBooks);
        }

        // Obtiene un libro por id (u otros campos que use el DAL).
        public async Task<Books> GetBooksByIdAsync(Books pBooks)
        {
            return await DALBooks.GetBooksByIdAsync(pBooks);
        }

        // Obtiene todos los libros.
        public async Task<List<Books>> GetAllBooksAsync()
        {
            return await DALBooks.GetAllBooksAsync();
        }

        // Consulta libros según criterios en el objeto.
        public async Task<List<Books>> GetBooksAsync(Books pBooks)
        {
            return await DALBooks.GetBooksAsync(pBooks);
        }

        // Obtiene libros incluyendo propiedades relacionadas (ej. autor, categoría).
        public async Task<List<Books>> GetIncludePropertiesAsync(Books pBooks)
        {
            return await DALBooks.GetIncludePropertiesAsync(pBooks);
        }

        // Busca libros por título (o parte del título).
        public async Task<List<Books>> BuscarPorTituloAsync(string pTitulo)
        {
            return await DALBooks.GetBooksByTitleAsync(pTitulo);
        }

        // Devuelve una página de libros y el total de registros que coinciden con el filtro.
        // page: número de página (1-based). pageSize: registros por página.
        public async Task<(List<Books> Books, int TotalRecords)> GetPaginatedBooksAsync(Books pBooks, int page = 1, int pageSize = 12)
        {
            // Recupera todos los registros con propiedades incluidas según filtros.
            var allBooks = await GetIncludePropertiesAsync(pBooks);
            int totalRecords = allBooks.Count;

            // Ajusta el auxiliar de top según el tamaño de página (si el DAL lo usa).
            pBooks.Top_Aux = pageSize;

            // Paginación en memoria: Skip/Take sobre la lista ya obtenida.
            var paginatedBooks = allBooks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (paginatedBooks, totalRecords);
        }
    }
}

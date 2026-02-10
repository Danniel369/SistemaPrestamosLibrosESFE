using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    // Capa de reglas de negocio para PostsDocs.
    // Actúa como fachada que delega las operaciones al DAL.
    public class BLPostDocs
    {
        // Crea un documento asociado a un post.
        public async Task<int> CreatePostDocAsync(PostsDocs pPostDocs)
        {
            return await DALPostsDocs.CreatePostDocAsync(pPostDocs);
        }

        // Actualiza un documento de post.
        public async Task<int> UpdatePostDocAsync(PostsDocs pPostDocs)
        {
            return await DALPostsDocs.UpdatePostDocAsync(pPostDocs);
        }

        // Elimina un documento de post.
        public async Task<int> DeletePostDocAsync(PostsDocs pPostDocs)
        {
            return await DALPostsDocs.DeletePostDocAsync(pPostDocs);
        }

        // Elimina todos los documentos de un post por su id.
        public async Task<int> DeleteAllPostsDocsByIdPostAsync(Posts pPosts)
        {
            return await DALPostsDocs.DeleteAllPostsDocsByIdPostAsync(pPosts);
        }

        // Obtiene un documento por su id.
        public async Task<PostsDocs> GetPostDocsByIdAsync(PostsDocs pPostDocs)
        {
            return await DALPostsDocs.GetPostDocsByIdAsync(pPostDocs);
        }

        // Obtiene todos los documentos asociados a un post (por id del post).
        public async Task<List<PostsDocs>> GetPostDocsByPostIdAsync(Posts pPosts)
        {
            return await DALPostsDocs.GetPostDocsByPostIdAsync(pPosts);
        }

        // Obtiene todos los documentos de posts.
        public async Task<List<PostsDocs>> GetAllPostDocsAsync()
        {
            return await DALPostsDocs.GetAllPostDocsAsync();
        }

        // Consulta documentos según criterios en el objeto.
        public async Task<List<PostsDocs>> GetPostDocsAsync(PostsDocs pPostDocs)
        {
            return await DALPostsDocs.GetPostDocsAsync(pPostDocs);
        }
    }
}

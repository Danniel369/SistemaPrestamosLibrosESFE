using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules;

public class BLPinnedPosts
{
    // Instancia del repositorio/ DAL para pinned posts.
    DALPinnedPosts DALPinnedPosts = new DALPinnedPosts();

    // Obtiene el pinned post asociado a un post (por id del post).
    public async Task<PinnedPosts> GetPinnedPostByIdPost(Posts posts)
    {
        return await DALPinnedPosts.GetPinnedPostByPostId(posts);
    }

    // Comprueba si un post está fijado (pinned).
    public async Task<bool> IsPostPinned(Posts posts)
    {
        return await DALPinnedPosts.IsPostPinned(posts);
    }

    // Crea un pinned post.
    public async Task<bool> CreatePinnedPost(PinnedPosts pinnedPost)
    {
        return await DALPinnedPosts.CreatePinnedPost(pinnedPost);
    }

    // Elimina un pinned post.
    public async Task<bool> DeletePinnedPost(PinnedPosts pinnedPosts)
    {
        return await DALPinnedPosts.DeletePinnedPost(pinnedPosts);
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class AuthorsController : Controller
    {
        BLAuthors authorsBL = new BLAuthors();

        // GET: CategoriesController
        public async Task<IActionResult> Index(Authors pAuthors = null, int page = 1, int pageSize = 5)
        {
            if (pAuthors == null)
                pAuthors = new Authors();

            if (pAuthors.Top_Aux == -1)
                pAuthors.Top_Aux = 0;

            var allAuthors = await authorsBL.GetAuthorsAsync(pAuthors);
            allAuthors = allAuthors.OrderBy(a => a.AUTHOR_ID).ToList();

            // Aplica la paginación manualmente
            int totalRegistros = allAuthors.Count();
            int totalPaginas = totalRegistros > 0 ? (int)Math.Ceiling((double)totalRegistros / pageSize) : 1;

            var authors = allAuthors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;
            ViewBag.Top = pageSize;
            ViewBag.ShowMenu = true;

            return View(authors);
        }


        // GET: CategoriesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = id });
            ViewBag.ShowMenu = true;
            return View(authors);
        }

        // GET: CategoriesController/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.ShowMenu = true;
            return View();
        }

        // POST: CategoriesController/Create
        //Metodo para crear un nuevo autor mediante el metodo regular y por peticion AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Authors pAuthors)
        {
            try
            {
                int result = await authorsBL.CreateAuthorsAsync(pAuthors);

                // revisa si la peticion es un AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    if (result > 0)
                    {
                        return Json(new { success = true, authorId = pAuthors.AUTHOR_ID, authorName = pAuthors.AUTHOR_NAME });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Ocurrio un error en el guardado de Autor" });
                    }
                }
                else
                {
                    // seguimineot regular para las peticiones que no seas AJAX
                    TempData["CreateSuccess"] = true;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ee)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ee.Message });
                }
                else
                {
                    ViewBag.Error = ee.Message;
                    return View(pAuthors);
                }
            }
        }



        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = id });
            ViewBag.ShowMenu = true;
            return View(authors);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Authors pAuthors)
        {
            try
            {
                int result = await authorsBL.UpdateAuthorsAsync(pAuthors);
                TempData["EditSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pAuthors);
            }
        }

        // GET: CategoriesController/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = id });
        //    ViewBag.ShowMenu = true;
        //    return View(authors);
        //}

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int result = await authorsBL.DeleteAuthorsAsync(new Authors { AUTHOR_ID = id });
                return Ok(new { success = true, message = "Autor eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string nombre)
        {
            var filtro = new Authors
            {
                AUTHOR_NAME = nombre,
                Top_Aux = 20 // Puedes ajustar el límite de resultados si deseas
            };

            var lista = await authorsBL.GetAuthorsAsync(filtro);

            var resultados = lista.Select(a => new
            {
                id = a.AUTHOR_ID,
                nombre = a.AUTHOR_NAME
            });

            return Json(resultados);
        }

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class EditorialsController : Controller
    {

        BLEditorials editorialsBL = new BLEditorials();
        BLCategories categoriesBL = new BLCategories();
        BLCatalogs catalogsBL = new BLCatalogs();

        public async Task<IActionResult> Index(Editorials pEditorials = null, int page = 1, int top_aux = 5)
        {
            if (pEditorials == null)
                pEditorials = new Editorials();

            // Forzamos al BL a traer todos los registros para paginar nosotros en memoria
            pEditorials.Top_Aux = -1;

            var allEditorials = await editorialsBL.GetEditorialsAsync(pEditorials);

            // Ordenamiento
            allEditorials = allEditorials.OrderBy(e => e.EDITORIAL_ID).ToList();

            // Lógica para manejar "Todos" (-1)
            int actualPageSize = (top_aux == -1) ? (allEditorials.Count > 0 ? allEditorials.Count : 5) : top_aux;

            // Calcular paginación
            int totalRegistros = allEditorials.Count();
            int totalPaginas = totalRegistros > 0 ? (int)Math.Ceiling((double)totalRegistros / actualPageSize) : 1;

            var editorials = allEditorials
                .Skip((page - 1) * actualPageSize)
                .Take(actualPageSize)
                .ToList();

            // Pasar datos a la vista
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;
            ViewBag.Top = top_aux; // Esto mantiene seleccionada la opción en el dropdown
            ViewBag.ShowMenu = true;

            return View(editorials);
        }


        // GET: AcquisitionTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var editorials = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = id });
            ViewBag.ShowMenu = true;
            return View(editorials);
        }

        // GET: CategoriesController/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.ShowMenu = true;
            return View();
        }

        // POST: CategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Editorials pEditorials)
        {
            try
            {
                int result = await editorialsBL.CreateEditorialsAsync(pEditorials);

                // revisa si la peticion es un AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    if (result > 0)
                    {
                        return Json(new { success = true, editoriaL_ID = pEditorials.EDITORIAL_ID, editoriaL_NAME = pEditorials.EDITORIAL_NAME });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Ocurrio un error en el guardado de la Editorial" });
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
                    return View(pEditorials);
                }
            }
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var editions = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = id });
            ViewBag.ShowMenu = false;
            return View(editions);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Editorials pEditorials)
        {
            try
            {
                int result = await editorialsBL.UpdateEditorialsAsync(pEditorials);
                TempData["EditSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pEditorials);
            }
        }

        // GET: CategoriesController/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var editions = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = id });
        //    ViewBag.ShowMenu = true;
        //    return View(editions);
        //}

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int result = await editorialsBL.DeleteEditorialsAsync(new Editorials { EDITORIAL_ID = id });
                return Ok(new { success = true, message = "Editorial eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string nombre)
        {
            var editorial = new Editorials
            {
                EDITORIAL_NAME = nombre,
                Top_Aux = 10
            };

            var lista = await editorialsBL.GetEditorialsAsync(editorial);

            var resultado = lista.Select(x => new {
                editorialId = x.EDITORIAL_ID,
                editorialName = x.EDITORIAL_NAME
            });

            return Json(resultado);
        }

    }
}

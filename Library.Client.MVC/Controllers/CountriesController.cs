using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class CountriesController : Controller
    {
        BLCountries countriesBL = new BLCountries();

        // GET: AcquisitionTypesController
        public async Task<IActionResult> Index(string COUNTRY_NAME, int page = 1, int top_aux = 5)
        {
            // 1. Filtro base para traer datos de la DB
            var filtro = new Countries
            {
                COUNTRY_NAME = COUNTRY_NAME,
                Top_Aux = -1 // Traemos todo para paginar en memoria o filtrar
            };

            var allCountries = await countriesBL.GetCountriesAsync(filtro);

            // 2. Ordenamiento
            allCountries = allCountries.OrderBy(c => c.COUNTRY_ID).ToList();

            // 3. Manejo de la lógica "Todos" (-1)
            // Si top_aux es -1, el tamaño de la página es el total de registros
            int actualPageSize = (top_aux == -1) ? allCountries.Count : top_aux;
            if (actualPageSize <= 0) actualPageSize = 5; // Evitar división por cero si está vacío

            int totalRegistros = allCountries.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / actualPageSize);

            // 4. Paginación con Skip y Take
            var countries = allCountries
                .Skip((page - 1) * actualPageSize)
                .Take(actualPageSize)
                .ToList();

            // 5. Datos para la Vista
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;
            ViewBag.Top = top_aux; // Mantenemos el valor original (5, 10, -1, etc.)
            ViewBag.ShowMenu = true;

            return View(countries);
        }
        // GET: AcquisitionTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var countries = await countriesBL.GetCountriesByIdAsync(new Countries { COUNTRY_ID = id });
            ViewBag.ShowMenu = true;
            return View(countries);
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
        public async Task<ActionResult> Create(Countries pCountries)
        {
            try
            {
                int result = await countriesBL.CreateCountriesAsync(pCountries);

                // revisa si la peticion es un AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    if (result > 0)
                    {
                        return Json(new { success = true, countrY_ID = pCountries.COUNTRY_ID, countrY_NAME = pCountries.COUNTRY_NAME });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Ocurrio un error en el guardado del Pais" });
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
                    return View(pCountries);
                }
            }
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var countries = await countriesBL.GetCountriesByIdAsync(new Countries { COUNTRY_ID = id });
            ViewBag.ShowMenu = true;
            return View(countries);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Countries pCountries)
        {
            try
            {
                int result = await countriesBL.UpdateCountriesAsync(pCountries);
                TempData["EditSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pCountries);
            }
        }

        // GET: CategoriesController/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var countries = await countriesBL.GetCountriesByIdAsync(new Countries { COUNTRY_ID = id });
        //    ViewBag.ShowMenu = true;
        //    return View(countries);
        //}

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int result = await countriesBL.DeleteCountriesAsync(new Countries { COUNTRY_ID = id });
                return Ok(new { success = true, message = "País eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string nombre)
        {
            var filtro = new Countries
            {
                COUNTRY_NAME = nombre,
                Top_Aux = 20 // o el número que prefieras
            };

            var lista = await countriesBL.GetCountriesAsync(filtro);

            var resultados = lista.Select(p => new
            {
                id = p.COUNTRY_ID,
                nombre = p.COUNTRY_NAME
            });

            return Json(resultados);
        }

    }
}

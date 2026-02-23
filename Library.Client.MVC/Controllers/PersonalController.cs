using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain; // Asegura que se pueda acceder al modelo Docentes
using Library.BusinessRules; // Asegura que se pueda acceder a BLDocentes
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class PersonalController : Controller // Nombre del controlador
    {
        //  Usaremos la nueva BL
        private readonly BLPersonal _personalBL;

        // 2. Constructor que recibe la dependencia (inyecta la BL)
        public PersonalController(BLPersonal personalBL)
        {
            _personalBL = personalBL;
        }

        // 1. Mostrar la lista de Docentes con Paginación y Filtro
        // GET: /Docente/Index
        public async Task<IActionResult> Index(Personal pPersonal = null, int page = 1, int pageSize = 5) // ⬅️ Usa modelo Docentes
        {
            if (pPersonal == null)
                pPersonal = new Personal();

            var allPersonal = await _personalBL.GetPersonalAsync(pPersonal); // ⬅️ Usa BLDocentes
            allPersonal = allPersonal.OrderBy(t => t.PERSONAL_ID).ToList(); // ⬅️ Usa Id

            // Aplica la paginación manualmente
            int totalRegistros = allPersonal.Count();
            int totalPaginas = totalRegistros > 0 ? (int)Math.Ceiling((double)totalRegistros / pageSize) : 1;

            var personal = allPersonal
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;
            ViewBag.Top = pageSize;
            ViewBag.ShowMenu = true;

            return View(personal); // ⬅️ Pasa List<Docentes>
        }

        // 2. Muestra los detalles de un Docente
        // GET: /Docente/Details/5
        public async Task<ActionResult> Details(long id)
        {
            var personal = await _personalBL.GetPersonalByIdAsync(new Personal { PERSONAL_ID = id }); // ⬅️ Usa Id y BLDocentes
            ViewBag.ShowMenu = true;
            return View(personal);
        }

        // GET: /Personal/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.ShowMenu = true;
            return View();
        }
        
        // POST: /Personal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Personal pPersonal)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ShowMenu = true;
                return View(pPersonal);
            }

            try
            {
                int result = await _personalBL.CreatePersonalAsync(pPersonal);

                // Si se insertó correctamente
                if (result > 0)
                {
                    TempData["CreateSuccess"] = true;
                    return RedirectToAction("Index");   // <-- AHORA SÍ REDIRIGE
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo guardar el docente.");
                    return View(pPersonal);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.ShowMenu = true;
                return View(pPersonal);
            }
        }

        // 5. Muestra el formulario de edición
        // GET: /Docente/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var personal = await _personalBL.GetPersonalByIdAsync(new Personal { PERSONAL_ID = id }); // ⬅️ Usa Id y BLDocentes
            ViewBag.ShowMenu = true;
            return View(personal);
        }

        // 6. Procesa la edición del Docente
        // POST: /Docente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Personal pPersonal)
        {
            try
            {
                int result = await _personalBL.UpdatePersonalAsync(pPersonal); // ⬅️ Usa BLDocentes
                TempData["EditSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pPersonal);
            }
        }

        // 7. Elimina un Docente
        // POST: /Docente/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                int result = await _personalBL.DeletePersonalAsync(new Personal { PERSONAL_ID = id }); // ⬅️ Usa Id y BLDocentes
                return Ok(new { success = true, message = "Personal eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetPersonalSuggestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new List<string>());

            var filtro = new Personal
            {
                Email = query
            };

            var lista = await _personalBL.GetPersonalAsync(filtro);

            var sugerencias = lista
                .Select(t => t.Email)
                .Distinct()
                .Take(10)
                .ToList();

            return Json(sugerencias);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarDocente(string correo)
        {
            // Filtro por correo (o parte del correo)
            var filtro = new Personal
            {
                Email = correo
            };

            var lista = await _personalBL.GetPersonalAsync(filtro);

            var resultados = lista.Select(t => new
            {
                id = t.PERSONAL_ID,
                correo = t.Email,
                nombreCompleto = $"{t.Name} {t.LastName}",
                telefono = t.CellPhone,
                rol = t.Rol
            });

            return Json(resultados);
        }





    }
}
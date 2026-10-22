using CSRFProtectionDemo.Models;
using CSRFProtectionDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSRFProtectionDemo.Controllers;

public class HomeController(DemoStore store) : Controller
{
    [HttpGet]
    public IActionResult Index() => View(new ProfileInput { Name = store.Read() });

    // Protegido pelo filtro global AutoValidateAntiforgeryToken.
    [HttpPost]
    public IActionResult Save(ProfileInput input)
    {
        if (!ModelState.IsValid) return View("Index", input);
        store.Update(input.Name.Trim());
        TempData["Success"] = "Alteração aceite: token CSRF válido.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Error() => Problem("Ocorreu um erro inesperado.");
}

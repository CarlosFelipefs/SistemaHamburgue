using SistemaHamburgueria.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;


namespace SistemaHamburgueria.Controllers
{
    [Authorize]
    public class FuncionarioController : Controller
    {
        private EmpresaContexto db = new EmpresaContexto();

        // ─── UTILITÁRIO ───────────────────────────────────────────────
        private string GerarHash(string senha)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(senha);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // ─── AUTENTICAÇÃO ─────────────────────────────────────────────
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }
        /*
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string senha)
        {
            // TEMPORÁRIO — mostra o erro real na tela
            string senhaHash = GerarHash(senha);
            var funcionario = db.Funcionarios
                .FirstOrDefault(f => f.login == email && f.Senha == senhaHash);

            if (funcionario != null)
            {
                FormsAuthentication.SetAuthCookie(funcionario.login, false);
                Session["FuncionarioId"] = funcionario.Id;
                Session["FuncionarioNome"] = funcionario.Nome;
                Session["FuncionarioCargo"] = funcionario.Cargo;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "E-mail ou senha inválidos.";
            return View();
        }
        */

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string senha)
        {
            /*
            string senhaHash = GerarHash(senha);

            // TEMPORÁRIO — remove depois
            ViewBag.Erro = $"Hash gerado: {senhaHash} | Email: {email}";
            return View();
            */

            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    ViewBag.Erro = "Preencha e-mail e senha.";
                    return View();
                }

                string senhaHash = GerarHash(senha);

                var funcionario = db.Funcionarios
                    .FirstOrDefault(f => f.login == email && f.Senha == senhaHash);

                if (funcionario != null)
                {
                    FormsAuthentication.SetAuthCookie(funcionario.login, false);
                    Session["FuncionarioId"] = funcionario.Id;
                    Session["FuncionarioNome"] = funcionario.Nome;
                    Session["FuncionarioCargo"] = funcionario.Cargo;

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Erro = "E-mail ou senha inválidos.";
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Login] Erro: {ex.Message}");
                ViewBag.Erro = "Ocorreu um erro ao processar o login. Tente novamente.";
                return View();
            }

        }


        [AllowAnonymous]
        public ActionResult Logout()
        {
            try
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                Session.Abandon();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Logout] Erro: {ex.Message}");
            }
            return RedirectToAction("Login");
        }

        // ─── LISTAR ───────────────────────────────────────────────────
        public ActionResult Index()
        {
            try
            {
                return View(db.Funcionarios.ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Funcionario.Index] Erro: {ex.Message}");
                return View("Error");
            }
        }

        // ─── CREATE ───────────────────────────────────────────────────
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Funcionario funcionario)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(funcionario);

                bool loginExistente = db.Funcionarios.Any(f => f.login == funcionario.login);
                if (loginExistente)
                {
                    ModelState.AddModelError("login", "Já existe um funcionário com este e-mail.");
                    return View(funcionario);
                }

                // Salva senha como hash
                funcionario.Senha = GerarHash(funcionario.Senha);

                db.Funcionarios.Add(funcionario);
                db.SaveChanges();
                TempData["Sucesso"] = "Funcionário cadastrado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Funcionario.Create] Erro: {ex.Message}");
                ModelState.AddModelError("", "Erro ao salvar o funcionário. Tente novamente.");
                return View(funcionario);
            }
        }

        // ─── EDIT ─────────────────────────────────────────────────────
        public ActionResult Edit(int id)
        {
            try
            {
                var funcionario = db.Funcionarios.Find(id);
                if (funcionario == null) return HttpNotFound();

                // Limpa a senha para não exibir o hash no campo
                funcionario.Senha = string.Empty;
                return View(funcionario);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Funcionario.Edit GET] Erro: {ex.Message}");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Funcionario funcionario)
        {
            try
            {
                // Remove validação da senha — campo pode vir vazio (manter senha atual)
                ModelState.Remove("Senha");

                if (!ModelState.IsValid)
                    return View(funcionario);

                var funcNoBanco = db.Funcionarios.Find(funcionario.Id);
                if (funcNoBanco == null) return HttpNotFound();

                funcNoBanco.Nome = funcionario.Nome;
                funcNoBanco.Cargo = funcionario.Cargo;
                funcNoBanco.login = funcionario.login;

                // Só atualiza senha se o usuário digitou uma nova
                if (!string.IsNullOrWhiteSpace(funcionario.Senha))
                    funcNoBanco.Senha = GerarHash(funcionario.Senha);

                db.SaveChanges();
                TempData["Sucesso"] = "Funcionário atualizado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Funcionario.Edit POST] Erro: {ex.Message}");
                ModelState.AddModelError("", "Erro ao atualizar o funcionário.");
                return View(funcionario);
            }
        }

        // ─── DELETE ───────────────────────────────────────────────────
        public ActionResult Delete(int id)
        {
            try
            {
                var funcionario = db.Funcionarios.Find(id);
                if (funcionario == null) return HttpNotFound();
                return View(funcionario);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Funcionario.Delete GET] Erro: {ex.Message}");
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var funcionario = db.Funcionarios.Find(id);
                if (funcionario == null) return HttpNotFound();

                db.Funcionarios.Remove(funcionario);
                db.SaveChanges();
                TempData["Sucesso"] = "Funcionário removido com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Funcionario.Delete POST] Erro: {ex.Message}");
                TempData["Erro"] = "Não foi possível remover o funcionário.";
                return RedirectToAction("Index");
            }
        }
    }
}
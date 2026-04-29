using System.Web;
using System.Web.Mvc;

namespace SistemaHamburgueria
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // Exige autenticação em toda a aplicação.
            // [AllowAnonymous] nos controllers/actions sobrepõe esta regra.
            filters.Add(new AuthorizeAttribute());
        }
    }
}
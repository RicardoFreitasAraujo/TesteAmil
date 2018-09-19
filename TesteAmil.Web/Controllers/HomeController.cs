using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TesteAmil.Analisador;
using TesteAmil.Domain.Entities;

namespace TesteAmil.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Processar(HttpPostedFileBase arquivo)
        {
            bool sucesso = true;
            string mensagem = String.Empty;
            Ranking ranking = null;

            try
            {
                if (arquivo == null)
                    throw new Exception("Nenhum arquivo definido");

                var nomeArquivo = Path.GetFileName(arquivo.FileName);
                var caminhoArquivo = Path.Combine(Server.MapPath("~/Documentos"), nomeArquivo);
                arquivo.SaveAs(caminhoArquivo);

                AnalisadorRanking analisador = new AnalisadorRanking(caminhoArquivo);
                ranking = analisador.Analisar();

            }
            catch (Exception ex)
            {
                sucesso = false;
                mensagem = ex.Message;
            }

            return Json(new { sucesso = sucesso, mensagem = mensagem, data = ranking }, JsonRequestBehavior.AllowGet);
        }


    }
}
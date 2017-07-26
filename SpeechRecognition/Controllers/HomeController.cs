using SpeechRecognition.Models;
using SpeechToText.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpeechRecognition.Controllers
{
    public class HomeController : Controller
    {
        private readonly MicrosoftCognitiveSpeechService speechService = new MicrosoftCognitiveSpeechService();

        public ActionResult Index()
        {
            SpeechDetail model = new SpeechDetail();
            if (Session["text"] != null)
            {
                model.ConversationText = Session["text"].ToString();
                Session["text"] = null;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            try
            {

                // Verify that the user selected a file
                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    //var fileName = Path.GetFileName(file.FileName);
                    string path = string.Empty;
                    if (file != null && file.ContentLength > 0)
                    {
                        // extract only the filename
                        var fileName = Path.GetFileName(file.FileName);
                        // store the file inside ~/App_Data/uploads folder
                        path = Path.Combine(Server.MapPath("~/"), fileName);
                        file.SaveAs(path);
                    }
                    var stream = System.IO.File.OpenRead(path);

                    var text = speechService.GetTextFromAudioAsync(stream).Result;

                    Session["text"] = text;
                }
                // redirect back to the index action to show the form once again
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session["text"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Save(SpeechRecognition.Models.SpeechDetail model)
        {
            var context = new SpeechDetailEntities();

            var t = new SpeechDetail //Make sure you have a table called test in DB
            {
                Email = model.Email,
                ConversationText = model.ConversationText,
            };

            context.SpeechDetails.Add(t);
            context.SaveChanges();

            Session["saved"] = true;

            return RedirectToAction("Index");
        }



        
        public ActionResult Load()
        {
            ViewBag.Message = "Load text from user";

            return View();
        }

        [HttpPost]
        public ActionResult Load(SpeechRecognition.Models.SpeechDetail model)
        {
            var context = new SpeechDetailEntities();

            var detail = context.SpeechDetails.FirstOrDefault(x => x.Email == model.Email);

            model.ConversationText = detail.ConversationText;

            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrailerSearch.Models;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace TrailerSearch.Controllers
{
    public class LoadTrailerAndDisplayController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }
        //Invokes GetTrailers View
        public ActionResult GetTrailers()
        {
            return View();
        }
        //Invokes DisplayTrailers View
        public ActionResult DisplayTrailers()
        {            
            IMDB imdb=null;
            string movie = Convert.ToString(Request.Form["movieName"]);
            if (!string.IsNullOrEmpty(movie))
             imdb = new IMDB(movie);         
            return View(imdb);
        }

    }
}

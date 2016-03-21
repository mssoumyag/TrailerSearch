using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

///
namespace TrailerSearch.Models
{
    /// <summary>
    /// IMDB Wrapper class.
    /// </summary>
    public class IMDB
    {
               
        public string Id { get; set; }
        public string Title { get; set; }
        public ArrayList Videos { get; set; }
        public string ImdbURL { get; set; }
        public string urlEmbededHTML { get; set; }// string for table formation which includes video url

        //Search engine URLs
        private string GoogleSearch = "http://www.google.com/search?q=imdb+";
        private string BingSearch = "http://www.bing.com/search?q=imdb+";
        private string AskSearch = "http://www.ask.com/web?q=imdb+";

        //Hardcoded strings to static variables
        private static string ImdbUrlForVideosFirstPart = "http://www.imdb.com/video/imdb/";
        private static string ImdbUrlForVideosSecondPart = "/imdb/embed?autoplay=false&width=480";
         /// <summary>
        /// Constructor of IMDB class.
         /// </summary>
         /// <param name="movieName"></param>
        public IMDB(string movieName)
        {
            string imdbUrl = RetrieveImdbUrl(System.Uri.EscapeUriString(movieName));
            //status = false;       
            ParseImdbResponse(imdbUrl);
            
        }

        
        /// <summary>
        /// Get IMDb URL from search results
        /// </summary>
        /// <param name="movieName"></param>
        /// <param name="searchEngine"></param>
        /// <returns></returns>
        private string RetrieveImdbUrl(string movieName, string searchEngine = "google")
        {
            //default searchEngine to Google 
            string url = GoogleSearch + movieName; 
            if (searchEngine.ToLower().Equals("bing")) url = BingSearch + movieName;
            if (searchEngine.ToLower().Equals("ask")) url = AskSearch + movieName;
            string html = getUrlData(url);
            ArrayList imdbUrls = matchAll(@"<a href=""(http://www.imdb.com/title/tt\d{7}/)"".*?>.*?</a>", html);
            if (imdbUrls.Count > 0)
                return (string)imdbUrls[0]; //return first IMDb result
            else if (searchEngine.ToLower().Equals("google")) //if Google search fails
                return RetrieveImdbUrl(movieName, "bing"); //search using Bing
            else if (searchEngine.ToLower().Equals("bing")) //if Bing search fails
                return RetrieveImdbUrl(movieName, "ask"); //search using Ask
            else //search fails
                return string.Empty;
        }

    /// <summary>
    /// Parse Imdb Response. 
    /// </summary>
    /// <param name="imdbUrl"></param>
        private void ParseImdbResponse(string imdbUrl)
        {
            urlEmbededHTML = string.Empty;
            if (string.IsNullOrEmpty(imdbUrl))
            {
                
                urlEmbededHTML += "<table style='width:100%'><tr><td><b>No Trailers Found.Please search something else!!</b></td></tr></table>";
            }
            else
            {
                string html = getUrlData(imdbUrl + "combined");
                Id = match(@"<link rel=""canonical"" href=""http://www.imdb.com/title/(tt\d{7})/combined"" />", html);
                if (!string.IsNullOrEmpty(Id))
                {
                    //status = true;
                    Title = match(@"<title>(IMDb \- )*(.*?) \(.*?</title>", html, 2);
                   
                    ImdbURL = "http://www.imdb.com/title/" + Id + "/";
                    //if (GetExtraInfo)
                    //{                        
                        Videos = GetImdbVideos();
                        PopulateTableWithTheUrl();                        
                    //}
                }
            }         
        }    
        /// <summary>
        /// Forms HTML table and assigns iframe with IMDB Url
        /// </summary>
        private void PopulateTableWithTheUrl()
        {
            urlEmbededHTML = string.Empty;           
            int videoCount = 0;
            urlEmbededHTML = "<table style='width:100%'>";
            if (Videos.Count == 0)
            {
                urlEmbededHTML += "<tr><td><b>No Trailers Found.Search something else!!</b></td></tr></table>";
            }
            else
            {
                for (int row = 0; row < Math.Ceiling((double)Videos.Count / 3); row++)
                {
                    urlEmbededHTML += "<tr>";
                    for (int column = 0; column < 3; column++)
                    {
                        if (videoCount < Videos.Count)
                        {
                            urlEmbededHTML += "<td>";
                            urlEmbededHTML += "<iframe name='testiFrame' id='runtimeIFrame' frameborder='no' scrolling='no' width='480' height='270'  src='" + Videos[videoCount].ToString() + "' style='left:0; background-color: beige;'></iframe>";
                            //s += "<iframe name='testiFrame' id='runtimeIFrame" + i + "'frameborder='no' scrolling='auto' height='400px' width='100%'  src='" + VideosList[i].ToString() + "' style='left:0; background-color: beige;'></iframe>";               
                            urlEmbededHTML += "</td>";
                            videoCount++;
                        }
                    }
                    urlEmbededHTML += "</tr>";
                }
                urlEmbededHTML += "</table>";
            }     
        }
        
        /// <summary>
        /// Get URL Data
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string getUrlData(string url)
        {
            WebClient client = new WebClient();
            Random random = new Random();
            //Random IP Address
            client.Headers["X-Forwarded-For"] = random.Next(0, 255) + "." + random.Next(0, 255) + "." + random.Next(0, 255) + "." + random.Next(0, 255);
            //Random User-Agent
            client.Headers["User-Agent"] = "Mozilla/" + random.Next(3, 5) + ".0 (Windows NT " + random.Next(3, 5) + "." + random.Next(0, 2) + "; rv:2.0.1) Gecko/20100101 Firefox/" + random.Next(3, 5) + "." + random.Next(0, 5) + "." + random.Next(0, 5);
            Stream dataStream = client.OpenRead(url);
            StreamReader reader = new StreamReader(dataStream);
            StringBuilder stringBuilder = new StringBuilder();
            while (!reader.EndOfStream)
                stringBuilder.Append(reader.ReadLine());
            return stringBuilder.ToString();
        }
        /// <summary>
        /// Match all instances and return as ArrayList
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="html"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private ArrayList matchAll(string regex, string html, int i = 1)
        {
            ArrayList list = new ArrayList();
            foreach (Match item in new Regex(regex, RegexOptions.Multiline).Matches(html))
                list.Add(item.Groups[i].Value.Trim());
            return list;
        }
        /// <summary>
        /// Match single instance
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="html"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private string match(string regex, string html, int i = 1)
        {
            return new Regex(regex, RegexOptions.Multiline).Match(html).Groups[i].Value.Trim();
        }
        /// <summary>
        /// get all video urls
        /// </summary>
        /// <returns></returns>
        private ArrayList GetImdbVideos()
        {
            ArrayList list = new ArrayList();
            string videoUrl = "http://www.imdb.com/title/" + Id + "/videogallery";
            string mediaHtml = getUrlData(videoUrl);
            list = matchAll_videos(@"([/video/imdb]/[v][i][0-9])\w+", mediaHtml, 1);
            return list;
        }
        /// <summary>
        /// Match video url 
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="html"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private ArrayList matchAll_videos(string regex, string html, int i = 1)
        {
            ArrayList list = new ArrayList();
            //foreach (Match m in new Regex(regex, RegexOptions.Multiline).Matches(html))
            //    list.Add(m.Groups[i].Value);
            foreach (Match item in new Regex(regex).Matches(html))
            {
                if (!list.Contains(ImdbUrlForVideosFirstPart + (item.Value.ToString().Substring(2)) + ImdbUrlForVideosSecondPart))
                    list.Add(ImdbUrlForVideosFirstPart + (item.Value.ToString().Substring(2)) + ImdbUrlForVideosSecondPart);
            }
            return list;
        }

    }
}
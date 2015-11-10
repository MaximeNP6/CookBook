using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace getImports
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ne pas remplir
            Object[] result = null;
            HttpWebRequest con = null;
            HttpWebResponse httpResponse = null;

            //Url de base
            String urlBase = "http://v8.mailperformance.com/";
            //X-Key
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            //Lancement de la connexion pour remplir la requete
            String url = urlBase + "imports/";

            result = allConnection(con, url, xKey, null, "GET");
            con = (HttpWebRequest)result[1];
            httpResponse = (HttpWebResponse)result[2];

            verification(result);

            Console.ReadLine();
            httpResponse.Close();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////FIN DU PROGRAMME////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////DEBUT DES FONCTIONS/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static Boolean verification(Object[] result)
        {
            int response = 0;
            HttpWebRequest con = null;
            HttpWebResponse httpResponse = null;
            String responseString = null;

            response = (int)result[0];
            con = (HttpWebRequest)result[1];
            httpResponse = (HttpWebResponse)result[2];
            responseString = (String)result[3];

            // Verification
            if (response >= 200 && response <= 299)
            {
                // Tout est bon

                //On affiche la reponse
                Console.Write("OK : \n\n" + responseString + "\n\n");
                return (true);
            }
            // Il y a eu une erreur
            Console.Write("Error : " + response + " " + responseString + "\n");
            return (false);
        }

        //Fonction de connexion
        static HttpWebRequest Connect(String url, String xKey, String method)
        {
            //Lancement de la connexion pour remplir la requete
            HttpWebRequest con = (HttpWebRequest)WebRequest.Create(url);
            con.Method = method;

            //Mise en place du xKey et des options
            con.Headers.Add("X-Key", xKey);
            con.ContentType = "application/json";
            con.Accept = "application/vnd.mperf.v8.import.v1+json";

            return (con);
        }

        //Fonction de connexion et envoie des informations
        static Object[] allConnection(HttpWebRequest con, String url, String xKey, JObject jsonMessage, String method)
        {
            con = Connect(url, xKey, method);
            con.ContentLength = 0;
            if (jsonMessage != null)
            {
                con.ContentLength = jsonMessage.ToString().Length;
                var streamWriter = new StreamWriter(con.GetRequestStream());
                streamWriter.Write(jsonMessage);
                streamWriter.Flush();
                streamWriter.Close();
            }

            //Test de l'envoi
            HttpWebResponse httpResponse = null;
            int response = 0;
            String responseString = null;
            try
            {
                httpResponse = (HttpWebResponse)con.GetResponse();
                response = 200;

                //Lecture des donnees
                StreamReader reader = new StreamReader(con.GetResponse().GetResponseStream());
                responseString = reader.ReadToEnd();
                httpResponse.Close();
                reader.Close();
            }
            //Reception du signal
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    httpResponse = (HttpWebResponse)ex.Response;
                    response = (int)httpResponse.StatusCode;
                }
            }
            Object[] result = { response, con, httpResponse, responseString };
            return (result);
        }
    }
}

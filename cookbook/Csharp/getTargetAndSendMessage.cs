using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace getTargetAndSendMessage
{
    class Program
    {
        static HttpWebRequest Connect(string url, string xKey, string method)
        {
            //Lancement de la connexion pour remplir la requete
            HttpWebRequest con = (HttpWebRequest)WebRequest.Create(url);
            con.Method = method;

            //Mise en place du xKey et des options
            con.Headers.Add("X-Key", xKey);
            con.ContentType = "application/json";

            return (con);
        }

        static void Main(string[] args)
        {
            //Ici, renseignez l'email, la X-Key et l'id du message
            string unicity = "test@test.com";
            string xKey = "ABCDEFJHIJKLMNOPQRSTUVWXYZ0123456789";
            string idMessage = "000ABC";

            //Appel de la fonction Connect
            string url = "http://v8.mailperformance.com/targets?unicity=" + unicity;
            HttpWebRequest con = Connect(url, xKey, "GET");
            
            //Verification des reponses
            HttpWebResponse httpResponseGet = (HttpWebResponse)con.GetResponse();
            if ((int)httpResponseGet.StatusCode != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {0}", (int)httpResponseGet.StatusCode, httpResponseGet.StatusCode.ToString());
            }
            else
            {
                //Lecture des donnees
                StreamReader reader = new StreamReader(con.GetResponse().GetResponseStream());
                string responseString = reader.ReadToEnd();
                httpResponseGet.Close();
                reader.Close();

                //On prend l'idTarget
                dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                string idTarget = responseJson.id;

                //Nouvelle url en fonction de l'id du message et de la cible
                url = "http://v8.mailperformance.com/actions/" + idMessage + "/targets/" + idTarget;

                //Lancement de la connexion pour remplir la requete
                con = Connect(url, xKey, "POST");
                con.ContentLength = 0;

                //Verification des reponses
                HttpWebResponse httpResponsePost = (HttpWebResponse)con.GetResponse();
                if ((int)httpResponsePost.StatusCode != 204)
                {
                    //Affichage de l'erreur
                    Console.Write("Error : {0} {0}", (int)httpResponsePost.StatusCode, httpResponsePost.StatusCode.ToString());
                }
                else
                {
                    Console.Write("Message sent to " + unicity);
                }
                httpResponsePost.Close();
            }

            //Attente de lecture (optionnel)
            Console.ReadLine();

            httpResponseGet.Close();
        }
    }
}

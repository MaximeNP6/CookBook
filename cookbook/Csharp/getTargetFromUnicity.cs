using System;
using System.IO;
using System.Net;

namespace getTargetFromUnicity
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ici, renseignez l'email dont vous voulez obtenir les valeurs des champs et la X-Key
            string unicity = "test@test.com";
            string xKey = "ABCDEFJHIJKLMNOPQRSTUVWXYZ0123456789";
            
            //Lancement de la connection pour remplir la requete
            string url = "http://v8.mailperformance.com/targets?unicity=" + unicity;
            HttpWebRequest con = (HttpWebRequest)WebRequest.Create(url);
            con.Method = "GET";

            //Mise en place du xKey et des options
            con.Headers.Add("X-Key", xKey);
            con.ContentType = "application/json";

            //Verification des reponses
            HttpWebResponse httpResponse = (HttpWebResponse)con.GetResponse();
            if ((int)httpResponse.StatusCode != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {0}", (int)httpResponse.StatusCode, httpResponse.StatusCode.ToString());
            }
            else
            {
                //Lecture des donnees
                StreamReader reader = new StreamReader(con.GetResponse().GetResponseStream());
                string responseString = reader.ReadToEnd();
                Console.Write("{0}", responseString);
            }

            //Attente de lecture (optionnel)
            Console.ReadLine();

            httpResponse.Close();
        }
    }
}

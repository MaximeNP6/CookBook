using Newtonsoft.Json;
using System;
using System.Net;
using System.Xml;

namespace getTargetAndSendMessage
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Changez le Path pour correspondre à la destination de votre fichier de configuration
            var doc = new XmlDocument();
            doc.Load("config.xml");

            var xKey = doc.DocumentElement.SelectSingleNode("/config/xKey").InnerText;
            var baseUrl = doc.DocumentElement.SelectSingleNode("/config/url").InnerText;


            //Ici, renseignez l'email, la X-Key et l'id du message
            const string unicity = "test@test.com";
            const string idMessage = "000000";

            //Appel de la fonction Connect
            var url = baseUrl + "targets?unicity=" + unicity;
            var reponse = Utils.allConnection(url, xKey, null, "GET");

            //Verification des reponses
            if ((int)reponse[0] != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)reponse[0], ((HttpWebResponse)reponse[2]).StatusCode.ToString());
                //Attente de lecture (optionnel)
                Console.ReadLine();

                ((HttpWebResponse)reponse[2]).Close();
                return;
            }
            
            //Lecture des donnees
            
            //On prend l'idTarget
            dynamic responseJson = JsonConvert.DeserializeObject((string)reponse[3]);
            string idTarget = responseJson.id;

            //Nouvelle url en fonction de l'id du message et de la cible
            url = baseUrl + "actions/" + idMessage + "/targets/" + idTarget;

            reponse = Utils.allConnection(url, xKey, null, "POST");

            //Verification des reponses
            if ((int)reponse[0] == 200)
            {
                Console.Write("Message sent to " + unicity);
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)reponse[0], ((HttpWebResponse)reponse[2]).StatusCode.ToString());
            }

            //Attente de lecture (optionnel)
            Console.ReadLine();

            ((HttpWebResponse)reponse[2]).Close();
        }
    }
}

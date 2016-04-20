using System;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace sendHTML
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

            //Ici, renseignez l'email de la cible, la xKey, l'id du message
            const string unicity = "test@test.com";
            const string idMessage = "000000";

            const string htmlMessage = "html message for Csharp";	//Si vous ne voulez pas de cette option, commentez la ligne de creation du Json
            const string textMessage = "text message for Csharp";	//Si vous ne voulez pas de cette option, commentez la ligne de creation du Json
            const string subject = "subject for Csharp";
            const string mailFrom = "mail@address.com";
            const string replyTo = "mail@return.com";

            //Appel de la fonction Connect
            var url = baseUrl + "targets?unicity=" + unicity;
            var reponse = Utils.allConnection(url, xKey, null, "GET");

            //Verification des reponses
            if ((int)reponse[0] == 200)
            {
                Console.Write("{0}", (string)reponse[3]);
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)reponse[0], ((HttpWebResponse)reponse[2]).StatusCode.ToString());
                ((HttpWebResponse)reponse[2]).Close();
                return;
            }
            ((HttpWebResponse)reponse[2]).Close();

            //On affiche la cible
            Console.Write((string)reponse[3] + "\n");

            //On prend le idTarget
            dynamic responseJson = JsonConvert.DeserializeObject((string)reponse[3]);
            string idTarget = responseJson.id;

            //Nouvelle url en fonction de l'id du message et de la cible
            url = baseUrl + "actions/" + idMessage + "/targets/" + idTarget;

            //Creation du message en Json
            var content = new JObject();
            content.Add("html", htmlMessage);	//Si vous ne voulez pas de l'option : htmlMessage, commentez cette ligne
            content.Add("text", textMessage);	//Si vous ne voulez pas de l'option : textMessage, commentez cette ligne

            var header = new JObject();
            header.Add("subject", subject);
            header.Add("mailFrom", mailFrom);
            header.Add("replyTo", replyTo);

            var jsonMessage = new JObject();
            jsonMessage.Add("content", content);
            jsonMessage.Add("header", header);


            Console.WriteLine("Sending :\n" + jsonMessage + '\n');

            //Lancement de la connexion pour remplir la requete
            reponse = Utils.allConnection(url, xKey, null, "POST");

            //Verification des reponses
            if ((int)reponse[0] == 200)
            {
                Console.Write("\nMessage sent to " + unicity);
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

using System;
using System.Net;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace createSmsAction
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

            const string type = "smsMessage";	//Code pour envoyer un SMS
            const string name = "SMSFromApi (Csharp)";	//Nom de l'action
            const string description = "SMSFromApi (Csharp)";	//Description de l'action

            int? informationFolder = 01234;	//Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
            int? informationCategory = 01234;	//Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

            const string textContent = "Text message";	//Message texte

            //On trouve l'adresse pour la requete
            var url = baseUrl + "actions";


            //Creation du Json du message
            var informations = new JObject();
            informations.Add("folder", informationFolder);
            informations.Add("category", informationCategory);

            var content = new JObject();
            content.Add("textContent", textContent);

            var jsonMessage = new JObject();
            jsonMessage.Add("type", type);
            jsonMessage.Add("name", name);
            jsonMessage.Add("description", description);
            jsonMessage.Add("informations", informations);
            jsonMessage.Add("content", content);

            //On affiche le message
            Console.Write(jsonMessage + "\n");

            //Lancement de la connexion pour remplir la requete
            var result = Utils.allConnection(url, xKey, jsonMessage, "POST");

            //Verification des reponses
            if ((int)result[0] == 200)
            {
                Console.Write("Action : " + name + " created.");
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)result[0], ((HttpWebResponse)result[2]).StatusCode.ToString());
            }

            //Attente de lecture (optionnel)
            Console.ReadLine();
            ((HttpWebResponse)result[2]).Close();

        }
    }
}

using System;
using System.Net;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace createSmsCampaignAction
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

            const string type = "smsCampaign"; //Code pour envoyer une campagne SMS
            const string name = "SMSCampaignFromApi (Csharp)"; //Nom de l'action
            const string description = "SMSCampaignFromApi (Csharp)"; //Description de l'action

            int? informationFolder = null;
                //Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
            int? informationCategory = null;
                //Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

            const string textContent = "Text message"; //Message texte

            //On trouve l'adresse pour la requete
            var url = baseUrl + "actions";


            //Creation du Json du message
            var informations = new JObject();
            informations.Add("folder", informationFolder);
            informations.Add("category", informationCategory);

            var content = new JObject();
            content.Add("textContent", textContent);

            var scheduler = new JObject();
            scheduler.Add("type", "asap"); //Envoie : immediat = 'asap' / Date = 'scheduled'
            //scheduler.Add("startDate", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever

            var jsonMessage = new JObject();
            jsonMessage.Add("type", type);
            jsonMessage.Add("name", name);
            jsonMessage.Add("description", description);
            jsonMessage.Add("informations", informations);
            jsonMessage.Add("scheduler", scheduler);
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

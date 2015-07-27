using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace createSmsCampaignAction
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ici, renseignez la xKey et les parametres personnalises du message
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            String type = "smsCampaign";	//Code pour envoyer une campagne SMS
            String name = "SMSCampaignFromApi (Csharp)";	//Nom de l'action
            String description = "SMSCampaignFromApi (Csharp)";	//Description de l'action

            int informationFolder = 0123;	//Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
            int informationCategory = 0123;	//Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

            String textContent = "Text message";	//Message texte

            //On trouve l'addresse pour la requete
            String url = "http://v8.mailperformance.com/actions";


            //Creation du Json du message
            JObject informations = new JObject();
            informations.Add("folder", informationFolder);
            informations.Add("category", informationCategory);

            JObject content = new JObject();
            content.Add("textContent", textContent);

            JObject jsonMessage = new JObject();
            jsonMessage.Add("type", type);
            jsonMessage.Add("name", name);
            jsonMessage.Add("description", description);
            jsonMessage.Add("informations", informations);
            jsonMessage.Add("content", content);

            //On affiche le message
            Console.Write(jsonMessage + "\n");

            //Lancement de la connexion pour remplir la requete
            HttpWebRequest con = Connect(url, xKey, "POST");
            con.ContentLength = jsonMessage.ToString().Length;
            var streamWriter = new StreamWriter(con.GetRequestStream());
            streamWriter.Write(jsonMessage);
            streamWriter.Flush();
            streamWriter.Close();

            //Test de l'envoie
            HttpWebResponse httpResponse = null;
            int response = 0;
            try
            {
                httpResponse = (HttpWebResponse)con.GetResponse();
                response = 200;
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

            //Verification des reponses
            if (response != 200)
            {
                //Affichage de l'erreur
                Console.Write("\nError : " + response);
            }
            else
            {
                Console.Write("Action : " + name + " created.");
            }
            httpResponse.Close();

            //Attente de lecture (optionnel)
            Console.ReadLine();

            httpResponse.Close();
        }

        //Fonctions ----


        //Fonction de connexion
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
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace createMailCampaignAction
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ici, renseignez la xKey
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            String type = "mailCampaign";	//Code pour envoyer une campagne de mail
            String name = "MailCampaignFromApi (Csharp)";	//Nom de l'action
            String description = "MailCampaignFromApi (Csharp)";	//Description de l'action

            int informationFolder = 0123;	//Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
            int informationCategory = 0123;	//Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

            String contentHeadersFromPrefix = "prefix";	//Adresse expeditice
            String contentHeadersFromLabel = "label";	//Libelle expediteur
            String contentHeadersReply = "address@reply.com"; //Adresse de reponse

            String contentSubject = "Subject of the message";	//Objet du mail
            String contentHTML = "Html message";	//Message HTML
            String contentText = "Text message";	//Message texte

            int[] idTestSegment = { 0123 };	//Id du segment de test pour la validation
            int[] idSelectSegment = { 0123 };	//Ids des segments selectionnes


            //On trouve l'adresse pour la requete
            String url = "http://v8.mailperformance.com/actions";


            //Creation du Json du message
            JObject informations = new JObject();
            informations.Add("folder", informationFolder);
            informations.Add("category", informationCategory);

            JObject contentHeadersFrom = new JObject();
            contentHeadersFrom.Add("prefix", contentHeadersFromPrefix);
            contentHeadersFrom.Add("label", contentHeadersFromLabel);

            JObject contentHeaders = new JObject();
            contentHeaders.Add("from", contentHeadersFrom);
            contentHeaders.Add("reply", contentHeadersReply);

            JArray SelectSegment = new JArray();
            SelectSegment.Add(idTestSegment);

            JObject segments = new JObject();
            segments.Add("selected", SelectSegment);

            JObject scheduler = new JObject();
<<<<<<< HEAD
            scheduler.Add("type", "asap");	//Envoie : immediat = 'asap' / Date = 'scheduled'
            //scheduler.Add("startDate", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever
=======
            scheduler.Add("type", "asap");	//Envoi : immediat = 'asap' / Date = 'scheduled'
            //scheduler.Add("type", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever
>>>>>>> origin/master
            scheduler.Add("segments", segments);

            JObject content = new JObject();
            content.Add("headers", contentHeaders);
            content.Add("subject", contentSubject);
            content.Add("html", contentHTML);
            content.Add("text", contentText);

            JObject jsonMessage = new JObject();
            jsonMessage.Add("type", type);
            jsonMessage.Add("name", name);
            jsonMessage.Add("description", description);
            jsonMessage.Add("informations", informations);
            jsonMessage.Add("scheduler", scheduler);
            jsonMessage.Add("content", content);

            //On affiche le message
            Console.Write(jsonMessage + "\n");


            //Lancement de la connexion pour remplir la requete
            Object[] connection = allConnection(url, xKey, jsonMessage);
            int response = (int)connection[0];
            HttpWebRequest con = (HttpWebRequest)connection[1];
            HttpWebResponse httpResponse = (HttpWebResponse)connection[2];
            String responseString = (String)connection[3];

            //Verification des reponses
            if (response != 200)
            {
                //Affichage de l'erreur
                Console.Write("\nError : " + response);
            }
            else
            {
                //L'action a bien ete cree
                Console.Write("Action : " + name + " created.\n\n");

                //On recupere l'id de l'action
                dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                string idAction = responseJson.id;

                //On valide l'action
                url = "http://v8.mailperformance.com/actions/" + idAction + "/validation";

                JArray testSegments = new JArray();
                testSegments.Add(idTestSegment);	//Les Ids des differents segments de tests

                JObject jsonTest = new JObject();
                jsonTest.Add("fortest", true);	//Phase de test
                jsonTest.Add("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
                jsonTest.Add("testSegments", testSegments);	//Les Ids des differents segments de tests
                jsonTest.Add("mediaForTest", null);	//Rediriger tous les tests vers une seule adresse ('null' pour aucune valeur)
                jsonTest.Add("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
                jsonTest.Add("comments", null);	//Commentaire ('null' pour aucuns commentaires)

                //On affiche le json
                Console.Write(jsonTest + "\n");

                //Lancement de la connexion
                connection = allConnection(url, xKey, jsonTest);
                response = (int)connection[0];
                con = (HttpWebRequest)connection[1];
                httpResponse = (HttpWebResponse)connection[2];
                responseString = (String)connection[3];

                int actionState = waitForState(idAction, xKey);

                if (response != 200 || actionState == 20)
                {
                    //Affichage de l'erreur
                    if (actionState == 20)
                        Console.Write("Error : the test failed.");
                    else
                        Console.Write("Error : " + response + " " + responseString);
                }
                else
                {
                    //La phase de test a reussi
                    Console.Write("The action " + name + " is tested.\n\n");

                    //Creation du Json du message pour la validation
                    JObject jsonValid = new JObject();
                    jsonValid.Add("fortest", false);	//Phase de validation
                    jsonValid.Add("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
                    jsonValid.Add("testSegments", null);	//Les Ids des differents segments de tests
                    jsonValid.Add("mediaForTest", null);	//Rediriger tous les tests vers une seule adresse ('null' pour aucune valeur)
                    jsonValid.Add("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
                    jsonValid.Add("comments", null);	//Commentaire ('null' pour aucuns commentaires)

                    //On affiche le json
                    Console.Write(jsonValid + "\n");

                    //Lancement de la connexion
                    connection = allConnection(url, xKey, jsonValid);
                    response = (int)connection[0];
                    con = (HttpWebRequest)connection[1];
                    httpResponse = (HttpWebResponse)connection[2];
                    responseString = (String)connection[3];

                    //Verification des reponses
                    if (response != 200)
                    {
                        //Affichage de l'erreur
                        Console.Write("Error : " + response + " " + responseString);
                    }
                    else
                    {
                        //La phase de validation a reussi
                        Console.Write("The action " + name + " is validated.\n\n");
                    }
                }
                httpResponse.Close();

                //Attente de lecture (optionnel)
                Console.ReadLine();
            }
        }

        private static object[] allConnection()
        {
            throw new NotImplementedException();
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

		//Fonction de connexion et envoie des informations
        static Object[] allConnection(String url, String xKey, JObject jsonMessage)
        {
            HttpWebRequest con = Connect(url, xKey, "POST");
            con.ContentLength = jsonMessage.ToString().Length;
            var streamWriter = new StreamWriter(con.GetRequestStream());
            streamWriter.Write(jsonMessage);
            streamWriter.Flush();
            streamWriter.Close();

            //Test de l'envoie
            HttpWebResponse httpResponse = null;
            int response = 0;
            string responseString = null;
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

		//Fonction d'attente de fin de test
        static int waitForState(String idAction, String xKey)
        {
            int actionState = 30;

            while (actionState != 38 && actionState != 20)
            {
                //On attend 20 secondes
                Console.Write("Wait 20sec...\n");
                System.Threading.Thread.Sleep(2000);

                //Nouvelle adresse
                String url = "http://v8.mailperformance.com/actions/" + idAction;

                //Lancement de la connexion pour remplir la requete
                HttpWebRequest con = Connect(url, xKey, "GET");

                //Test de l'envoi
                HttpWebResponse httpResponse = null;
                int response = 0;
                string responseString = null;
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
                if (response == 200)
                {
                    //On recupere l'etat de l'action
                    dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                    actionState = responseJson.informations.state;
                }
                else
                    actionState = 20;
            }
            return (actionState);
        }
    }
}

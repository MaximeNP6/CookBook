﻿using System;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace createMailAction
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

            const string type = "mailMessage";	//Code pour envoyer un mail
            const string name = "MailFromApi (Csharp)";	//Nom de l'action
            const string description = "MailFromApi (Csharp)";	//Description de l'action

            int? informationFolder = 01234;	//Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
            int? informationCategory = 01234;	//Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

            const string contentHeadersFromPrefix = "prefix";	//Adresse expeditice
            const string contentHeadersFromLabel = "label";	//Libelle expediteur
            const string contentHeadersReply = "address@reply.dev"; //Adresse de reponse

            const string contentSubject = "Subject of the message";	//Objet du mail
            const string contentHTML = "Html message";	//Message HTML
            const string contentText = "Text message";	//Message texte

            int[] idTestSegment = { 01234 };	//Id du segment de test
            int[] idSelectSegment = { 01234 };	//Ids des segments selectionnes


            //On trouve l'adresse pour la requete
            var url = baseUrl + "actions";


            //Creation du Json du message
            var informations = new JObject();
            informations.Add("folder", informationFolder);
            informations.Add("category", informationCategory);

            var contentHeadersFrom = new JObject();
            contentHeadersFrom.Add("prefix", contentHeadersFromPrefix);
            contentHeadersFrom.Add("label", contentHeadersFromLabel);

            var contentHeaders = new JObject();
            contentHeaders.Add("from", contentHeadersFrom);
            contentHeaders.Add("reply", contentHeadersReply);

            var content = new JObject();
            content.Add("headers", contentHeaders);
            content.Add("subject", contentSubject);
            content.Add("html", contentHTML);
            content.Add("text", contentText);

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
                //L'action a bien ete cree
                Console.Write("Action : " + name + " created.\n\n");
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)result[0], ((HttpWebResponse)result[2]).StatusCode.ToString());
                ((HttpWebResponse)result[2]).Close();
                Console.ReadLine();
                return;
            }

            ((HttpWebResponse)result[2]).Close();


            //On recupere l'id de l'action
            dynamic responseJson = JsonConvert.DeserializeObject((string)result[3]);
            string idAction = responseJson.id;

            //On valide l'action
            url = baseUrl + "actions/" + idAction + "/validation";

            var testSegments = new JArray();
            testSegments.Add(idTestSegment);	//Les Ids des differents segments de tests

            var jsonTest = new JObject();
            jsonTest.Add("fortest", true);	//Phase de test
            jsonTest.Add("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
            jsonTest.Add("testSegments", testSegments);	//Les Ids des differents segments de tests
            jsonTest.Add("mediaForTest", null);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
            jsonTest.Add("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
            jsonTest.Add("comments", null);	//Commentaire ('NULL' pour aucuns commentaires)

            //On affiche le json
            Console.Write(jsonTest + "\n");

            //Lancement de la connexion
            result = Utils.allConnection(url, xKey, jsonTest, "POST");

            var actionState = Utils.waitForState(idAction, xKey, baseUrl);

            if ((int)result[0] != 200 || actionState != 38)
            {
                //Affichage de l'erreur
                if (actionState == 20)
                    Console.Write("Error : the test failed.");
                else if (actionState == 10)
                    Console.Write("Error : check the campaign in the Backoffice.");
                else
                    Console.Write("Error : " + (int)result[0] + " " + (string)result[3]);
                Console.ReadLine();
                return;
            }
            else
            {
                //La phase de test a reussi
                Console.Write("The action " + name + " has been tested.\n\n");

                //Creation du Json du message pour la validation
                var jsonValid = new JObject();
                jsonValid.Add("fortest", false);	//Phase de validation
                jsonValid.Add("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
                jsonValid.Add("testSegments", null);	//Les Ids des differents segments de tests
                jsonValid.Add("mediaForTest", null);	//Rediriger tous les tests vers une seule adresse ('null' pour aucune valeur)
                jsonValid.Add("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
                jsonValid.Add("comments", null);	//Commentaire ('null' pour aucuns commentaires)

                //On affiche le json
                Console.Write(jsonValid + "\n");

                //Lancement de la connexion
                result = Utils.allConnection(url, xKey, jsonValid, "POST");

                //Verification des reponses
                if ((int)result[0] == 200)
                {
                    //L'action a bien ete cree
                    Console.Write("Action : " + name + " validated.\n\n");
                }
                else
                {
                    //Affichage de l'erreur
                    Console.Write("Error : {0} {1}", (int)result[0], ((HttpWebResponse)result[2]).StatusCode.ToString());
                }

                ((HttpWebResponse)result[2]).Close();
                //Attente de lecture (optionnel)
                Console.ReadLine();
            }
        }
    }
}

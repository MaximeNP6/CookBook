using System;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace duplicateAndValidate
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

            const string idAction = "000AB0";	//Id de l'action a dupliquer
            int[] idTestSegment = { 1234 };	//Id du segment de test

            //Duplication de l'action

            //Nouvelle url
            var url = baseUrl + "actions/" + idAction + "/duplication";

            //On duplique a l'identique
            var result = Utils.allConnection(url, xKey, null, "POST");
            dynamic responseJson =  JsonConvert.DeserializeObject((string) result[3]);
            string name = responseJson.name;

            //Verification des reponses
            if ((int)result[0] == 200)
            {
                Console.Write("The action duplicate ( " + name + " ) has been created.\n\n");
                Console.Write((string)result[3] + "\n");
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)result[0], ((HttpWebResponse)result[2]).StatusCode.ToString());
                return;
            }

            ((HttpWebResponse)result[2]).Close();


            //On recupere l'id de l'action
            responseJson = JsonConvert.DeserializeObject((string)result[3]);
            string idNewAction = responseJson.id;

            //On test l'action

            //Nouvelle url
            url = baseUrl + "actions/" + idNewAction + "/validation";

            var testSegments = new JArray();
            testSegments.Add(idTestSegment);

            var jsonTest = new JObject();
            jsonTest.Add("fortest", true);	//Phase de test
            jsonTest.Add("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
            jsonTest.Add("testSegments", testSegments);	//Les Ids des differents segments de test
            jsonTest.Add("mediaForTest", null);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
            jsonTest.Add("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
            jsonTest.Add("comments", null);	//Commentaire ('NULL' pour aucuns commentaires)

            //On affiche le json
            Console.Write(jsonTest + "\n");

            //Lancement de la connexion
            result = Utils.allConnection(url, xKey, jsonTest, "POST");

            //Verification des reponses
            if ((int)result[0] == 200)
            {
                Console.Write("{0}", (string)result[3]);
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)result[0], ((HttpWebResponse)result[2]).StatusCode.ToString());
                ((HttpWebResponse)result[2]).Close();
                return;
            }

            ((HttpWebResponse)result[2]).Close();

            var actionState = Utils.waitForState(idNewAction, xKey, baseUrl);

            if ((int)result[0] != 200 || actionState != 38)
            {
                //Affichage de l'erreur
                if (actionState == 20)
                    Console.Write("Error : the test has failed.");
                else if (actionState == 10)
                    Console.Write("Error : check the campaign in the Backoffice.");
                else
                    Console.Write("Error : " + (int)result[0] + " " + (string)result[3]);
            }
            else
            {
                //La phase de test a reussi
                Console.Write("The action has been tested.\n\n");

                //Creation du Json du message pour la validation
                var jsonValid = new JObject();
                jsonValid.Add("fortest", false);	//Phase de test
                jsonValid.Add("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
                jsonValid.Add("testSegments", null);	//Les Ids des differents segments de tests
                jsonValid.Add("mediaForTest", null);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
                jsonValid.Add("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
                jsonValid.Add("comments", null);	//Commentaire ('NULL' pour aucun commentaire)

                //On affiche le json
                Console.Write(jsonValid + "\n");

                //Lancement de la connexion
                result = Utils.allConnection(url, xKey, jsonValid, "POST");

                //Verification des reponses
                if ((int)result[0] == 200)
                {
                    Console.Write("The action has been validated.\n\n");
                }
                else
                {
                    //Affichage de l'erreur
                    Console.Write("Error : {0} {1}", (int)result[0], ((HttpWebResponse)result[2]).StatusCode.ToString());
                }
            }

            //Attente de lecture (optionnel)
            ((HttpWebResponse)result[2]).Close();

            Console.ReadLine();

        }
    }
}

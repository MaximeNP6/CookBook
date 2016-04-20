using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Xml;

namespace duplicateAndAddNewSegment
{
    internal class Program
    {
        static JObject getNewActionDuplicateJson(JObject actionDuplicateJson, string segmentId)
        {
            int[] segmentList = { Convert.ToInt32(segmentId) };

            var segmentArray = new JArray();
            segmentArray.Add(segmentList);

            //Modification de la nouvelle action
            var segments = new JObject();
            segments.Add("selected", segmentArray);

            var scheduler = new JObject();
            scheduler.Add("type", "asap");	//Envoi : immediat = 'asap' / Date = 'scheduled'
            //scheduler.Add("startDate", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever
            scheduler.Add("segments", segments);

            actionDuplicateJson.Remove("scheduler");
            actionDuplicateJson.Add("scheduler", scheduler);

            //On affiche le message
            Console.Write(actionDuplicateJson + "\n");

            return (actionDuplicateJson);
        }

        static JObject getActionValidation(string segmentId)
        {
            var segmentList = new JArray();
            segmentList.Add(segmentId);

            var actionValidationJson = new JObject();
            actionValidationJson.Add("fortest", true);	//Phase de test
            actionValidationJson.Add("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
            actionValidationJson.Add("testSegments", segmentList);	//Les Ids des differents segments de tests
            actionValidationJson.Add("mediaForTest", null);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
            actionValidationJson.Add("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
            actionValidationJson.Add("comments", null);	//Commentaire ('NULL' pour aucuns commentaires)

            //On affiche le message
            Console.Write(actionValidationJson + "\n");

            return (actionValidationJson);
        }

        public static object[] duplicateAction(string baseUrl, string actionId, string xKey)
        {
            //Nouvelle url
            var url = baseUrl + "actions/" + actionId + "/duplication";

            //On duplique a l'identique
            var result = Utils.allConnection(url, xKey, null, "POST");
            dynamic responseJson = JsonConvert.DeserializeObject((string)result[3]);
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
                return null;
            }

            ((HttpWebResponse)result[2]).Close();

            return result;
        }

        public static int updateActionDuplicate(string baseUrl, string actionDuplicateId, dynamic actionDuplicateJson, string segmentId, string xKey)
        {
            //Nouvelle url
            var url = baseUrl + "actions/" + actionDuplicateId;

            var newJson = getNewActionDuplicateJson(actionDuplicateJson, segmentId);
            
            var result = Utils.allConnection(url, xKey, newJson, "PUT");

            //Verification des reponses
            if ((int)result[0] == 200)
            {
                Console.Write("The action duplicate has been updated.\n\n");
                Console.Write((string)result[3] + "\n");
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)result[0], ((HttpWebResponse)result[2]).StatusCode.ToString());
                return -1;
            }

            ((HttpWebResponse)result[2]).Close();

            return 0;
        }

        public static object[] testActionDuplicate(string baseUrl, string actionDuplicateId, string segmentId, string xKey)
        {
            //Nouvelle url
            string url = baseUrl + "actions/" + actionDuplicateId + "/validation";

            //Lancement de la connexion
            var result = Utils.allConnection(url, xKey, getActionValidation(segmentId), "POST");

            if ((int)result[0] != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error request 'PUT' updateActionDuplicate : " + (int)result[0]);
                return null;
            }

            var actionState = Utils.waitForState(actionDuplicateId, xKey, baseUrl);

            if (actionState != 38)
            {
                //Affichage de l'erreur
                if (actionState == 20)
                    Console.Write("Error : the test failed.");
                else if (actionState == 10)
                    Console.Write("Error : check the campaign in the Backoffice.");
            }
            else
            {
                //La phase de test a reussi
                Console.Write("The action has been tested.\n\n");
            }

            return (result);
        }

        private static void Main(string[] args)
        {
            // Changez le Path pour correspondre à la destination de votre fichier de configuration
            var doc = new XmlDocument();
            doc.Load("config.xml");

            var xKey = doc.DocumentElement.SelectSingleNode("/config/xKey").InnerText;
            var baseUrl = doc.DocumentElement.SelectSingleNode("/config/url").InnerText;

            const string segmentId = "01234";	//Id du segment a ajouter
            const string testSegmentId = "01234";	//Id du segment a ajouter
            const string actionId = "000ABC";	//Id de l'action a dupliquer

            var result = duplicateAction(baseUrl, actionId, xKey);

            if (result == null)
                return;

            //On recupere l'id de l'action
            dynamic responseJson = JsonConvert.DeserializeObject((string)result[3]);
            string idNewAction = responseJson.id;

            var ret = updateActionDuplicate(baseUrl, idNewAction, responseJson, segmentId, xKey);

            if (ret != 0)
                return;

            result = testActionDuplicate(baseUrl, idNewAction, testSegmentId, xKey);

            if (result == null)
                return;

            ((HttpWebResponse)result[2]).Close();

            Console.WriteLine("\nDupplicate done");

            Console.ReadLine();
        }

    }
}

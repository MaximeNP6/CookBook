using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace duplicateAndAddNewSegment
{
    class Program
    {
        static JObject getTargetJson(String unicity)
        {
            //Syntaxe pour les differents types d'informations :
            String targetString = "name";	//Chaine de caracteres
            String tagetListOfValues = "Mr";	//Liste de valeurs
            String targetEmail = unicity;	//E-mail
            String tagetPhoneNumber = "0123456789";	//Telephone
            String targetTextZone = "150 caracters max";	//Zone de texte
            int targetNumbers = 123;	//Valeur numerique
            String targetDate = "01/01/2000";	//Date
            JProperty listMultipleValues = new JProperty("5456", "valeur 1", "valeur 2");	//Liste de valeurs multiples : particulier, la premiere valeur est l'id-champ, les autres sont les valeurs de ce champ.

            //Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur du champ"
            JObject targetJson = new JObject();
            targetJson.Add("5398", targetString);
            targetJson.Add("5399", tagetListOfValues);
            targetJson.Add("5400", targetEmail);
            targetJson.Add("5452", tagetPhoneNumber);
            targetJson.Add("5453", targetTextZone);
            targetJson.Add("5454", targetNumbers);
            targetJson.Add("5455", targetDate);
            targetJson.Add(listMultipleValues);   //Liste de valeurs multiples : particulier, la premiere valeur est l'id-champ, les autres sont les valeurs de ce champ.

            //On affiche le message
            Console.Write("New Json : " + targetJson + "\n");

            return (targetJson);
        }

        static JObject getSegmentJson()
        {
            String segmentType = "static";	//Code pour creer un segment Statique
            String segmentName = "Nom du segment";	//Nom du segment
            String segmentDescription = "Description";	//Description du segment
            String segmentExpiration = "2016-01-08T12:11:00Z";	//Date d'expiration du segment
            Boolean segmentIsTest = true;	//Le test est un segment de test : oui = 'true' / non = 'false'
            
            //Creation du Json du message
            JObject segmentJson = new JObject();
            segmentJson.Add("type", segmentType);
            segmentJson.Add("name", segmentName);
            segmentJson.Add("description", segmentDescription);
            segmentJson.Add("expiration", segmentExpiration);
            segmentJson.Add("isTest", segmentIsTest);

            //On affiche le message
            Console.Write(segmentJson + "\n");

            return (segmentJson);
        }

        static JObject getNewActionDuplicateJson(JObject actionDuplicateJson, String segmentId)
        {

            int[] segmentList = { Convert.ToInt32(segmentId) };

            JArray segmentArray = new JArray();
            segmentArray.Add(segmentList);

            //Modification de la nouvelle action
            JObject segments = new JObject();
            segments.Add("selected", segmentArray);

            JObject scheduler = new JObject();
            scheduler.Add("type", "asap");	//Envoi : immediat = 'asap' / Date = 'scheduled'
            //scheduler.Add("type", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever
            scheduler.Add("segments", segments);

            actionDuplicateJson.Remove("scheduler");
            actionDuplicateJson.Add("scheduler", scheduler);

            //On affiche le message
            Console.Write(actionDuplicateJson + "\n");

            return (actionDuplicateJson);
        }

        static JObject getActionValidation(String segmentId)
        {
            JArray segmentList = new JArray();
            segmentList.Add(segmentId);

            JObject actionValidationJson = new JObject();
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


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////FIN DES CREATION DES JSONS//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////DEBUT DU PROGRAMME//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        static void Main(String[] args)
        {
            //Url de base
            String urlBase = "http://v8.mailperformance.com/";

            //Ici, renseignez l'email de la cible, la X-Key
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String unicity = "test@test.com";	//L'adresse mail de la cible a mettre dans le segment ("null" pour ne rien ajouter au segment)
            String segmentId = "0123";	//Id du segment a modifier ("null" si le segment est a creer)
            String actionId = "000ABC";	//Id de l'action a dupliquer

            //Ne pas remplir
            String targetId = null;
            Object[] result = null;
            int response = 0;
            HttpWebRequest con = null;
            HttpWebResponse httpResponse = null;
            String responseString = null;

            //Lancement de la connexion pour remplir la requete
            String url = urlBase + "targets?unicity=" + unicity;

            result = allConnection(con, url, xKey, null, "GET");
            response = (int)result[0];
            con = (HttpWebRequest)result[1];
            httpResponse = (HttpWebResponse)result[2];
            responseString = (String)result[3];

            Console.Write("Target : \n" + responseString + "\n\n");
            
            result = createTarget(response, urlBase, con, xKey, unicity, result);
            con = (HttpWebRequest)result[1];
            httpResponse = (HttpWebResponse)result[2];
            targetId = ((dynamic)(JsonConvert.DeserializeObject((String)result[3]))).id;

            result = createSegment(urlBase, segmentId, con, xKey, result);
            con = (HttpWebRequest)result[1];
            segmentId = ((dynamic)(JsonConvert.DeserializeObject((String)result[3]))).id;

            result = targetToSegment(urlBase, targetId, segmentId, con, xKey, result);
            con = (HttpWebRequest)result[1];
            responseString = (String)result[3];

            result = duplicateAction(urlBase, actionId, targetId, con, xKey, result);
            con = (HttpWebRequest)result[1];
            String actionDuplicateId = ((dynamic)(JsonConvert.DeserializeObject((String)result[3]))).id;
            JObject actionDuplicateJson = ((dynamic)(JsonConvert.DeserializeObject((String)result[3])));

            result = updateActionDuplicate(urlBase, actionDuplicateId, actionDuplicateJson, segmentId, con, xKey, result);

            result = testActionDuplicate(urlBase, actionDuplicateId, segmentId, con, xKey, result);
            httpResponse = (HttpWebResponse)result[2];

            Console.ReadLine();
            httpResponse.Close();
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////FIN DU PROGRAMME////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////DEBUT DES FONCTIONS/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public static Object[] createTarget(int response, String urlBase, HttpWebRequest con, String xKey, String unicity, Object[] result)
    	{
            String url = urlBase + "targets/";

            if (response == 404)
            {
                //La cible n'existe pas
                Console.Write("Error : 404 : Creation of the target.\n");
                Console.ReadLine();

                result = allConnection(con, url, xKey, getTargetJson(unicity), "POST");
            }
            else if (response == 200)
            {
                //La cible existe
                Console.Write("The target exist.\n");
                Console.ReadLine();

                result = allConnection(con, url, xKey, getTargetJson(unicity), "PUT");
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error request createTarget : " + response);
                Console.ReadLine();
                System.Environment.Exit(1);
            }

            if ((int)result[0] != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error request createTarget : " + (int)result[0]);
                System.Environment.Exit(1);
            }

            Console.Write("Data base changed : \n" + result[3] + "\n\n");

            return (result);
        }

        public static Object[] createSegment(String urlBase, String segmentId, HttpWebRequest con, String xKey, Object[] result)
        {
            //On trouve l'adresse pour la requete
            String url = urlBase + "segments/";

            if (segmentId != null)
            {
                Console.Write("The segment exist.\n");
                Console.ReadLine();

                url = url + segmentId;
                result = allConnection(con, url, xKey, getSegmentJson(), "PUT");
            }
            else
            {
                Console.Write("The segment doesn't exist.\n");
                Console.ReadLine();

                result = allConnection(con, url, xKey, getSegmentJson(), "POST");
            }

            if ((int)result[0] != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error request createSegment : " + (int)result[0]);
                Console.ReadLine();
                System.Environment.Exit(1);
            }

            Console.Write("The segment static  : " + ((dynamic)(JsonConvert.DeserializeObject((String)result[3]))).name + " is created / updated :\n" + (String)result[3] + "\n\n");
            Console.ReadLine();

            return (result);
        }

        public static Object[] targetToSegment(String urlBase, String targetId, String segmentId, HttpWebRequest con, String xKey, Object[] result)
        {
            //Nouvelle url
            String url = urlBase + "targets/" + targetId + "/segments/" + segmentId;

            //Connexion
            result = allConnection(con, url, xKey, null, "POST");

            if ((int)result[0] != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error request 'POST' targetToSegment : " + (int)result[0]);
                Console.ReadLine();
                System.Environment.Exit(1);
            }

            Console.Write("The target (" + targetId + ") is add to the segment (" + segmentId + ").\n\n");
            Console.ReadLine();

            return (result);
        }

        public static Object[] duplicateAction(String urlBase, String actionId, String targetId, HttpWebRequest con, String xKey, Object[] result)
        {
            //Nouvelle url
            String url = urlBase + "actions/" + actionId + "/duplication";

            //On duplique a l'identique
            result = allConnection(con, url, xKey, null, "POST");

            if ((int)result[0] != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error request 'POST' duplicateAction : " + (int)result[0]);
                Console.ReadLine();
                System.Environment.Exit(1);
            }

            Console.Write("The action duplicate ( " + ((dynamic)(JsonConvert.DeserializeObject((String)result[3]))).name + " ) is created :\n" + (String)result[3] + "\n\n");
            Console.ReadLine();

            return (result);
        }

        public static Object[] updateActionDuplicate(String urlBase, String actionDuplicateId, JObject actionDuplicateJson, String segmentId, HttpWebRequest con, String xKey, Object[] result)
        {
            //Nouvelle url
		    String url = urlBase + "actions/" + actionDuplicateId;

            result = allConnection(con, url, xKey, getNewActionDuplicateJson(actionDuplicateJson, segmentId), "PUT");

            if ((int)result[0] != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error request 'PUT' updateActionDuplicate : " + (int)result[0]);
                Console.ReadLine();
                System.Environment.Exit(1);
            }

            Console.Write("The action duplicate ( " + ((dynamic)(JsonConvert.DeserializeObject((String)result[3]))).name + " ) is update :\n" + (String)result[3] + "\n\n");

            return (result);
        }

        public static Object[] testActionDuplicate(String urlBase, String actionDuplicateId, String segmentId, HttpWebRequest con, String xKey, Object[] result)
        {
            //Nouvelle url
            String url = urlBase + "actions/" + actionDuplicateId + "/validation";

            //Lancement de la connexion
            result = allConnection(con, url, xKey, getActionValidation(segmentId), "POST");

            if ((int)result[0] != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error request 'PUT' updateActionDuplicate : " + (int)result[0]);
                Console.ReadLine();
                System.Environment.Exit(1);
            }

            int actionState = waitForState(actionDuplicateId, xKey, urlBase);

            if (actionState == 20)
            {
                //Affichage de l'erreur
                Console.Write("Error : the test failed.");
            }
            else
            {
                //La phase de test a reussi
                Console.Write("The action is tested.\n\n");
            }

            return (result);
        }

        //Fonction de connexion
        static HttpWebRequest Connect(String url, String xKey, String method)
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
        static Object[] allConnection(HttpWebRequest con, String url, String xKey, JObject jsonMessage, String method)
        {
            con = Connect(url, xKey, method);
            con.ContentLength = 0;
            if (jsonMessage != null)
            {
                con.ContentLength = jsonMessage.ToString().Length;
                var streamWriter = new StreamWriter(con.GetRequestStream());
                streamWriter.Write(jsonMessage);
                streamWriter.Flush();
                streamWriter.Close();
            }

            //Test de l'envoi
            HttpWebResponse httpResponse = null;
            int response = 0;
            String responseString = null;
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

        static int waitForState(String idAction, String xKey, String urlBase)
        {
            int actionState = 30;

            while (actionState != 38 && actionState != 20)
            {
                //On attend 20 secondes
                Console.Write("Wait 20sec...\n");
                System.Threading.Thread.Sleep(20000);

                //Nouvelle adresse
                String url = urlBase + "actions/" + idAction;

                //Lancement de la connexion pour remplir la requete
                HttpWebRequest con = Connect(url, xKey, "GET");

                //Test de l'envoi
                HttpWebResponse httpResponse = null;
                int response = 0;
                String responseString = null;
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

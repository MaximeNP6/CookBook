using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace duplicateAndValidate
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ici, renseignez la xKey
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            String unicity = "test@test.com";	//Email de la cible
            int[] idSegment = { 0123 };	//Id du segment
            String idAction = "000ABC";	//Id de l'action a dupliquer
            int[] idTestSegment = { 0123 };	//Id du segment de test

            //On cherche la cible a ajouter dans le segment

            //Nouvelle url
            String url = "http://v8.mailperformance.com/targets?unicity=" + unicity;

            //Lancement de la connexion pour remplir la requete
            Object[] connection = allConnection(url, xKey, null, "GET");
            int response = (int)connection[0];
            HttpWebRequest con = (HttpWebRequest)connection[1];
            HttpWebResponse httpResponse = (HttpWebResponse)connection[2];
            String responseString = (String)connection[3];

            //Verification des reponses
            if (response != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error request 'GET' : " + response);
            }
            else
            {
                //On affiche la cible
                Console.Write(responseString + "\n");

                //On recupere l'id de la cible
                dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                string idTarget = responseJson.id;

                //Nouvelle url
                url = "http://v8.mailperformance.com/targets/" + idTarget + "/segments/" + idSegment[0];

                //On ajoute la cible au segment
                connection = allConnection(url, xKey, null, "POST");
                response = (int)connection[0];
                con = (HttpWebRequest)connection[1];
                httpResponse = (HttpWebResponse)connection[2];
                responseString = (String)connection[3];

                //Verification des reponses
                if (response != 200)
                {
                    //Affichage de l'erreur
                    Console.Write("Error request 'POST' : " + response);
                }
                else
                {
                    Console.Write("The target " + unicity + " is added to the segment " + idSegment[0] + ".\n\n");

                    //Duplication de l'action

                    //Nouvelle url
                    url = "http://v8.mailperformance.com/actions/" + idAction + "/duplication";

                    //On duplique a l'identique
                    connection = allConnection(url, xKey, null, "POST");
                    response = (int)connection[0];
                    con = (HttpWebRequest)connection[1];
                    httpResponse = (HttpWebResponse)connection[2];
                    responseString = (String)connection[3];

                    //Verification des reponses
                    if (response != 200)
                    {
                        //Affichage de l'erreur
                        Console.Write("Error request 'POST' duplication : " + response);
                    }
                    else
                    {
                        //On affiche la cible
                        Console.Write("The action duplicate ( " + (responseJson = JsonConvert.DeserializeObject(responseString)).name + " ) is created.\n\n");
                        Console.Write(responseString + "\n");

                        //On recupere l'id de l'action
                        responseJson = JsonConvert.DeserializeObject(responseString);
                        String idNewAction = responseJson.id;

                        JObject data = new JObject();
                        data = responseJson;

                        JArray segmentArray = new JArray();
                        segmentArray.Add(idSegment);

                        //Modification de la nouvelle action
                        JObject segments = new JObject();
                        segments.Add("selected", segmentArray);

                        JObject scheduler = new JObject();
                        scheduler.Add("type", "asap");	//Envoi : immediat = 'asap' / Date = 'scheduled'
                        //scheduler.Add("startDate", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever
                        scheduler.Add("segments", segments);

                        data.Remove("scheduler");
                        data.Add("scheduler", scheduler);


                        //Nouvelle url
                        url = "http://v8.mailperformance.com/actions/" + idNewAction;

                        //On modifie l'action
                        connection = allConnection(url, xKey, data, "PUT");
                        response = (int)connection[0];
                        con = (HttpWebRequest)connection[1];
                        httpResponse = (HttpWebResponse)connection[2];
                        responseString = (String)connection[3];

                        String name = (responseJson = JsonConvert.DeserializeObject(responseString)).name;

                        //Verification des reponses
                        if (response != 200)
                        {
                            //Affichage de l'erreur
                            Console.Write("Error request 'PUT' duplication : " + response);
                        }
                        else
                        {
                            Console.Write("The action duplicate ( " + name + " ) is update.\n\n");

                            //On test l'action

                            //Nouvelle url
                            url = "http://v8.mailperformance.com/actions/" + idNewAction + "/validation";

                            JArray testSegments = new JArray();
                            testSegments.Add(idTestSegment);

                            JObject jsonTest = new JObject();
                            jsonTest.Add("fortest", true);	//Phase de test
                            jsonTest.Add("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
                            jsonTest.Add("testSegments", testSegments);	//Les Ids des differents segments de test
                            jsonTest.Add("mediaForTest", null);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
                            jsonTest.Add("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
                            jsonTest.Add("comments", null);	//Commentaire ('NULL' pour aucuns commentaires)

                            //On affiche le json
                            Console.Write(jsonTest + "\n");

                            //Lancement de la connexion
                            connection = allConnection(url, xKey, jsonTest, "POST");
                            response = (int)connection[0];
                            con = (HttpWebRequest)connection[1];
                            httpResponse = (HttpWebResponse)connection[2];
                            responseString = (String)connection[3];

                            int actionState = waitForState(idNewAction, xKey);

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
                                jsonValid.Add("fortest", false);	//Phase de test
                                jsonValid.Add("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
                                jsonValid.Add("testSegments", null);	//Les Ids des differents segments de tests
                                jsonValid.Add("mediaForTest", null);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
                                jsonValid.Add("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
                                jsonValid.Add("comments", null);	//Commentaire ('NULL' pour aucun commentaire)

                                //On affiche le json
                                Console.Write(jsonValid + "\n");

                                //Lancement de la connexion
                                connection = allConnection(url, xKey, jsonValid, "POST");
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
                        }
                    }
                }
            }

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

        //Fonction de connexion et envoie des informations
        static Object[] allConnection(String url, String xKey, JObject jsonMessage, String method)
        {
            HttpWebRequest con = Connect(url, xKey, method);
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

        static int waitForState(String idAction, String xKey)
        {
            int actionState = 30;

            while (actionState != 38 && actionState != 20)
            {
                //On attend 20 secondes
                Console.Write("Wait 20sec...\n");
                System.Threading.Thread.Sleep(20000);

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

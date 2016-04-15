using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///  This class is the one with all connection function.
/// </summary>
public class Utils
	{
		
        /// <summary>
        ///  This function allow us to connect to the url.
        /// </summary>
        static HttpWebRequest Connect(string url, string xKey, string method)
        {
            //Lancement de la connexion pour remplir la requete
            var con = (HttpWebRequest)WebRequest.Create(url);
            con.Method = method;

            //Mise en place du xKey et des options
            con.Headers.Add("X-Key", xKey);
            con.ContentType = "application/json";

            return (con);
        }

        /// <summary>
        ///  Fonction de connexion et envoi des informations
        /// </summary>
        public static object[] allConnection(string url, string xKey, JObject jsonMessage, string method)
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

            // Test de l'envoi
            HttpWebResponse httpResponse = null;
            var response = 0;
            string responseString = null;
            try
            {
                httpResponse = (HttpWebResponse)con.GetResponse();
                response = 200;

                // Lecture des donnees
                var reader = new StreamReader(con.GetResponse().GetResponseStream());
                responseString = reader.ReadToEnd();
                httpResponse.Close();
                reader.Close();
            }
            // Reception du signal
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    httpResponse = (HttpWebResponse)ex.Response;
                    response = (int)httpResponse.StatusCode;
                }
            }
            object[] result = { response, con, httpResponse, responseString };
            return (result);
        }

        /// <summary>
        ///  Fonction d'attente de changement d'état de l'action
        /// </summary>
        public static int waitForState(string idAction, string xKey, string baseUrl)
        {
            var actionState = 30;

            while (actionState != 38 && actionState != 20 && actionState != 10)
            {
                // On attend 20 secondes
                Console.Write("Wait 20sec...\n");
                System.Threading.Thread.Sleep(20000);

                // Nouvelle adresse
                var url = baseUrl + "actions/" + idAction;

                // Lancement de la connexion pour remplir la requete
                var result = allConnection(url, xKey, null, "GET");
                if ((int)result[0] == 200)
                {
                    // On recupere l'etat de l'action
                    dynamic responseJson = JsonConvert.DeserializeObject((string)result[3]);
                    actionState = responseJson.informations.state;
                }
                else
                    actionState = 20;
            }
            return (actionState);
        }

    }

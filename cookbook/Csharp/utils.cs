using System;
using System.IO;
using System.Net;
using System.Text;
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
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            //Mise en place du xKey et des options
            request.Headers.Add("X-Key", xKey);
            request.ContentType = "application/json";

            return (request);
        }

        /// <summary>
        ///  Fonction de connexion et envoi des informations
        /// </summary>
        public static object[] allConnection(string url, string xKey, dynamic jsonMessage, string method)
        {
            var request = Connect(url, xKey, method);
            request.ContentLength = 0;
            if (jsonMessage != null)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonMessage.ToString());
                request.ContentLength = byteArray.Length;
                var dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            // Test de l'envoi
            HttpWebResponse httpResponse = null;
            var response = 0;
            string responseString = null;
            try
            {
                httpResponse = (HttpWebResponse)request.GetResponse();
                response = 200;

                // Lecture des donnees
                var reader = new StreamReader(request.GetResponse().GetResponseStream());
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
            object[] result = { response, request, httpResponse, responseString };
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

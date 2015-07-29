using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace sendHTML
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ici, renseignez l'email de la cible, la xKey, l'id du message et les parametres personnalises
            string unicity = "test@test.com";
            string xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string idMessage = "000ABC";

            string htmlMessage = "html message for Csharp";	//Si vous ne voulez pas de cette option, effacer la ligne de creation du Json 
            string textMessage = "text message for Csharp";	//Si vous ne voulez pas de cette option, effacer la ligne de creation du Json 
            string subject = "subject for Csharp";
            string mailFrom = "mail@address.com";
            string replyTo = "mail@return.com";

            //Appel de la fonction Connect
            string url = "http://v8.mailperformance.com/targets?unicity=" + unicity;
            HttpWebRequest con = Connect(url, xKey, "GET");

            //Teste de l'envoie
            HttpWebResponse httpResponseGet = null;
            int response = 0;
            try
            {
                httpResponseGet = (HttpWebResponse)con.GetResponse();
                response = 200;
            }
            //Reception du signal 
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    httpResponseGet = (HttpWebResponse)ex.Response;
                    response = (int)httpResponseGet.StatusCode;
                }
            }

            //Verification des reponses
            if (response != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error : " + response);
            }
            else
            {
                //Lecture des donnees
                StreamReader reader = new StreamReader(con.GetResponse().GetResponseStream());
                string responseString = reader.ReadToEnd();
                httpResponseGet.Close();
                reader.Close();

                //On affiche la cible
                Console.Write(responseString + "\n");
                Console.ReadLine();

                //On prend le idTarget
                dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                string idTarget = responseJson.id;

                //Nouvelle url en fonction de l'id du message et de la cible
                url = "http://v8.mailperformance.com/actions/" + idMessage + "/targets/" + idTarget;

                //Creation du message en Json
                JObject content = new JObject();
                content.Add("html", htmlMessage);	//Si vous ne voulez pas de l'option : htmlMessage, effacer cette ligne
                content.Add("text", textMessage);	//Si vous ne voulez pas de l'option : textMessage, effacer cette ligne

                JObject header = new JObject();
                header.Add("subject", subject);
                header.Add("mailFrom", mailFrom);
                header.Add("replyTo", replyTo);

                JObject jsonMessage = new JObject();
                jsonMessage.Add("content", content);
                jsonMessage.Add("header", header);


                Console.Write(jsonMessage);
                Console.ReadLine();


                //Lancement de la connexion pour remplir la requete
                con = Connect(url, xKey, "POST");
                con.ContentLength = jsonMessage.ToString().Length;
                var streamWriter = new StreamWriter(con.GetRequestStream());
                streamWriter.Write(jsonMessage);
                streamWriter.Flush();
                streamWriter.Close();

                //Teste de l'envoie
                HttpWebResponse httpResponse = null;
                response = 0;
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
                    Console.Write("\nMessage sent to " + unicity);
                }
                httpResponse.Close();

                //Attente de lecture (optionnel)
                Console.ReadLine();

                httpResponseGet.Close();
            }
        }

        //Fonctions ----

		
		//Fonction de connection
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
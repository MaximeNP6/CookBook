using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace createSegmentDynamic
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ici, renseignez la xKey et les parametres personnalises du message
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String segmentId = "0123";	//Id du segment a modifier ('null' si le segment est a creer)

            String type = "dynamic";	//Code pour creer un segment Dynamique
            String name = "SegmentDynamic (Csharp)";	//Nom du segment
            String description = "SegmentDynamic (Csharp)";	//Description du segment
            String expiration = "2016-01-08T12:11:00Z";	//Date d'expiration du segment
            Boolean isTest = false;	//Le test est un segment de test : oui = 'true' / non = 'false'
            String parentId = null;	//Id du segment pere ('null' pour aucun segments pere)


            //On trouve l'addresse pour la requete
            String url = "http://v8.mailperformance.com/segments/";
            if (segmentId != null)
            {
                url = "http://v8.mailperformance.com/segments/" + segmentId;
            }

            //Creation du Json du message
            JObject jsonData = new JObject();
            jsonData.Add("type", type);
            jsonData.Add("name", name);
            jsonData.Add("description", description);
            jsonData.Add("expiration", expiration);
            jsonData.Add("isTest", isTest);
            jsonData.Add("parentId", parentId);

            //On affiche le message
            Console.Write(jsonData + "\n");


            //Lancement de la connexion pour remplir la requete
            Object[] connection = null;

            if (segmentId != null)
            {
                connection = allConnection(url, xKey, jsonData, "PUT");
            }
            else
            {
                connection = allConnection(url, xKey, jsonData, "POST");
            }

            int response = (int)connection[0];
            HttpWebRequest con = (HttpWebRequest)connection[1];
            HttpWebResponse httpResponse = (HttpWebResponse)connection[2];
            String responseString = (String)connection[3];

            //Verification des reponses
            if (response != 200)
            {
                //Affichage de l'erreur
                Console.Write("\nError : " + response + "\n\n");
            }
            else
            {
                //Le segment a bien ete modifier
                Console.Write("The segment dynamic  : " + name + " is update.");
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
    }
}
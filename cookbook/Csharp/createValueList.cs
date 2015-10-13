using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace createValueList
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ici, renseignez la xKey et les parametres personnalises
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String valueListsId = "012";	//Id de la liste a modifier ('null' si le critere doit etre creer)

            String name = "createValueList (Csharp)";	//Nom de la liste
            Boolean ordered = false;	//Ordonnee : oui = 'true' / non = 'false'

            JArray values = new JArray();	//Valeurs de la liste
                JObject value1 = new JObject();	//Premiere valeur
                    value1.Add("index", 1);	//Numeros de la valeur
                    value1.Add("value", "Mr");	//Valeur du champs
                JObject value2 = new JObject();	//Premiere valeur
                    value2.Add("index", 2);	//Numeros de la valeur
                    value2.Add("value", "Mme");	//Valeur du champs
                values.Add(value1);
                values.Add(value2);


            //On trouve l'adresse pour la requete
            String url = "http://v8.mailperformance.com/valueLists/";
            if (valueListsId != null)
            {
                url = "http://v8.mailperformance.com/valueLists/" + valueListsId;
            }
            //Creation du Json du message
            JObject jsonData = new JObject();
            jsonData.Add("name", name);
            jsonData.Add("ordered", ordered);
            jsonData.Add("values", values);

            //On affiche le message
            Console.Write(jsonData + "\n");


            //Lancement de la connexion pour remplir la requete
            Object[] connection = null;

            if (valueListsId != null)
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
                //La liste de valeur a bien ete cree
                Console.Write("The value list  : " + name + " is created / update.");
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
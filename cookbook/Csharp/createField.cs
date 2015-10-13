using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace createField
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ici, renseignez la xKey et les parametres personnalises du message
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String fieldId = "0123";	//Id du critere a modifier ('null' si le critere doit etre creer)

            String type = "numeric";	//Code pour creer un critere :
				//Normales : email = 'email' / telephone = 'phone' / zone de texte = 'textArena' / chaine de caractere = 'textField' / valeur numerique = 'numeric' / date = 'date'
				//Listes : liste de valeurs = 'singleSelectList' / Liste de valeurs multiples = 'multipleSelectList'
            String name = "createField (Csharp)";	//Nom du critere
            Boolean isUnicity = false;	//Unicite : oui = 'true' / non = 'false'
            Boolean isMandatory = false;	//Obligatoire : oui = 'true' / non = 'false'

            int constraintOperator = 0;	//Pour les valeurs numeriques et les dates : 1 = '>' / 2 = '<' / 3 = '>=' / 4 = '<=' / 5 = '==' ('0' pour rien)
            String constraintValue = "42";	//Valeur de la contrainte ('null' pour rien)

            int valueListId = 0;	//Id de la liste dans le cas d'un 'singleSelectList' ou un 'multipleSelectList' ('0' pour rien)


            //On trouve l'adresse pour la requete
            String url = "http://v8.mailperformance.com/fields/";
            if (fieldId != null)
            {
                url = "http://v8.mailperformance.com/fields/" + fieldId;
            }
            //Creation du Json du message
            JObject jsonData = new JObject();
            jsonData.Add("type", type);
            jsonData.Add("name", name);
            jsonData.Add("isUnicity", isUnicity);
            jsonData.Add("isMandatory", isMandatory);

            if (constraintOperator != 0 && constraintValue != null)
            {
                JObject constraint = new JObject();
                constraint.Add("operator", constraintOperator);
                constraint.Add("value", constraintValue);
                jsonData.Add("constraint", constraint);
            }
            else if (valueListId != 0)
            {
                jsonData.Add("valueListId", valueListId);
            }

            //On affiche le message
            Console.Write(jsonData + "\n");


            //Lancement de la connexion pour remplir la requete
            Object[] connection = null;

            if (fieldId != null)
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
                //Le critere a bien ete cree
                Console.Write("The field  : " + name + " is created / update.");
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
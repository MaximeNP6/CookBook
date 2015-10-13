using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace createTargetAndAddToSegment
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ici, renseignez l'email de la cible, la X-Key et l'id du segment
            String unicity = "test@test.com";
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String idSegment = "7501";

            //Syntaxe pour les differents types d'informations :
            String chain = "name";	//Chaine de caracteres
            String listOfValues = "Mr";	//Liste de valeurs
            String email = "test@test.com";	//E-mail
            String phoneNumber = "0123456789";	//Telephone
            String textZone = "150 caracters max";	//Zone de texte
            int numbers = 123;	//Valeur numerique
            String date = "01/01/2000";	//Date
            JProperty listMultipleValues = new JProperty("5456", "valeur 1", "valeur 2");	//Liste de valeurs multiples : particulier, la premiere valeur est l'id-champ, les autres sont les valeurs de ce champ.

            //Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur du champ"
            JObject data = new JObject();
            data.Add("5398", chain);
            data.Add("5399", listOfValues);
            data.Add("5400", email);
            data.Add("5452", phoneNumber);
            data.Add("5453", textZone);
            data.Add("5454", numbers);
            data.Add("5455", date);
            data.Add(listMultipleValues);   //Liste de valeurs multiples : particulier, la premiere valeur est l'id-champ, les autres sont les valeurs de ce champ.


            //Lancement de la connexion pour remplir la requete 'GET'
            string url = "http://v8.mailperformance.com/targets?unicity=" + unicity;
            HttpWebRequest con = Connect(url, xKey, "GET");

            //Test de l'envoie
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
            if (response == 404 || response == 200)
            {
                if (response == 404)
                {
                    //La cible n'existe pas
                    Console.Write("Error : 404 : Creation of the target.");
                    Console.ReadLine();

                    //Requette 'POST' pour creer la cible
                    url = "http://v8.mailperformance.com/targets/";
                    con = postOrPutOnTarget(con, "POST", data, xKey, url);
                }
                //La cible existe
                StreamReader reader = new StreamReader(con.GetResponse().GetResponseStream());
                string responseString = reader.ReadToEnd();
                httpResponseGet.Close();
                reader.Close();

                //On prend le idTarget
                dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                String idTarget = responseJson.id;

                Console.Write(responseString);
                Console.ReadLine();

                //On remplit la requete 'POST'
                url = "http://v8.mailperformance.com/targets/" + idTarget + "/segments/" + idSegment;
                con = postOrPutOnTarget(con, "POST", data, xKey, url);
                Console.Write("The target " + unicity + " is added to the segment " + idSegment + ".");
            }
            else
            {
                //Erreur
                Console.Write("Error : " + response);
            }

            //Attente de lecture (optionnel)
            Console.ReadLine();

            httpResponseGet.Close();
        }



        //Fonctions---------
		


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

        static HttpWebRequest postOrPutOnTarget(HttpWebRequest con, String request, JObject data, String xKey, String url)
        {
            //Lancement de la connexion pour remplir la requete
            con = Connect(url, xKey, request);
            con.ContentLength = data.ToString().Length;
            var streamWriter = new StreamWriter(con.GetRequestStream());
            streamWriter.Write(data);
            streamWriter.Flush();
            streamWriter.Close();

            //Test de l'envoie
            HttpWebResponse httpResponsePost = null;
            int response = 0;
            try
            {
                httpResponsePost = (HttpWebResponse)con.GetResponse();
                response = 200;
            }
            //Reception du signal 
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    httpResponsePost = (HttpWebResponse)ex.Response;
                    response = (int)httpResponsePost.StatusCode;
                }
            }

            //Verification des reponses
            if (response != 200 && response != 204)
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {0}", response, httpResponsePost.StatusCode.ToString());
                return (null);
            }
            else
            {
                return (con);
            }
        }
    }
}
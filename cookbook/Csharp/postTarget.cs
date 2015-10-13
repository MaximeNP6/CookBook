using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;


namespace postTarget
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ici, renseignez l'email et la X-Key
            string unicity = "test@test.com";
            string xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

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


            //Appel de la fonction Connect
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
            if (response == 404)
            {
                //La cible n'existe pas
                Console.Write("Error : 404 : Creation of the target.");
                Console.ReadLine();

                con = postOrPutOnTarget(con, "POST", data, xKey);
            }
            else if (response == 200)
            {
                //La cible existe
                StreamReader reader = new StreamReader(con.GetResponse().GetResponseStream());
                string responseString = reader.ReadToEnd();
                httpResponseGet.Close();
                reader.Close();

                Console.Write(responseString);
                Console.ReadLine();

                con = postOrPutOnTarget(con, "PUT", data, xKey);
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



        //Fonctions



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

        static HttpWebRequest postOrPutOnTarget(HttpWebRequest con, String request, JObject data, String xKey)
        {
            //Nouvelle url
            String url = "http://v8.mailperformance.com/targets/";

            //Lancement de la connexion pour remplir la requete
            con = Connect(url, xKey, request);
            con.ContentLength = data.ToString().Length;
            var streamWriter = new StreamWriter(con.GetRequestStream());
            streamWriter.Write(data);
            streamWriter.Flush();
            streamWriter.Close();

            //Verification des reponses
            HttpWebResponse httpResponsePost = (HttpWebResponse)con.GetResponse();
            if ((int)httpResponsePost.StatusCode != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {0}", (int)httpResponsePost.StatusCode, httpResponsePost.StatusCode.ToString());
            }
            else
            {
                //Lecture des donnees
                StreamReader reader = new StreamReader(con.GetResponse().GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();

                //On affiche la nouvelle base
                Console.Write("\n" + responseString + "\n\nData bas changed.");
            }
            httpResponsePost.Close();
            return (con);
        }

    }
}

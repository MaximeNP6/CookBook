using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SendMessageOnSegment
{
    class Program
    {
        static void Main(string[] args)
        {
            //Url de base
            String urlBase = "http://v8.mailperformance.com/";

            //X-Key
            String xKey = "ABCDEFJHIJKLMNOPQRSTUVWXYZ0123456789";

            String segmentId = "1234";	//Id du segment
            String actionId = "00012A";	//Id de l'action a dupliquer

            //Ne pas remplir
            Object[] result = null;
            HttpWebRequest con = null;
            HttpWebResponse httpResponse = null;
            String responseString = null;

            //Lancement de la connexion pour remplir la requete
            String url = urlBase + "segments/" + segmentId + "/targets/";
            Console.Write("Url : " + url + "\n");

            result = allConnection(con, url, xKey, null, "GET");
            if (verification(result) == false)
                System.Environment.Exit(0);
            con = (HttpWebRequest)result[1];
            httpResponse = (HttpWebResponse)result[2];
            responseString = (String)result[3];
            Console.Write("OK : " + responseString + "\n\n");

            String[] idTargets = myTab(responseString);

            //Pour chaque Target on fait un SendMessage
            for (int i = 0; i < idTargets.Length; i++)
            {
                //Nouvelle url
                url = urlBase + "actions/" + actionId + "/targets/" + idTargets[i];
                //SendMessage
                result = allConnection(con, url, xKey, null, "POST");
                Console.Write("Url : " + url + "\n");
                if (verification(result) == false)
                    System.Environment.Exit(0);
                Console.Write("OK\n\n");
            }

            Console.ReadLine();
            httpResponse.Close();
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////FIN DU PROGRAMME////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////DEBUT DES FONCTIONS/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static String[] myTab(String result)
        {
            String str = "";

            //On transforme la string en tableau de char
            char[] charArray = result.ToCharArray();

            //On regarde tout le tableau de char
            for (int i = 0; i < charArray.Length; i++)
            {
                //Si un caractere et bon, on le garde
                if (((int)(charArray[i]) >= (int)'0' && (int)(charArray[i]) <= (int)'9')
                        || ((int)(charArray[i]) >= (int)'A' && (int)(charArray[i]) <= (int)'Z')
                        || charArray[i] == ',')
                {
                    str = str + charArray[i];
                }
            }
            //On transforme la string en tableau de string
            String[] tab = str.Split(new[]{","}, StringSplitOptions.RemoveEmptyEntries);

            //On return
            return (tab);
        }

        static Boolean verification(Object[] result)
        {
            int response = 0;
            HttpWebRequest con = null;
            HttpWebResponse httpResponse = null;
            String responseString = null;

            response = (int)result[0];
            con = (HttpWebRequest)result[1];
            httpResponse = (HttpWebResponse)result[2];
            responseString = (String)result[3];

            //Verification des reponses
            if (response >= 200 && response <= 299)
                return (true);

            //Affichage de l'erreur
            Console.Write("Error : " + response + " " + responseString + "\n");
            httpResponse = (HttpWebResponse)result[2];
            httpResponse.Close();
            Console.ReadLine();
            return (false);
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
        static Object[] allConnection(HttpWebRequest con, String url, String xKey, Object jsonMessage, String method)
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

            //Test de l'envoie
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
    }
}

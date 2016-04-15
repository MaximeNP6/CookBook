using System;
using System.IO;
using System.Net;
using System.Xml;
using Newtonsoft.Json;

namespace SendMessageOnSegment
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Changez le Path pour correspondre à la destination de votre fichier de configuration
            var doc = new XmlDocument();
            doc.Load("config.xml");

            var xKey = doc.DocumentElement.SelectSingleNode("/config/xKey").InnerText;
            var baseUrl = doc.DocumentElement.SelectSingleNode("/config/url").InnerText;

            const string segmentId = "01234";	//Id du segment
            const string actionId = "000000";	//Id de l'action a dupliquer

            //Ne pas remplir
            HttpWebResponse httpResponse = null;
            String responseString = null;

            //Lancement de la connexion pour remplir la requete
            String url = baseUrl + "segments/" + segmentId + "/targets/";
            Console.Write("Url : " + url + "\n");

            var result = Utils.allConnection(url, xKey, null, "GET");
            ((HttpWebResponse)result[2]).Close();
            //Verification des reponses
            if ((int)result[0] != 200)
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)result[0], ((HttpWebResponse)result[2]).StatusCode.ToString());
            }

            Console.Write("OK : " + (string)result[3] + "\n\n");

            var targets = JsonConvert.DeserializeObject<string[]>((string)result[3]);

            //Pour chaque Target on fait un SendMessage
            foreach (var target in targets)
            {
                //Nouvelle url
                url = baseUrl + "actions/" + actionId + "/targets/" + target;
                //SendMessage
                result = Utils.allConnection(url, xKey, null, "POST");
                ((HttpWebResponse)result[2]).Close();
                Console.Write("Url : " + url + "\n");
                if ((int)result[0] == 200)
                {
                    Console.Write("OK\n\n");
                    Console.WriteLine(target);
                }
                else
                {
                    //Affichage de l'erreur
                    Console.Write("Error : {0} {1}", (int) result[0],
                        ((HttpWebResponse) result[2]).StatusCode.ToString());
                }
            }
            Console.WriteLine("Mail sent to the segment");
            Console.ReadLine();
        }
    }
}

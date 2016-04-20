using System;
using System.Net;
using System.Xml;

namespace getImports
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

            // Lancement de la connexion pour remplir la requete
            var url = baseUrl + "imports/";

            var result = Utils.allConnection(url, xKey, null, "GET");

            //Verification des reponses
            if ((int)result[0] == 200)
            {
                Console.Write("{0}", (string)result[3]);
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)result[0], ((HttpWebResponse)result[2]).StatusCode.ToString());
            }

            Console.ReadLine();
            ((HttpWebResponse)result[2]).Close();
        }
    }
}

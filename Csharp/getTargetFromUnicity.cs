using System;
using System.IO;
using System.Net;
using System.Xml;

namespace getTargetFromUnicity
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
            
            //Ici, renseignez l'email et la X-Key
            const string unicity = "test@test.com";
            
            //Lancement de la connexion pour remplir la requete
            var url = baseUrl + "targets?unicity=" + unicity;

            var reponse = Utils.allConnection(url, xKey, null, "GET");

            //Verification des reponses
            if ((int)reponse[0] == 200)
            {
                Console.Write("{0}", (string)reponse[3]);
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int)reponse[0], ((HttpWebResponse)reponse[2]).StatusCode.ToString());
            }

            //Attente de lecture (optionnel)
            Console.ReadLine();

            ((HttpWebResponse)reponse[2]).Close();
        }
    }
}

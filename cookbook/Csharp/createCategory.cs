using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace createCategory
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

            string categoriesId = null;	// Id de la categorie a modifier ('null' si la categorie doit etre cree)

            var name = "Category Csharp";	// Nom de la categorie
            var description = "Category (Csharp)";	// Description de la categorie


            // On trouve l'adresse pour la requete
            var url = baseUrl + "categories/" + categoriesId;

            Console.Write(xKey + "\n");

            // Creation du Json du message
            var jsonData = new JObject();
            jsonData.Add("name", name);
            jsonData.Add("description", description);

            // On affiche le message
            Console.Write(jsonData + "\n");


            // Lancement de la connexion pour remplir la requete
            var connection = Utils.allConnection(url, xKey, jsonData, (segmentId == null ? "POST" : "PUT"));
            
            // Verification des reponses
            if ((int)connection[0] != 200)
            {
                // Affichage de l'erreur
                Console.Write("\nError : " + connection[0]);
            }
            else
            {
                // La categorie a bien ete cree
                Console.Write("The category  : " + name + " is created / update.");
            }
            // Attente de lecture (optionnel)
            Console.ReadLine();

            ((HttpWebResponse)connection[2]).Close();
        }
    }
}

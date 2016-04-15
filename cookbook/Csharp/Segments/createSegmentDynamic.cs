using Newtonsoft.Json.Linq;
﻿using System;
using System.Net;
using System.Xml;

namespace createSegmentDynamic
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

            string segmentId = null; //Id du segment a modifier ('null' si le segment doit etre cree)

            var type = "dynamic"; //Code pour creer un segment Dynamique
            var name = "SegmentDynamic (Csharp)"; //Nom du segment
            var description = "SegmentDynamic (Csharp)"; //Description du segment
            var expiration = "2026-08-08T12:11:00Z"; //Date d'expiration du segment
            var isTest = true; //Le test est un segment de test : oui = 'true' / non = 'false'

            var url = baseUrl + "V1/segments/" + segmentId;

            //Creation du Json du message
            var jsonData = new JObject();
            jsonData.Add("type", type);
            jsonData.Add("name", name);
            jsonData.Add("description", description);
            jsonData.Add("expiration", expiration);
            jsonData.Add("isTest", isTest);

            //On affiche le message
            Console.Write(jsonData + "\n");


            //Lancement de la connexion pour remplir la requete
            var result = Utils.allConnection(url, xKey, jsonData, (segmentId == null ? "POST" : "PUT"));
            
            //Verification des reponses
            if ((int) result[0] == 200)
            {
                Console.Write("{0}", (string) result[3]);
            }
            else
            {
                //Affichage de l'erreur
                Console.Write("Error : {0} {1}", (int) result[0], ((HttpWebResponse) result[2]).StatusCode.ToString());
            }

            Console.ReadLine();
            ((HttpWebResponse) result[2]).Close();
        }
    }
}

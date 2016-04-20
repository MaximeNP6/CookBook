using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace createTargetAndAddToSegment
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

            //Ici, renseignez l'id du segment
            string idSegment = "01234";

            //Syntaxe pour les differents types d'informations :
            var chain = "name";	//Chaine de caracteres
            var listOfValues = "Mr";	//Liste de valeurs
            var email = "test@test.com";	//E-mail
            var phoneNumber = "0123456789";	//Telephone
            var textZone = "150 caracters max";	//Zone de texte
            var numbers = 123;	//Valeur numerique
            var date = "01/01/2000";	//Date
            var listMultipleValues = new JProperty("XXXX", "valeur 1", "valeur 2");	//Liste de valeurs multiples : particulier, la premiere valeur est l'id-champ, les autres sont les valeurs de ce champ.

            //Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur du champ"
            var data = new JObject();
            data.Add("XXXX", chain);
            data.Add("XXXX", listOfValues);
            data.Add("XXXX", email);
            data.Add("XXXX", phoneNumber);
            data.Add("XXXX", textZone);
            data.Add("XXXX", numbers);
            data.Add("XXXX", date);
            data.Add(listMultipleValues);   //Liste de valeurs multiples : particulier, la premiere valeur est l'id-champ, les autres sont les valeurs de ce champ.


            //Lancement de la connexion pour remplir la requete 'GET'
            var url = baseUrl + "targets?unicity=" + email;
            //Requette 'POST' pour creer la cible
            var con = Utils.allConnection(url, xKey, data, "POST");
            //On prend le idTarget
            dynamic responseJson = JsonConvert.DeserializeObject((String)con[3]);
            string idTarget = responseJson.id;

            Console.Write((String)con[3]);

            //On remplit la requete 'POST'
            url = baseUrl + "targets/" + idTarget + "/segments/" + idSegment;
            con = Utils.allConnection(url, xKey, data, "POST");
            HttpWebResponse httpResponse = (HttpWebResponse)con[2];
            Console.Write("The target " + email + " has been added to the segment " + idSegment + ".");
            
            //Attente de lecture (optionnel)
            Console.ReadLine();

            httpResponse.Close();
        }

    }
}

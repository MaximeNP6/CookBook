using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace createField
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
            
            string fieldId = null;	//Id du critere a modifier ('null' si le critere doit etre cree)

            var type = "numeric";	//Code pour creer un critere :
				//Normales : email = 'email' / telephone = 'phone' / zone de texte = 'textArena' / chaine de caractere = 'textField' / valeur numerique = 'numeric' / date = 'date'
				//Listes : liste de valeurs = 'singleSelectList' / Liste de valeurs multiples = 'multipleSelectList'
            var name = "createField (Csharp)";	//Nom du critere
            var isUnicity = false;	//Unicite : oui = 'true' / non = 'false'
            var isMandatory = false;	//Obligatoire : oui = 'true' / non = 'false'

            var constraintOperator = 0;	//Pour les valeurs numeriques et les dates : 1 = '>' / 2 = '<' / 3 = '>=' / 4 = '<=' / 5 = '==' ('0' pour rien)
            var constraintValue = "42";	//Valeur de la contrainte ('null' pour rien)

            var valueListId = 0;	//Id de la liste dans le cas d'un 'singleSelectList' ou un 'multipleSelectList' ('0' pour rien)


            //On trouve l'adresse pour la requete
            var url = baseUrl + "fields/" + fieldId;
            //Creation du Json du message
            var jsonData = new JObject();
            jsonData.Add("type", type);
            jsonData.Add("name", name);
            jsonData.Add("isUnicity", isUnicity);
            jsonData.Add("isMandatory", isMandatory);

            if (constraintOperator != 0 && constraintValue != null)
            {
                var constraint = new JObject();
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
            var connection = Utils.allConnection(url, xKey, jsonData, (segmentId == null ? "POST" : "PUT"));

            var response = (int)connection[0];
            var con = (HttpWebRequest)connection[1];
            var httpResponse = (HttpWebResponse)connection[2];
            var responseString = (string)connection[3];

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
    }
}

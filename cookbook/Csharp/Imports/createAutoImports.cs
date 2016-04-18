using System;
using System.Net;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace createAutoImports
{
    internal class Program
    {
        public static JObject getCreateImportJson()
        {
            // Remplissez les informations obligatoires
            const string importName = "Nom de votre Import automatique"; // Nom de l'import
            const string schedulerName = "Nom du scheduler"; // Nom du scheduler
            const int binding = 0123; // Id du binding
            const int segmentId = 0123; // Id du segment
            const int fieldsId = 0123; // Id du field
            string[] contactsId = { "01234" }; // Id des utilisateurs : Administration -> Utilisateurs -> Identifiant
            int[] groupsContactsId = {  }; // Id des groupes : Administration -> Groupes -> Identifiant

            //Creation du Json


            var source = new JObject();
            source.Add("type", "ftp");

            var occurs = new JObject();
            occurs.Add("type", "daily"); //Quotidient
            /* Autres possibilites :
            occurs.Add("type", "weekly"); // Hebdomadaire
            String days[] = {"Mon", "Tue", "Wed", "Thu", "Fri", "Sat"};
            occurs.Add("days", days);

            occurs.Add("type", "monthly"); // Mensuel
            int days[] = {20};
            occurs.Add("days", days);
            */

            var periodicityValue = new JObject();
            periodicityValue.Add("hour", 6);
            periodicityValue.Add("minute", 0);
            periodicityValue.Add("second", 0);

            var periodicity = new JObject(); // Heure de l'import
            periodicity.Add("type", "once");
            periodicity.Add("value", periodicityValue);

            var frequency = new JObject();
            frequency.Add("occurs", occurs);
            frequency.Add("periodicity", periodicity);

            var validityStart = new JObject(); // Les dates doivent etre coherentes avec la date de creation
            validityStart.Add("year", 2016);
            validityStart.Add("month", 12);
            validityStart.Add("date", 1);
            validityStart.Add("hour", 0);
            validityStart.Add("minute", 0);
            validityStart.Add("second", 0);

            var validityEnd = new JObject(); // Les dates doivent etre coherentes avec la date de creation
            validityEnd.Add("year", 2017);
            validityEnd.Add("month", 10);
            validityEnd.Add("date", 1);
            validityEnd.Add("hour", 0);
            validityEnd.Add("minute", 0);
            validityEnd.Add("second", 0);

            var validity = new JObject(); // La validation n'est pas obligatoire
            validity.Add("start", validityStart);
            validity.Add("end", validityEnd);

            var scheduler = new JObject();
            scheduler.Add("type", "periodic");
            scheduler.Add("name", schedulerName);
            scheduler.Add("frequency", frequency);
            scheduler.Add("validity", validity);

            var country = new JArray();
            country.Add("FRA"); // Pour changer de pays de normalisation utiliser l'ISO ALPHA-3 Code (https://en.wikipedia.org/wiki/ISO_3166-1_alpha-3)

            var fields = new JObject();
            fields.Add(fieldsId.ToString(), country);

            var normalization = new JObject(); // Parametre de normalisation
            normalization.Add("type", "normalization");
            normalization.Add("fields", fields);

            var segmentation = new JObject(); // Ajout des cibles a un Segment
            segmentation.Add("type", "segmentation");
            segmentation.Add("segmentId", segmentId);
            segmentation.Add("emptyExisitingSegment", true);

            var destination = new JObject();
            destination.Add("sms", true);
            destination.Add("email", true);

            var redList = new JObject(); // Ajout des cibles a la liste rouge
            redList.Add("type", "redList");
            redList.Add("destination", destination);

            var rules = new JObject();
            rules.Add("ignore", true); //ou rules.Add("first", true); ou rules.Add("last", true);

            var duplicate = new JObject(); // Regles a appliquer en cas de doublon
            duplicate.Add("type", "duplicate");
            duplicate.Add("rules", rules);


            var contactGuids = new JArray();
            contactGuids.Add(contactsId);

            var groupIds = new JArray();
            groupIds.Add(groupsContactsId);

            var report = new JObject(); // Parametres du rapport
            report.Add("type", "report");
            report.Add("sendFinalReport", false);
            report.Add("sendErrorReport", true);
            // Si un des rapport doit être envoyer, il faut au moins un destinataire ('contactGuids'/'groupIds')
            report.Add("contactGuids", contactGuids); // Id des utilisateurs : Administration -> Utilisateurs -> Identifiant
            report.Add("groupIds", groupIds); // Id des groupes : Administration -> Groupes -> Identifiant

            var database = new JObject(); // Parametres pour mettre a jour une cible dans la base de donnees
            database.Add("type", "database");
            database.Add("updateExisting", true);
            database.Add("crushData", true);

            var features = new JArray(); // Si vous voulez enlever une option il suffit de mettre la ligne en commentaire
            //features.Add(normalization);
            features.Add(segmentation);
             //features.Add(redList);
            features.Add(duplicate);
            features.Add(report);
            features.Add(database);

            var createImportJson = new JObject();
            createImportJson.Add("name", importName);
            createImportJson.Add("binding", binding);
            createImportJson.Add("source", source);
            createImportJson.Add("scheduler", scheduler);
            createImportJson.Add("features", features);

            //On affiche le Json
            Console.Write("Json :\n" + createImportJson + "\n\n");

            return (createImportJson);
        }

        private static void Main(string[] args)
        {
            // Changez le Path pour correspondre à la destination de votre fichier de configuration
            var doc = new XmlDocument();
            doc.Load("config.xml");

            var xKey = doc.DocumentElement.SelectSingleNode("/config/xKey").InnerText;
            var baseUrl = doc.DocumentElement.SelectSingleNode("/config/url").InnerText;

            //Lancement de la connexion pour remplir la requete
            var url = baseUrl + "imports/";

            Console.Write("CREATE IMPORT\n");

            var result = Utils.allConnection(url, xKey, getCreateImportJson(), "POST");
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

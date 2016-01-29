using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace createAutoImports
{
    class Program
    {
        public static JObject getCreateImportJson()
        {
            // Remplissez les informations obligatoires
            string importName = "Nom de votre Import automatique"; // Nom de l'import
            string schedulerName = "Nom du scheduler"; // Nom du scheduler
            int binding = 1234; // Id du binding
            int segmentId = 1234; // Id du segment
            int fieldsId = 1234; // Id du field
            string[] contactsId = { "1234ABCD" }; // Id des utilisateurs : Administration -> Utilisateurs -> Identifiant
            int[] groupsContactsId = { 1234 }; // Id des groupes : Administration -> Groupes -> Identifiant

            //Creation du Json


            JObject source = new JObject();
            source.Add("type", "ftp");

            JObject occurs = new JObject();
            occurs.Add("type", "daily"); //Quotidient
            /* Autres possibilites :
            occurs.Add("type", "weekly"); // Hebdomadaire
            String days[] = {"Mon", "Tue", "Wed", "Thu", "Fri", "Sat"};
            occurs.Add("days", days);

            occurs.Add("type", "monthly"); // Mensuel
            int days[] = {20};
            occurs.Add("days", days);
            */

            JObject periodicityValue = new JObject();
            periodicityValue.Add("hour", 6);
            periodicityValue.Add("minute", 0);
            periodicityValue.Add("second", 0);

            JObject periodicity = new JObject(); // Heure de l'import
            periodicity.Add("type", "once");
            periodicity.Add("value", periodicityValue);

            JObject frequency = new JObject();
            frequency.Add("occurs", occurs);
            frequency.Add("periodicity", periodicity);

            JObject validityStart = new JObject(); // Les dates doivent etre coherentes avec la date de creation
            validityStart.Add("year", 2016);
            validityStart.Add("month", 12);
            validityStart.Add("date", 1);
            validityStart.Add("hour", 0);
            validityStart.Add("minute", 0);
            validityStart.Add("second", 0);

            JObject validityEnd = new JObject(); // Les dates doivent etre coherentes avec la date de creation
            validityEnd.Add("year", 2017);
            validityEnd.Add("month", 10);
            validityEnd.Add("date", 1);
            validityEnd.Add("hour", 0);
            validityEnd.Add("minute", 0);
            validityEnd.Add("second", 0);

            JObject validity = new JObject(); // La validation n'est pas obligatoire
            validity.Add("start", validityStart);
            validity.Add("end", validityEnd);

            JObject scheduler = new JObject();
            scheduler.Add("type", "periodic");
            scheduler.Add("name", schedulerName);
            scheduler.Add("frequency", frequency);
            scheduler.Add("validity", validity);

            JArray country = new JArray();
            country.Add("FRA"); // Pour changer de pays de normalisation utiliser l'ISO ALPHA-3 Code (https://en.wikipedia.org/wiki/ISO_3166-1_alpha-3)

            JObject fields = new JObject();
            fields.Add(fieldsId.ToString(), country);

            JObject normalization = new JObject(); // Parametre de normalisation
            normalization.Add("type", "normalization");
            normalization.Add("fields", fields);

            JObject segmentation = new JObject(); // Ajout des cibles a un Segment
            segmentation.Add("type", "segmentation");
            segmentation.Add("segmentId", segmentId);
            segmentation.Add("emptyExisitingSegment", true);

            JObject destination = new JObject();
            destination.Add("sms", true);
            destination.Add("email", true);

            JObject redList = new JObject(); // Ajout des cibles a la liste rouge
            redList.Add("type", "redList");
            redList.Add("destination", destination);

            JObject rules = new JObject();
            rules.Add("ignore", true); //ou rules.Add("first", true); ou rules.Add("last", true);

            JObject duplicate = new JObject(); // Regles a appliquer en cas de doublon
            duplicate.Add("type", "duplicate");
            duplicate.Add("rules", rules);


            JArray contactGuids = new JArray();
            contactGuids.Add(contactsId);

            JArray groupIds = new JArray();
            groupIds.Add(groupsContactsId);

            JObject report = new JObject(); // Parametres du rapport
            report.Add("type", "report");
            report.Add("sendFinalReport", false);
            report.Add("sendErrorReport", true);
            // Si un des rapport doit être envoyer, il faut au moins un destinataire ('contactGuids'/'groupIds')
            report.Add("contactGuids", contactGuids); // Id des utilisateurs : Administration -> Utilisateurs -> Identifiant
            report.Add("groupIds", groupIds); // Id des groupes : Administration -> Groupes -> Identifiant

            JObject database = new JObject(); // Parametres pour mettre a jour une cible dans la base de donnees
            database.Add("type", "database");
            database.Add("updateExisting", true);
            database.Add("crushData", true);

            JArray features = new JArray(); // Si vous voulez enlever une option il suffit de mettre la ligne en commentaire
            features.Add(normalization);
            features.Add(segmentation);
             //features.Add(redList);
            features.Add(duplicate);
            features.Add(report);
            features.Add(database);

            JObject createImportJson = new JObject();
            createImportJson.Add("name", importName);
            createImportJson.Add("binding", binding);
            createImportJson.Add("source", source);
            createImportJson.Add("scheduler", scheduler);
            createImportJson.Add("features", features);

            //On affiche le Json
            Console.Write("Json :\n" + createImportJson + "\n\n");

            return (createImportJson);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////FIN DE CREATION DES JSONS///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////DEBUT DU PROGRAMME//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        static void Main(string[] args)
        {
            //Ne pas remplir
            Object[] result = null;
            HttpWebRequest con = null;
            HttpWebResponse httpResponse = null;


            //Url de base
            String urlBase = "http://v8.mailperformance.com/";
            //X-Key
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            //Lancement de la connexion pour remplir la requete
            String url = urlBase + "imports/";

            Console.Write("CREATE IMPORT\n");

            result = allConnection(con, url, xKey, getCreateImportJson(), "POST");
            httpResponse = (HttpWebResponse)result[2];
            verification(result);

            Console.ReadLine();
            httpResponse.Close();
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////FIN DU PROGRAMME////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////DEBUT DES FONCTIONS/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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

            if (response >= 200 && response <= 299)
            {
                //On affiche la reponse
                Console.Write("OK : \n\n" + responseString + "\n\n");
                return (true);
            }
            Console.Write("Error : " + response + " " + responseString + "\n");
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
            con.Accept = "application/vnd.mperf.v8.import.v1+json";

            return (con);
        }

        //Fonction de connexion et envoie des informations
        static Object[] allConnection(HttpWebRequest con, String url, String xKey, JObject jsonMessage, String method)
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

            //Test de l'envoi
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

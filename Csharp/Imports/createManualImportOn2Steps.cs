using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace createManualImportOn2Steps
{
    class createManualImportOn2Steps
    {
        static JObject getCreateImportJson(int idBinding)
        {
            // Remplissez les informations obligatoires
            string importName = "Nom de votre Import Manuel"; //Nom de l'import
            int segmentId = 1234; // Id du segment
            String phoneFieldsId = "1234"; // Id du field
            String[] contactsId = { "1234ABCD" }; // Id des utilisateurs : Administration -> Utilisateurs -> Idenfitifant
            int[] groupsContactsId = { 1234 }; // Id des groupes : Administration -> Groupes -> Idenfitifant

            JArray phoneFieldsIdArray = new JArray(); // Pour changer de pays de normalisation utiliser l'ISO ALPHA-3 Code (https://en.wikipedia.org/wiki/ISO_3166-1_alpha-3)
            phoneFieldsIdArray.Add("FRA");

            JObject fields = new JObject();
            fields.Add(phoneFieldsId, phoneFieldsIdArray);

            JObject normalization = new JObject(); // Normalisation
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
            rules.Add("ignore", true); // ou rules.Add("first", true); ou rules.Add("last", true);

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
            report.Add("contactGuids", contactGuids); // Id des utilisateurs : Administration -> Utilisateurs -> Idenfitifant
            report.Add("groupIds", groupIds); // Id des groupes : Administration -> Groupes -> Idenfitifant

            JObject database = new JObject(); // Parametres pour mettre a jour une cible dans la base de donnees
            database.Add("type", "database");
            database.Add("updateExisting", true);
            database.Add("crushData", false);

            JArray features = new JArray();
            features.Add(normalization);
            features.Add(segmentation);
            //features.Add(redList);
            features.Add(duplicate);
            features.Add(report);
            features.Add(database);

            JObject createImportJson = new JObject();
            createImportJson.Add("name", importName);
            createImportJson.Add("features", features);
            createImportJson.Add("binding", idBinding);

            //On affiche le Json
            Console.Write("Json :\n" + createImportJson + "\n\n");

            return (createImportJson);
        }

        static string[] getFileSourceInfo()
        {
            // Remplissez les informations obligatoires
            string path = "C:\\votre\\chemin\\vers\\le\\fichier\\";
            string name = "nomDeVotreFichier.extension";
            string file = null;

            System.IO.StreamReader myFile =
                new System.IO.StreamReader(path + name);
            file = myFile.ReadToEnd();

            string[] result = { name, path, file };
            return (result);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////FIN DE CREATION DES JSONS////////////////////////////////////////////////////////////////////////////////////////////////////
        /////DEBUT DU PROGRAMME///////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static void Main(string[] args)
        {
            //Ne pas remplir
            Object[] result = null;
            HttpWebRequest con = null;
            HttpWebResponse httpResponse = null;

            //Url de base
            String urlBase = "https://backoffice.mailperformance.com/";
            //X-Key
            String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int idBinding = 1234; //Bindings cree par les APIs : importFormats.

            //Lancement de la connexion pour remplir la requete
            String url = urlBase + "imports/";
            Console.Write("CREATE IMPORT\n");
            result = allConnection(con, url, xKey, getCreateImportJson(idBinding), "POST");
            if (verification(result) == false)
                System.Environment.Exit(0);
            int importId = ((dynamic)(JsonConvert.DeserializeObject((String)result[3]))).id;

            //Nouvelle url
            url = urlBase + "imports/" + importId + "/executions";
            //On execute l'import
            Console.Write("EXECUTION IMPORT\n");
            result = sendFile(con, url, xKey, getFileSourceInfo(), "POST");
            if ((verification(result)) == false)
                System.Environment.Exit(0);

            Console.ReadLine();
            httpResponse = (HttpWebResponse)result[2];
            httpResponse.Close();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////FIN DU PROGRAMME/////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////DEBUT DES FONCTIONS//////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

        static Object[] sendFile(HttpWebRequest con, String url, String xKey, String[] fileInfo, String method)
        {
            string nameFile = fileInfo[0];
            string pathAndNameFile = fileInfo[1] + fileInfo[0];
            string file = fileInfo[2];

            Console.Write("\nnamefile = " + nameFile + "\npathAndNameFile = " + pathAndNameFile + "\nFILE = " + file + "\n");

            //Lancement de la connexion pour remplir la requete
            con = (HttpWebRequest)WebRequest.Create(url);
            con.Method = method;

            //Mise en place du xKey et des options
            con.Headers.Add("X-Key", xKey);
            con.Headers.Add("Content-Disposition", "form-data; filename=" + nameFile);
            con.Accept = "application/vnd.mperf.v8.import.v1+json";
            con.ContentType = "application/octet-stream";

            var streamWriter = new StreamWriter(con.GetRequestStream());
            streamWriter.Write(file);
            streamWriter.Flush();
            streamWriter.Close();

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

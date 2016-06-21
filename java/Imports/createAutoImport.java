import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class createAutoImport
{
    public static JSONObject getCreateImportJson() throws JSONException
    {
        // Remplissez les informations obligatoires
        String importName = "Nom de votre Import automatique"; // Nom de l'import
        String schedulerName = "Nom du scheduler"; // Nom du scheduler
        int binding = 1234; // Id du binding
        int segmentId = 1234; // Id du segment
        int fieldsId = 1234; // Id du field
        String contactsId[] = {"1234ABCD"}; // Id des utilisateurs : Administration -> Utilisateurs -> Identifiant
        int groupsContactsId[] = {1234}; // Id des groupes : Administration -> Groupes -> Identifiant

        //Creation du Json


        JSONObject source = new JSONObject();
        source.put("type", "ftp");

        JSONObject occurs = new JSONObject();
        occurs.put("type", "daily"); //Quotidient
        /* Autres possibilites :
        occurs.put("type", "weekly"); // Hebdomadaire
        String days[] = {"Mon", "Tue", "Wed", "Thu", "Fri", "Sat"};
        occurs.put("days", days);

        occurs.put("type", "monthly"); // Mensuel
        int days[] = {20};
        occurs.put("days", days);
        */

        JSONObject periodicityValue = new JSONObject();
        periodicityValue.put("hour", 6);
        periodicityValue.put("minute", 0);
        periodicityValue.put("second", 0);

        JSONObject periodicity = new JSONObject(); // Heure de l'import
        periodicity.put("type", "once");
        periodicity.put("value", periodicityValue);

        JSONObject frequency = new JSONObject();
        frequency.put("occurs", occurs);
        frequency.put("periodicity", periodicity);

        JSONObject validityStart = new JSONObject(); // Les dates doivent etre coherentes avec la date de creation
        validityStart.put("year", 2016);
        validityStart.put("month", 12);
        validityStart.put("date", 1);
        validityStart.put("hour", 0);
        validityStart.put("minute", 0);
        validityStart.put("second", 0);

        JSONObject validityEnd = new JSONObject(); // Les dates doivent etre coherentes avec la date de creation
        validityEnd.put("year", 2017);
        validityEnd.put("month", 10);
        validityEnd.put("date", 1);
        validityEnd.put("hour", 0);
        validityEnd.put("minute", 0);
        validityEnd.put("second", 0);

        JSONObject validity = new JSONObject(); // La validation n'est pas obligatoire
        validity.put("start", validityStart);
        validity.put("end", validityEnd);

        JSONObject scheduler = new JSONObject();
        scheduler.put("type", "periodic");
        scheduler.put("name", schedulerName);
        scheduler.put("frequency", frequency);
        scheduler.put("validity", validity);

        JSONObject fields = new JSONObject();
        String country[] = {"FRA"}; // Pour changer de pays de normalisation utiliser l'ISO ALPHA-3 Code (https://en.wikipedia.org/wiki/ISO_3166-1_alpha-3)
        fields.put(Integer.toString(fieldsId), country);

        JSONObject normalization = new JSONObject(); // Parametre de normalisation
        normalization.put("type", "normalization");
        normalization.put("fields", fields);

        JSONObject segmentation = new JSONObject(); // Ajout des cibles a un Segment
        segmentation.put("type", "segmentation");
        segmentation.put("segmentId", segmentId);
        segmentation.put("emptyExisitingSegment", true);

        JSONObject destination = new JSONObject();
        destination.put("sms", true);
        destination.put("email", true);

        JSONObject redList = new JSONObject(); // Ajout des cibles a la liste rouge
        redList.put("type", "redList");
        redList.put("destination", destination);

        JSONObject rules = new JSONObject();
        rules.put("ignore", true); //ou rules.put("first", true); ou rules.put("last", true);

        JSONObject duplicate = new JSONObject(); // Regles a appliquer en cas de doublon
        duplicate.put("type", "duplicate");
        duplicate.put("rules", rules);

        JSONObject report = new JSONObject(); // Parametres du rapport
        report.put("type", "report");
        report.put("sendFinalReport", false);
        report.put("sendErrorReport", true);
        // Si un des rapport doit être envoyer, il faut au moins un destinataire ('contactGuids'/'groupIds')
        report.put("contactGuids", contactsId); // Id des utilisateurs : Administration -> Utilisateurs -> Identifiant
        report.put("groupIds", groupsContactsId); // Id des groupes : Administration -> Groupes -> Identifiant

        JSONObject database = new JSONObject(); // Parametres pour mettre a jour une cible dans la base de donnees
        database.put("type", "database");
        database.put("updateExisting", true);
        database.put("crushData", true);

        JSONArray features = new JSONArray(); // Si vous voulez enlever une option il suffit de mettre la ligne en commentaire
        features.put(normalization);
        features.put(segmentation);
        //features.put(redList);
        features.put(duplicate);
        features.put(report);
        features.put(database);

        JSONObject createImportJson = new JSONObject();
        createImportJson.put("name", importName);
        createImportJson.put("binding", binding);
        createImportJson.put("source", source);
        createImportJson.put("scheduler", scheduler);
        createImportJson.put("features", features);

        //On affiche le Json
        System.out.print("Json :\n" + createImportJson + "\n\n");

        return (createImportJson);
    }


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DE CREATION DES JSONS////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DU PROGRAMME///////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static void main(String[] args) throws Exception
    {
        //Url de base
        String urlBase = "https://backoffice.mailperformance.com/";
        //X-Key
        String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        //Lancement de la connexion pour remplir la requete
        String url = urlBase + "imports/";
        HttpURLConnection con = null;

        System.out.print("CREATE IMPORT\n");

        con = urlPostOrPut(con, "POST", getCreateImportJson(), xKey, url);

        //Lecture des donnees
        BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
        String reply = buffRead.readLine();
        buffRead.close();
        System.out.print(reply);

        con.disconnect();
        System.exit(1);
    }


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DU PROGRAMME////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DES FONCTIONS/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //Fonctions de connexion
    public static HttpURLConnection startCon(String url, String xKey) throws MalformedURLException, IOException
    {
        //Lancement de la connexion
        HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();

        //Mise en place du xKey et des options
        con.setRequestProperty("X-Key", xKey);
        con.setRequestProperty("Content-Type", "application/json");
        con.setRequestProperty("Accept", "application/vnd.mperf.v8.import.v1+json");
        return (con);
    }

    public static HttpURLConnection urlPostOrPut(HttpURLConnection con, String request, JSONObject dataJson, String xKey, String url) throws MalformedURLException, IOException
    {
        //Lancement de la connexion pour remplir la requete
        con = startCon(url, xKey);
        con.setRequestProperty("Content-Length", Integer.toString(dataJson.toString().length()));
        con.setRequestMethod(request);
        con.setDoOutput(true);

        //Envoie des informations dans la connexion
        DataOutputStream streamCon = new DataOutputStream(con.getOutputStream());
        streamCon.write(dataJson.toString().getBytes());
        streamCon.flush();
        streamCon.close();

        //Verification des reponses
        int responseCode = con.getResponseCode();
        if (responseCode != 200 && responseCode != 204)
        {
            //Affichage de l'erreur
            System.out.print("Error : " + responseCode + ' ' + con.getResponseMessage() + ".\nEnd urlPostOrPut");
            System.exit(1);
        }
        return (con);
    }
}

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class createManualImports
{
    public static JSONObject getCreateImportJson() throws JSONException
    {
        // Remplissez les informations obligatoires
        String importName = "Nom de votre Import Manuel"; //Nom de l'import
        int binding = 1234; //Id du binding

        JSONObject createImportJson = new JSONObject();
        createImportJson.put("name", importName);
        createImportJson.put("binding", binding);

        //On affiche le Json
        System.out.print("Json :\n" + createImportJson + "\n");

        return (createImportJson);
    }

    public static String[] getFileSourceInfo() throws JSONException, IOException
    {
        // Remplissez les informations obligatoires
        String path = "C:\\votre\\chemin\\vers\\le\\fichier\\";
        String name = "nomDeVotreFichier.extension";
        String file = null;

        //On lit l'interieur du fichier
        BufferedReader br = new BufferedReader(new FileReader(path + name));
        try {
            StringBuilder sb = new StringBuilder();
            String line = br.readLine();

            while (line != null) {
                sb.append(line);
                sb.append(System.lineSeparator());
                line = br.readLine();
            }
            file = sb.toString();
        } finally {
            br.close();
        }

        String result[] = {name, path, file};

        return (result);
    }

    public static JSONObject getBindingsJson() throws JSONException
    {
        // Remplissez les informations obligatoires
        String nameBindings = "Nom du Binding"; // Nom du binding ('null' pour rien)


        JSONObject firstColumn = new JSONObject();
        firstColumn.put("columnIndex", 0); //Numeros de la colonne
        firstColumn.put("fieldId", 1234); //Id du field (-10 = On ne prend pas en compte cette colonne)

        JSONObject secondColumn = new JSONObject();
        secondColumn.put("columnIndex", 1); //Numeros de la colonne
        secondColumn.put("fieldId", 1234); //Id du field (-10 = On ne prend pas en compte cette colonne)

        JSONObject thirdColumn = new JSONObject();
        thirdColumn.put("columnIndex", 2); //Numeros de la colonne
        thirdColumn.put("fieldId", -10); //Id du field (-10 = On ne prend pas en compte cette colonne)

        JSONArray binds = new JSONArray(); //On indique manuellement les colonnes avec les idFields
        binds.put(firstColumn);
        binds.put(secondColumn);
        binds.put(thirdColumn);

        JSONObject bindingJson = new JSONObject();
        bindingJson.put("name", nameBindings); //Nom de ce bindings
        bindingJson.put("separator", 59); //Code ascii de votre separateur (59 = ;)
        bindingJson.put("startAt", 2); //Commence a la ligne indique
        bindingJson.put("binds", binds);
        bindingJson.put("savedImportFormat", true); //On sauvegarde le format de l'import

        //On affiche le Json
        System.out.print("Json :\n" + bindingJson + "\n");

        return (bindingJson);
    }

    public static JSONObject getExecutionImportJson(int idBindings) throws JSONException
    {
        // Remplissez les informations obligatoires
        int segmentId = 1234; // Id du segment
        String phoneFieldsId = "1234"; // Id du field
        String contactsId[] = {"1234ABCD"}; // Id des utilisateurs : Administration -> Utilisateurs -> Identifiant
        int groupsContactsId[] = {1234}; // Id des groupes : Administration -> Groupes -> Identifiant


        String phoneFieldsIdArray[] = {"FRA"}; // Pour changer de pays de normalisation utiliser l'ISO ALPHA-3 Code (https://en.wikipedia.org/wiki/ISO_3166-1_alpha-3)

        JSONObject fields = new JSONObject();
        fields.put(phoneFieldsId, phoneFieldsIdArray);

        JSONObject normalization = new JSONObject(); // Normalisation
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
        rules.put("ignore", true); // ou rules.put("first", true); ou rules.put("last", true);

        JSONObject duplicate = new JSONObject(); // Regles a appliquer en cas de doublon
        duplicate.put("type", "duplicate");
        duplicate.put("rules", rules);

        JSONObject report = new JSONObject(); // Parametres du rapport
        report.put("type", "report");
        report.put("sendFinalReport", false);
        report.put("sendErrorReport", true);
        report.put("contactGuids", contactsId); // Id des utilisateurs : Administration -> Utilisateurs -> Identifiant
        report.put("groupIds", groupsContactsId); // Id des groupes : Administration -> Groupes -> Identifiant

        JSONObject database = new JSONObject(); // Parametres pour mettre a jour une cible dans la base de donnees
        database.put("type", "database");
        database.put("updateExisting", true);
        database.put("crushData", false);


        JSONArray features = new JSONArray();
        features.put(normalization);
        features.put(segmentation);
        features.put(redList);
        features.put(duplicate);
        features.put(report);
        features.put(database);

        JSONObject executionImportData = new JSONObject();
        executionImportData.put("binding", idBindings);
        executionImportData.put("features", features);

        //On affiche le Json
        System.out.print("Json :\n" + executionImportData + "\n");

        return (executionImportData);
    }


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DE CREATION DES JSONS////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DU PROGRAMME///////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static void main(String[] args) throws Exception
    {
        HttpURLConnection con = null;
        String verif = null;


        //Url de base
        String urlBase = "https://backoffice.mailperformance.com/";
        //X-Key
        String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        //Nouvelle url
        String url = urlBase + "imports/";
        //On cree l'import de base
        System.out.print("CREATE IMPORT\n");
        con = urlPostOrPut(con, "POST", getCreateImportJson(), xKey, url);
        if ((verif = verification(con)) == null)
            System.exit(0);
        System.out.printf("OK : " + verif + "\n\n");
        int importId = (new JSONObject(verif)).getInt("id");

        //Nouvelle url
        url = urlBase + "imports/" + importId + "/source";
        //On ajoute le fichier source pour l'import
        System.out.print("SOURCE IMPORT\n" + url + "\n");
        con = sendFile(con, "PUT", getFileSourceInfo(), xKey, url);
        if ((verif = verification(con)) == null)
            System.exit(0);

        //Nouvelle url
        url = urlBase + "importFormats/";
        //On cree notre binding (parser du fichier source)
        System.out.print("CREATION BINDINGS\n");
        con = urlPostOrPut(con, "POST", getBindingsJson(), xKey, url);
        if ((verif = verification(con)) == null)
            System.exit(0);
        System.out.printf("OK : " + verif + "\n\n");
        int idBindings = (new JSONObject(verif)).getJSONObject("identifier").getInt("guid");

        //Nouvelle url
        url = urlBase + "imports/" + importId + "/executions";
        //On execute l'import
        System.out.print("EXECUTION IMPORT\n");
        con = urlPostOrPut(con, "POST", getExecutionImportJson(idBindings), xKey, url);
        if ((verif = verification(con)) == null)
            System.exit(0);
        System.out.printf("OK : " + verif + "\n\n");


        con.disconnect();
    }


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DU PROGRAMME////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DES FONCTIONS/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static String verification(HttpURLConnection con) throws IOException, JSONException
    {
        //Verification des reponses
        int responseCode = con.getResponseCode();
        if (responseCode != 200 && responseCode != 204)
        {
            //Affichage de l'erreur
            System.out.print("Error : " + responseCode + ' ' + con.getResponseMessage() + ".");
            return (null);
        }
        return (readConnection(con));
    }

    public static String readConnection(HttpURLConnection con) throws IOException, JSONException
    {
        //Lecture des donnees
        BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
        String reply = buffRead.readLine();
        buffRead.close();

        return (reply);
    }


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

    public static HttpURLConnection urlPostOrPut(HttpURLConnection con, String request, JSONObject dataJson, String xKey, String url) throws MalformedURLException, IOException, JSONException
    {
        //Lancement de la connexion pour remplir la requete
        con = startCon(url, xKey);
        con.setRequestProperty("Content-Length", Integer.toString(dataJson.toString().length()));
        con.setRequestMethod(request);
        con.setDoOutput(true);

        // Envoie des informations dans la connexion
        DataOutputStream streamCon = new DataOutputStream(con.getOutputStream());
        streamCon.write(dataJson.toString().getBytes());
        streamCon.flush();
        streamCon.close();

        return (con);
    }

    public static HttpURLConnection sendFile(HttpURLConnection con, String request, String fileInfo[], String xKey, String url) throws MalformedURLException, IOException
    {
        String nameFile = fileInfo[0];
        String pathAndNameFile = fileInfo[1] + fileInfo[0];
        String file = fileInfo[2];

        System.out.printf("\nnamefile = " + nameFile + "\npathAndNameFile = " + pathAndNameFile + "\nFILE = " + file + "\n");

        //Information du header :
        con = (HttpURLConnection)new URL(url).openConnection();
        con.setDoOutput(true);
        con.setRequestMethod(request);
        con.setRequestProperty("X-Key", xKey);
        con.setRequestProperty("Accept", "application/vnd.mperf.v8.import.v1+json");
        con.setRequestProperty("Content-Type", "application/octet-stream");
        con.setRequestProperty("Content-Disposition", "form-data; filename=" + nameFile);

        // Envoie des informations dans la connexion
        DataOutputStream streamCon = new DataOutputStream(con.getOutputStream());
        streamCon.write(fileInfo[2].getBytes());
        streamCon.flush();
        streamCon.close();

        return (con);
    }
}

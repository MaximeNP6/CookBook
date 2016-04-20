package createActions;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;

import java.net.HttpURLConnection;
import java.util.Map;
import java.util.Properties;

import Utils.Request;
import org.json.JSONException;
import org.json.JSONObject;

public class createMailCampaignActionWithExcludedSegments
{
    public static void main(String[] args) throws IOException, JSONException, InterruptedException
    {
        Object NULL = null;
        Properties properties = new Properties();
        try {
            properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
            System.out.print(e.getMessage());
        }
        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");

        String type = "mailCampaign";	//Code pour envoyer une campagne de mail
        String name = "MailCampaignFromApi (java)";	//Nom de l'action
        String description = "MailCampaignFromApi (java)";	//Description de l'action

        Integer informationFolder = null;	//Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
        Integer informationCategory = null;	//Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

        String contentHeadersFromPrefix = "prefix";	//Adresse expeditice
        String contentHeadersFromLabel = "label";	//Libelle expediteur
        String contentHeadersReply = "address@reply.com"; //Adresse de reponse

        String contentSubject = "Subject of the message";	//Objet du mail
        String contentHTML = "Html message";	//Message HTML
        String contentText = "Text message";	//Message texte

        int idTestSegment[] = {14091};	//Id du segment de test pour la validation
        int idSelectSegment[] = {14105};	//Ids des segments selectionnes
        int idExcludedSegment[] = {14180};	//Ids des segments exclus



        //On trouve l'adresse pour la requete
        String url = baseUrl + "actions";

        //Creation du Json du message
        JSONObject informations = new JSONObject();
        informations.put("folder", informationFolder);
        informations.put("category", informationCategory);

        JSONObject contentHeadersFrom = new JSONObject();
        contentHeadersFrom.put("prefix", contentHeadersFromPrefix);
        contentHeadersFrom.put("label", contentHeadersFromLabel);

        JSONObject contentHeaders = new JSONObject();
        contentHeaders.put("from", contentHeadersFrom);
        contentHeaders.put("reply", contentHeadersReply);

        JSONObject segments = new JSONObject();
        segments.put("selected", idSelectSegment);
        segments.put("excluded", idExcludedSegment);

        JSONObject scheduler = new JSONObject();
        scheduler.put("type", "asap");	//Envoie : immediat = 'asap' / Date = 'scheduled'
        //scheduler.put("startDate", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever
        scheduler.put("segments", segments);

        JSONObject content = new JSONObject();
        content.put("headers", contentHeaders);
        content.put("subject", contentSubject);
        content.put("html", contentHTML);
        content.put("text", contentText);

        JSONObject jsonData = new JSONObject();
        jsonData.put("type", type);
        jsonData.put("name", name);
        jsonData.put("description", description);
        jsonData.put("informations", informations);
        jsonData.put("scheduler", scheduler);
        jsonData.put("content", content);

        //On affiche le message
        System.out.print(jsonData + "\n");


        //Lancement de la connexion
        Map<String, String> resp = Request.connection(url, xKey, jsonData, "POST");

        if (resp.get("mess").equals("OK")) {
            //Lecture des donnees
            System.out.print("Action : " + name + " created.\n\n");
        } else {
            System.out.print("Error " + resp.get("code") + ": " + resp.get("mess") + "\n");
            return;
        }
        //On recupere l'id de l'action
        JSONObject jObject = new JSONObject(resp.get("reply"));
        String idAction = jObject.getString("id");

        //On valide l'action
        url = baseUrl + "actions/" + idAction + "/validation";


        JSONObject jsonTest = new JSONObject();
        jsonTest.put("fortest", true);	//Phase de test
        jsonTest.put("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
        jsonTest.put("testSegments", idTestSegment);	//Les Ids des differents segments de tests
        jsonTest.put("mediaForTest", NULL);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
        jsonTest.put("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
        jsonTest.put("comments", NULL);	//Commentaire ('NULL' pour aucuns commentaires)

        //On affiche le Json
        System.out.print(jsonTest + "\n");

        //Lancement de la connexion
        resp = Request.connection(url, xKey, jsonTest, "POST");

        //Verification des reponses
        int actionState = Request.waitForState(idAction, xKey, baseUrl);

        if (!resp.get("code").equals("204") || actionState != 38) {
            //Affichage de l'erreur
            if (actionState == 20)
                System.out.print("Error : the test has failed.");
            else if (actionState == 10)
                System.out.print("Error : check the campaign in the Backoffice.");
            else
                System.out.print("Error " + resp.get("code") + ": " + resp.get("mess") + "\n");
            return;
        }
        //La phase de test a reussi
        System.out.print("The action " + name + " has been tested.\n\n");

        //Creation du Json du message pour la validation
        JSONObject jsonValid = new JSONObject();
        jsonValid.put("fortest", false);    //Phase de validation
        jsonValid.put("campaignAnalyser", false);    //Campaign Analyzer : 'true' = oui / 'false' = non
        jsonValid.put("testSegments", NULL);    //Les Ids des differents segments de tests
        jsonValid.put("mediaForTest", NULL);    //Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
        jsonValid.put("textandHtml", false);    //Envoyer la version texte et la version html : 'true' = oui / 'false' = non
        jsonValid.put("comments", NULL);    //Commentaire ('NULL' pour aucuns commentaires)

        //On affiche le Json
        System.out.print(jsonValid + "\n");


        //Lancement de la connexion
        resp = Request.connection(url, xKey, jsonValid, "POST");

        //Verification des reponses
        if (resp.get("code").equals("204")) {
            //Lecture des donnees
            System.out.print("Action : " + name + " validated.\n\n");
        } else {
            System.out.print("Error " + resp.get("code") + ": " + resp.get("mess") + "\n");
        }
    }

}

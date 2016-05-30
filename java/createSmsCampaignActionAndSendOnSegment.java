import Utils.Request;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.FileInputStream;
import java.io.IOException;
import java.util.Map;
import java.util.Properties;

import static org.json.JSONObject.NULL;

public class createSmsCampaignActionAndSendOnSegment
{
    public static void main(String[] args) throws IOException, JSONException, InterruptedException {
        Properties properties = new Properties();
        try {
            properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
            System.out.print(e.getMessage());
        }
        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");

        int[] idTestSegment = {1234};	//Id du segment de test
        int[] idSelectSegment = {1234};	//Id du segment de test

        String[] countries = {"FRA"};

        String type = "smsCampaign";	//Code pour envoyer une campagne de SMS
        String name = "SMSCampaignFromApi (java)";	//Nom de l'action
        String description = "SMSCampaignFromApi (java)";	//Description de l'action

        Integer informationFolder = null;	//Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
        Integer informationCategory = null;	//Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

        String textContent = "Text message";	//Message texte

        //On trouve l'adresse pour la requete
        String url = baseUrl + "actions";

        //Creation du Json du message
        JSONObject informations = new JSONObject();
        informations.put("folder", informationFolder);
        informations.put("category", informationCategory);

        JSONObject content = new JSONObject();
        content.put("textContent", textContent);

        JSONObject segments = new JSONObject();
        segments.put("selected", idSelectSegment);

        JSONObject scheduler = new JSONObject();
        scheduler.put("type", "asap");	//Envoie : immediat = 'asap' / Date = 'scheduled'
        scheduler.put("segments", segments);
        //scheduler.put("startDate", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever

        JSONObject settings = new JSONObject();
        settings.put("countries", countries);

        JSONObject jsonMessage = new JSONObject();
        jsonMessage.put("type", type);
        jsonMessage.put("name", name);
        jsonMessage.put("description", description);
        jsonMessage.put("informations", informations);
        jsonMessage.put("content", content);
        jsonMessage.put("scheduler", scheduler);
        jsonMessage.put("settings", settings);

        //On affiche le message
        System.out.print(jsonMessage + "\n");

        Map<String, String> resp = Request.connection(url, xKey, jsonMessage, "POST");

        if (resp.get("mess").equals("OK")) {
            //Lecture des donnees
            System.out.print("Action : " + name + " created.\n\n"
                    + "Don\'t forget to normalize your phone numbers with your country.\n");
        } else {
            System.out.print("Error " + resp.get("code") + ": " + resp.get("mess") + " while trying to create " +
                    "SMSCampaign\n");
            return;
        }

        JSONObject data = new JSONObject(resp.get("reply"));
        String idNewAction = data.getString("id");

        //On teste l'action

        //Nouvelle url
        url = baseUrl + "actions/" + idNewAction + "/validation";

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

        int actionState = Request.waitForState(idNewAction, xKey, baseUrl);

        if (actionState != 38) {
            //Affichage de l'erreur
            System.out.print("Error : " + resp.get("code") + " " + resp.get("mess") + " while trying to test the " +
                    "SMSCampaign\n");
            return;
        }
        //La phase de test a reussi
        System.out.print("The action " + data.getString("name") + " has been tested.\n\n");

        //On valide l'action

        JSONObject jsonValid = jsonTest;

        //Creation du Json du message pour la validation
        jsonValid.put("fortest", false);	//Phase de test
        jsonValid.put("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
        jsonValid.put("testSegments", NULL);	//Les Ids des differents segments de tests
        jsonValid.put("mediaForTest", NULL);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
        jsonValid.put("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
        jsonValid.put("comments", NULL);	//Commentaire ('NULL' pour aucuns commentaires)

        //On affiche le Json
        System.out.print(jsonValid + "\n");


        //Lancement de la connexion
        resp = Request.connection(url, xKey, jsonValid, "POST");

        //Verification des reponses
        if (resp.get("code").equals("204")) {
            //La phase de validation a reussi
            System.out.print("The action " + data.getString("name") + " has been validated.\n\n");
        }
        else {
            //Affichage de l'erreur
            System.out.print("Error : " + resp.get("code") + " " + resp.get("mess") + " while trying to validate " +
                    "SMSCampagin\n");
        }
	}
}

package createActions;

import java.io.FileInputStream;
import java.io.IOException;

import java.util.Map;
import java.util.Properties;

import Utils.Request;
import org.json.JSONException;
import org.json.JSONObject;

public class createSmsCampaignAction
{
	public static void main(String[] args) throws IOException, JSONException
	{
        Properties properties = new Properties();
        try {
            properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
            System.out.print(e.getMessage());
        }
        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");

        String type = "smsCampaign";	//Code pour envoyer une campagne de SMS
        String name = "SMSCampaignFromApi (java)";	//Nom de l'action
        String description = "SMSCampaignFromApi (java)";	//Description de l'action

        Integer informationFolder = 0123;	//Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
        Integer informationCategory = 0123;	//Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

        String textContent = "Text message";	//Message texte

        //On trouve l'adresse pour la requete
        String url = baseUrl + "actions";

        //Creation du Json du message
        JSONObject informations = new JSONObject();
        informations.put("folder", informationFolder);
        informations.put("category", informationCategory);

        JSONObject content = new JSONObject();
        content.put("textContent", textContent);

        JSONObject scheduler = new JSONObject();
        scheduler.put("type", "scheduled");	//Envoie : immediat = 'asap' / Date = 'scheduled'
        //scheduler.put("startDate", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever

        JSONObject jsonMessage = new JSONObject();
        jsonMessage.put("type", type);
        jsonMessage.put("name", name);
        jsonMessage.put("description", description);
        jsonMessage.put("informations", informations);
        jsonMessage.put("content", content);
        jsonMessage.put("scheduler", scheduler);

        //On affiche le message
        System.out.print(jsonMessage + "\n");

        //On affiche le message
        System.out.print(jsonMessage + "\n");

        Map<String, String> resp = Request.connection(url, xKey, jsonMessage, "POST");

        if (resp.get("mess").equals("OK")) {
            //Lecture des donnees
            System.out.print("Action : " + name + " created.\n\n"
                    + "Don\'t forget to normalize your phone numbers with your country.");
        } else {
            System.out.print("Error " + resp.get("code") + ": " + resp.get("mess") + "\n");
        }
	}
}

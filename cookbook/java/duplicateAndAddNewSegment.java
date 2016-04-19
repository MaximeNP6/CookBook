import java.io.FileInputStream;
import java.io.IOException;

import java.util.ArrayList;
import java.util.Map;
import java.util.Properties;

import org.json.JSONException;
import org.json.JSONObject;

import Utils.Request;


public class duplicateAndAddNewSegment
{
    private static JSONObject getNewActionDuplicateJson(JSONObject actionDuplicateJson,
                                                        int[] segmentIds) throws JSONException
    {
        //Modification de la nouvelle action
        JSONObject segments = new JSONObject();
        segments.put("selected", segmentIds);

        JSONObject scheduler = new JSONObject();
        scheduler.put("type", "asap");	//Envoi : immediat = 'asap' / Date = 'scheduled'
        //scheduler.put("startDate", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever
        scheduler.put("segments", segments);

        actionDuplicateJson.put("scheduler", scheduler);

        return (actionDuplicateJson);
    }

    private static JSONObject getActionValidation(int[] segmentTestIds) throws JSONException
    {
        JSONObject actionValidationJson = new JSONObject();
        actionValidationJson.put("fortest", true);	//Phase de test
        actionValidationJson.put("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
        actionValidationJson.put("testSegments", segmentTestIds);	//Les Ids des differents segments de tests
        actionValidationJson.put("mediaForTest", JSONObject.NULL);	//Rediriger tous les tests vers une seule adresse ('JSONObject.NULL' pour aucune valeur)
        actionValidationJson.put("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
        actionValidationJson.put("comments", JSONObject.NULL);	//Commentaire ('JSONObject.NULL' pour aucuns commentaires)

        //On affiche le Json
        System.out.print(actionValidationJson + "\n");

        return (actionValidationJson);
    }

    public static ArrayList duplicateAction(String urlBase,
                                            String actionId, String xKey) throws IOException, JSONException
    {
        //Nouvelle url
        String url = urlBase + "actions/" + actionId + "/duplication";

        JSONObject data = new JSONObject();

        System.out.print("Duplicating the Action\n\n");

        //On duplique a l'identique
        Map<String, String> resp = Request.connection(url, xKey, data, "POST");

        data = new JSONObject(resp.get("reply"));

        if (resp.get("code").equals("200")) {
            System.out.print("The action duplicate \"" + data.getString("name") + "\" has been created :\n");

            System.out.print(resp.get("reply") + "\n\n");
        }
        else {
            // Affichage de l'erreur
            System.out.print("Error : " + resp.get("code") + " " + resp.get("mess"));
            return null;
        }

        //On recupere l'id de la nouvelle action
        String actionDuplicateId = data.getString("id");

        ArrayList result = new ArrayList();
        result.add(resp);
        result.add(actionDuplicateId);
        result.add(data);

        return (result);
    }

    public static int updateActionDuplicate(String urlBase, String actionDuplicateId,
                                            JSONObject actionDuplicateJson, int[] segmentIds,
                                            String xKey) throws IOException, JSONException
    {
        Map<String, String> resp;

        //Nouvelle url
        String url = urlBase + "actions/" + actionDuplicateId;

        System.out.print("Adding New Segment\n\n");

        //On duplique a l'identique
        resp = Request.connection(url, xKey, getNewActionDuplicateJson(actionDuplicateJson, segmentIds), "PUT");

        if (resp.get("code").equals("200")) {
            System.out.print("The duplicated action \"" + new JSONObject(resp.get("reply")).getString("name") +
                    "\" is update:\n");
            System.out.print(resp.get("reply") + "\n\n");
        }
        else {
            // Affichage de l'erreur
            System.out.print("Error : " + resp.get("code") + " " + resp.get("mess") + "\n");
            return -1;
        }

        return 0;
    }

    public static int testActionDuplicate(String urlBase, String actionDuplicateId,
                                          String xKey, int[] segmentTestIds) throws InterruptedException, IOException, JSONException
    {
        //Nouvelle url
        String url = urlBase + "actions/" + actionDuplicateId + "/validation";

        System.out.print("Testing the duplicated Action\n\n");

        //Lancement de la connexion
        Request.connection(url, xKey, getActionValidation(segmentTestIds), "POST");

        int actionState = Request.waitForState(actionDuplicateId, xKey, urlBase);

        if (actionState != 38) {
            //Affichage de l'erreur
            if (actionState == 20)
                System.out.print("Error : the test failed.");
            else if (actionState == 10)
                System.out.print("Error : check the campaign in the Backoffice.");
            return -1;
        }
        else {
            //La phase de test a reussi
            System.out.print("The action has been tested.\n\n");
        }
        return 0;
    }

    public static void main(String[] args) throws Exception
    {
        Properties properties   = new Properties();

        try {
          properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
          System.out.print(e.getMessage());
        }

        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");

        int[] segmentIds = {01234};	//Id du segment a modifier
        int[] segmentTestIds = {01234};	//Id du segment de test a modifier
        String actionId = "000ABC";	//Id de l'action a dupliquer

        ArrayList result;


        if ((result = duplicateAction(baseUrl, actionId, xKey)) == null)
            return;
        String actionDuplicateId = (String) result.get(1);
        JSONObject actionDuplicateJson = (JSONObject) result.get(2);

        if (updateActionDuplicate(baseUrl, actionDuplicateId, actionDuplicateJson, segmentIds, xKey) == -1)
            return;

        if (testActionDuplicate(baseUrl, actionDuplicateId, xKey, segmentTestIds) == -1)
            return;

        return;
    }
}

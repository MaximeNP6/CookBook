import java.io.FileInputStream;
import java.io.IOException;

import java.util.Map;
import java.util.Properties;

import Utils.Request;

import org.json.JSONException;
import org.json.JSONObject;

public class duplicateAndValidate
{
    public static void main(String[] args) throws IOException, JSONException, InterruptedException
    {
        Object NULL = null;
        Properties properties   = new Properties();

        try {
            properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
            System.out.print(e.getMessage());
        }

        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");

        String idAction = "000ABC";	//Id de l'action a dupliquer
        int[] idTestSegment = {01234};	//Id du segment de test

        JSONObject data = new JSONObject();

        //Nouvelle url
        String url = baseUrl + "actions/" + idAction + "/duplication";

        //On duplique a l'identique
        Map<String, String> resp = Request.connection(url, xKey, data, "POST");

        //Verification des reponses
        if (resp.get("code").equals("200")) {
            // La categorie a bien ete créée
            System.out.print("The Action has been duplicated.\n\n");
        }
        else {
            // Affichage de l'erreur
            System.out.print("Error : " + resp.get("code") + " " + resp.get("mess"));
            return;
        }
        data = new JSONObject(resp.get("reply"));
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

        if (!resp.get("code").equals("204") || actionState != 38) {
            //Affichage de l'erreur
            if (actionState == 20)
                System.out.print("Error : the test failed.");
            else if (actionState == 10)
                System.out.print("Error : check the campaign in the Backoffice.");
            else
                System.out.print("Error : " + resp.get("code") + " " + resp.get("mess"));
        }
        else {
            //La phase de test a reussi
            System.out.print("The action " + data.getString("name") + " is tested.\n\n");

            //On valide l'action

            //Creation du Json du message pour la validation
            JSONObject jsonValid = new JSONObject();
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
                System.out.print("The action " + data.getString("name") + " is validated.\n\n");
            }
            else {
                //Affichage de l'erreur
                System.out.print("Error : " + resp.get("code") + " " + resp.get("mess"));
            }
        }
    }
}

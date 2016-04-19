import java.io.FileInputStream;
import java.io.IOException;

import java.util.Map;
import java.util.Properties;

import org.json.JSONException;
import org.json.JSONObject;

import Utils.Request;

public class getTargetAndSendMessage
{
    public static void main(String[] args) throws IOException, JSONException
    {
        /*
         * Renseignez ci-dessous les paramètres que vous souhaitez
         * @var uniciy		=> E-mail de la cible
         * @var idMessage	=> ID du message que vous souhaitez envoyer
        */
        String unicity = "test@test.com";
        String idMessage = "000ABC";


        Properties properties = new Properties();
        try {
            properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
            System.out.print(e.getMessage());
        }
        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");

        String url = baseUrl + "targets?unicity=" + unicity;

        Map<String, String> resp;
        resp = Request.connection(url, xKey, null, "GET");

        // Vérification de la réponse
        if (!resp.get("code").equals("200")) {
             // Affichage de l'erreur
            System.out.print("Error : " + resp.get("code") + " " + resp.get("mess"));
            return;
        }
        //On récupère l'id de la cible
        JSONObject jObject  = new JSONObject(resp.get("reply"));
        String idTarget = jObject.getString("id");

        //Nouvelle url en fonction de l'id du message et de la cible
        url = baseUrl + "actions/" + idMessage + "/targets/" + idTarget;

        resp = Request.connection(url, xKey, new JSONObject(), "POST");

        // Vérification de la réponse
        if (resp.get("code").equals("204")) {
            // Affichage des données
            System.out.print("Message sent to " + unicity);
        }
        else {
            // Affichage de l'erreur
            System.out.print("Error : " + resp.get("code") + " " + resp.get("mess"));
            return;
        }
    }
}

import java.io.*;

import java.net.HttpURLConnection;

import java.util.Map;
import java.util.Properties;

import Utils.Request;

import org.json.JSONException;
import org.json.JSONObject;

public class sendHTML
{
	public static void main(String[] args) throws IOException, JSONException
	{
		//Ici, renseignez l'email de la cible, la xKey et l'id du message
		String unicity = "test@test.com";
		String idMessage = "000ABC";

		String htmlMessage = "html message for java";	//Si vous ne voulez pas de cette option, effacer la ligne de creation du Json
		String textMessage = "text message for java";	//Si vous ne voulez pas de cette option, effacer la ligne de creation du Json
		String subject = "subject for java";
		String mailFrom = "mail@address.com";
		String replyTo = "mail@return.com";

		Properties properties   = new Properties();

		try {
			properties.load(new FileInputStream("config.properties"));
		} catch (IOException e) {
			System.out.print(e.getMessage());
		}

		String xKey = properties.getProperty("xKey");
		String baseUrl = properties.getProperty("url");
		String url = baseUrl + "targets?unicity=" + unicity;

		//Lancement de la connexion pour remplir la requete
		Map<String, String> resp = Request.connection(url, xKey, null, "GET");

		//Verification des reponses
		if (resp.get("code").equals("200")) {
			//On recupere l'id de la cible
			JSONObject jObject  = new JSONObject(resp.get("reply"));
			String idTarget = jObject.getString("id");

			//On affiche la cible
			System.out.print(resp.get("reply") + "\n");

			//Nouvelle url en fonction de l'id du message et de la cible
			url = baseUrl + "actions/" + idMessage + "/targets/" + idTarget;

			//Creation du message en Json
			JSONObject content = new JSONObject();
			content.put("html", htmlMessage);	//Si vous ne voulez pas de l'option : htmlMessage, effacer cette ligne
			content.put("text", textMessage);	//Si vous ne voulez pas de l'option : textMessage, effacer cette ligne

			JSONObject header = new JSONObject();
			header.put("subject", subject);
			header.put("mailFrom", mailFrom);
			header.put("replyTo", replyTo);

			JSONObject jsonMessage = new JSONObject();
			jsonMessage.put("content", content);
			jsonMessage.put("header", header);

			//Lancement de la connexion pour remplir la requete
			resp = Request.connection(url, xKey, jsonMessage, "POST");

			// Verification des reponses
			if (resp.get("code").equals("204")) {
				System.out.print("Message sent to " + unicity);
			}
			else {
				//Affichage de l'erreur
				System.out.print("Error : " + resp.get("code") + ' ' + resp.get("mess"));
			}
		}
		else {
			//Affichage de l'erreur
			System.out.print("Error : " + resp.get("code") + ' ' + resp.get("reply"));
		}
	}
}

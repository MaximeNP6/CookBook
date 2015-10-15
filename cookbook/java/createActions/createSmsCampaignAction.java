import java.io.IOException;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONException;
import org.json.JSONObject;

public class createSmsCampaignAction
{
	public static void main(String[] args) throws IOException, JSONException
	{
		//Ici, renseignez la xKey
		String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		
		String type = "smsCampaign";	//Code pour envoyer une campagne de SMS
		String name = "SMSCampaignFromApi (java)";	//Nom de l'action
		String description = "SMSCampaignFromApi (java)";	//Description de l'action

		int informationFolder = 0123;	//Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
		int informationCategory = 0123;	//Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

		String textContent = "Text message";	//Message texte
		
		//On trouve l'adresse pour la requete
		String url = "http://v8.mailperformance.com/actions";
		
		//Creation du Json du message
		JSONObject informations = new JSONObject();
		informations.put("folder", informationFolder);
		informations.put("category", informationCategory);
		
		JSONObject content = new JSONObject();
		content.put("textContent", textContent);
		
		JSONObject jsonMessage = new JSONObject();
		jsonMessage.put("type", type);
		jsonMessage.put("name", name);
		jsonMessage.put("description", description);
		jsonMessage.put("informations", informations);
		jsonMessage.put("content", content);
		
		//On affiche le message
		System.out.print(jsonMessage + "\n");
		
		//Lancement de la connexion pour remplir la requete
		HttpURLConnection con = openConn(url, xKey);
		con.setRequestProperty("Content-Length", Integer.toString(jsonMessage.length()));
		con.setRequestMethod("POST");
		con.setDoOutput(true);
		        
		// Envoi des informations dans la connection
		OutputStreamWriter sendMessage = new OutputStreamWriter(con.getOutputStream());
		sendMessage.write(jsonMessage.toString());
		sendMessage.flush();
		sendMessage.close();
		
		//Verification des reponses
		int responseCode = con.getResponseCode();
		if (responseCode != 200)
		{
			//Affichage de l'erreur
			System.out.print("Error : " + responseCode + " " + con.getResponseMessage());
		}
		else
		{
			System.out.print("Action : " + name + " created.\n\n"
					+ "Don\'t forget to normalize your phone numbers with your country.");
		}
		con.disconnect();
	}
	
	
	//Fonctions ----

	
	public static HttpURLConnection openConn(String url, String xKey) throws MalformedURLException, IOException
	{
		//Lancement de la connexion
		HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();
		
		//Mise en place du xKey et des options
		con.setRequestProperty("X-Key", xKey);
		con.setRequestProperty("Content-Type", "application/json");
		return (con);
	}
}

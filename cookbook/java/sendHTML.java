package sendHTML;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONException;
import org.json.JSONObject;

public class sendHTML
{
	public static void main(String[] args) throws IOException, JSONException
	{
		//Ici, renseignez l'email de la cible, la xKey, l'id du message et les parametres personnalises
		String unicity = "test@test.com";
		String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		String idMessage = "000ABC";

		String htmlMessage = "html message for java";	//Si vous ne voulez pas de cette option, effacer la ligne de creation du Json 
		String textMessage = "text message for java";	//Si vous ne voulez pas de cette option, effacer la ligne de creation du Json 
		String subject = "subject for java";
		String mailFrom = "mail@address.com";
		String replyTo = "mail@return.com";
		
		//Lancement de la connexion pour remplir la requete
		String url = "http://v8.mailperformance.com/targets?unicity=" + unicity;
		HttpURLConnection con = openConn(url, xKey);
		con.setRequestMethod("GET");
		
		//Verification des reponses
		int responseCode = con.getResponseCode();
		if (responseCode != 200)
		{
			//Affichage de l'erreur
			System.out.print("Error : " + responseCode + ' ' + con.getResponseMessage());
		}
		else
		{
			//Lecture des donnees ligne par lignes
			BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
			String reply = buffRead.readLine();
			con.disconnect();
			buffRead.close();
			
			//On recupere l'id de la cible
			JSONObject jObject  = new JSONObject(reply);
			String idTarget = jObject.getString("id");
			
			//On affiche la cible
			System.out.print(reply + "\n");
			
			//Nouvelle url en fonction de l'id du message et de la cible
			url = "http://v8.mailperformance.com/actions/" + idMessage + "/targets/" + idTarget;

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
			con = openConn(url, xKey);
			con.setRequestProperty("Content-Length", Integer.toString(jsonMessage.length()));
			con.setRequestMethod("POST");
			con.setDoOutput(true);
			        
			// Envoie des informations dans la connection
			OutputStreamWriter sendMessage = new OutputStreamWriter(con.getOutputStream());
			sendMessage.write(jsonMessage.toString());
			sendMessage.flush();
			sendMessage.close();
			
			//Verification des reponses
			responseCode = con.getResponseCode();
			if (responseCode != 204)
			{
				//Affichage de l'erreur
				System.out.print("Error : " + responseCode + ' ' + con.getResponseMessage());
			}
			else
			{
				System.out.print("Message sent to " + unicity);
			}
		}
		con.disconnect();
	}
	
	//Fonctions ----
	
	public static HttpURLConnection openConn(String url, String xKey) throws MalformedURLException, IOException
	{
		//Lancement de la connection
		HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();
		
		//Mise en place du xKey et des options
		con.setRequestProperty("X-Key", xKey);
		con.setRequestProperty("Content-Type", "application/json");
		return (con);
	}
}

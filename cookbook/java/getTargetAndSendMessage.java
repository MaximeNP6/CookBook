import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONException;
import org.json.JSONObject;

public class getTargetAndSendMessage
{
	public static HttpURLConnection openConn(String url) throws MalformedURLException, IOException
	{
		//Lancement de la connexion
		HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();
		
		//Mise en place du xKey et des options
		con.setRequestProperty("X-Key", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
		con.setRequestProperty("Content-Type", "application/json");
		return (con);
	}
	
	public static void main(String[] args) throws IOException, JSONException
	{
		//Ici, renseignez l'email et l'id du message
		String unicity = "test@test.com";
		String idMessage = "000ABC";
		
		//Lancement de la connexion pour remplir la requete
		String url = "http://v8.mailperformance.com/targets?unicity=" + unicity;
		HttpURLConnection con = openConn(url);
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
			
			//Nouvelle url en fonction de l'id du message et de la cible
			url = "http://v8.mailperformance.com/actions/" + idMessage + "/targets/" + idTarget;

			//Lancement de la connexion pour remplir la requete
			con = openConn(url);
			con.setRequestProperty("Content-Length", "");
			con.setRequestMethod("POST");
			con.setDoOutput(true);
			        
			// Envoi des informations dans la connexion
			con.getOutputStream();
			
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
}

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
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
		String url = baseUrl + "targets";

		//Lancement de la connexion pour remplir la requête
		HttpURLConnection con = Request.openConn(url + "?unicity=" + unicity, xKey);
		con.setRequestMethod("GET");
		
		//Vérification des réponses
		int responseCode = con.getResponseCode();
		if (responseCode != 200)
		{
			//Affichage de l'erreur
			System.out.print("Error : " + responseCode + ' ' + con.getResponseMessage());
		}
		else
		{
			//Lecture des données ligne par ligne
			BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
			String reply = buffRead.readLine();
			con.disconnect();
			buffRead.close();
			
			//On récupère l'id de la cible
			JSONObject jObject  = new JSONObject(reply);
			String idTarget = jObject.getString("id");
			
			//Nouvelle url en fonction de l'id du message et de la cible
			url = baseUrl + "actions/" + idMessage + "/targets/" + idTarget;

			//Lancement de la connexion pour remplir la requête
			con = Request.openConn(url, xKey);
			con.setRequestProperty("Content-Length", "");
			con.setRequestMethod("POST");
			con.setDoOutput(true);
			        
			// Envoi des informations dans la connexion
			con.getOutputStream();
			
			//Vérification des réponses
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

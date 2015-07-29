import java.io.IOException;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONException;
import org.json.JSONObject;

public class createCategory
{
	public static void main(String[] args) throws IOException, JSONException, InterruptedException
	{
		//Ici, renseignez la xKey et les parametres personnalises du message
		String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		String categoriesId = "0123";	//Id de la categorie a modifier ('null' si la categorie est a creer)
		
		String name = "Category java";	//Nom de la categorie
		String description = "Category (java)";	//Description de la categorie


		//On trouve l'addresse pour la requete
		String url = "http://v8.mailperformance.com/categories/";
		if (categoriesId != null)
		{
			url = "http://v8.mailperformance.com/categories/" + categoriesId;
		}
		
		//Creation du Json du message
		JSONObject jsonData = new JSONObject();
		jsonData.put("name", name);
		jsonData.put("description", description);
		
		//On affiche le message
		System.out.print(jsonData + "\n\n");
		
		//Lancement de la connexion
		HttpURLConnection con = null;
		if (categoriesId != null)
		{
			con = connection(url, xKey, jsonData, "PUT");
		}
		else
		{
			con = connection(url, xKey, jsonData, "POST");
		}
		
		//Verification des reponses
		int responseCode = con.getResponseCode();
		if (responseCode != 200)
		{
			//Affichage de l'erreur
			System.out.print("Error : " + responseCode + " " + con.getResponseMessage());
		}
		else
		{
			//La categorie a bien ete cree
			System.out.print("The category : " + name + " is update.\n\n");
		}
		con.disconnect();
	}

	
	//Fonctions ----

	
	//Ouverture de la connexion
	public static HttpURLConnection openConn(String url, String xKey) throws MalformedURLException, IOException
	{
		//Lancement de la connexion
		HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();
		
		//Mise en place du xKey et des options
		con.setRequestProperty("X-Key", xKey);
		con.setRequestProperty("Content-Type", "application/json");
		return (con);
	}
	
	//Fonction de connexion et envoie des informations
	public static HttpURLConnection connection(String url, String xKey, JSONObject jsonMessage, String method) throws IOException
	{
		//Lancement de la connexion pour remplir la requete
		HttpURLConnection con = openConn(url, xKey);
		con.setRequestProperty("Content-Length", Integer.toString(jsonMessage.length()));
		con.setRequestMethod(method);
		con.setDoOutput(true);
				        
		//Envoie des informations dans la connexion
		OutputStreamWriter sendMessage = new OutputStreamWriter(con.getOutputStream());
		sendMessage.write(jsonMessage.toString());
		sendMessage.flush();
		sendMessage.close();
		return (con);
	}
}
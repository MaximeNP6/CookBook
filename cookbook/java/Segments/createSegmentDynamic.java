import java.io.IOException;
import java.io.OutputStreamWriter;
import java.net.HttpURLconnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONException;
import org.json.JSONObject;

public class createSegmentDynamic 
{
	public static void main(String[] args) throws IOException, JSONException, InterruptedException
	{
		//Ici, renseignez la xKey et les parametres personnalises
		String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		String segmentId = "0123";	//Id du segment a modifier ('null' si le segment est a creer)
		
		String type = "dynamic";	//Code pour creer un segment Dynamique
		String name = "SegmentDynamic (java)";	//Nom du segment
		String description = "SegmentDynamic (java)";	//Description du segment
		String expiration = "2016-01-08T12:11:00Z";	//Date d'expiration du segment
		Boolean isTest = true;	//Le test est un segment de test : oui = 'true' / non = 'false'
		String parentId = null;	//Id du segment pere ('null' pour aucun segments pere)


		//On trouve l'addresse pour la requete
		String url = "http://v8.mailperformance.com/segments/";
		if (segmentId != null)
		{
			url = "http://v8.mailperformance.com/segments/" + segmentId;
		}
		
		//Creation du Json du message
		JSONObject jsonData = new JSONObject();
		jsonData.put("type", type);
		jsonData.put("name", name);
		jsonData.put("description", description);
		jsonData.put("expiration", expiration);
		jsonData.put("isTest", isTest);
		jsonData.put("parentId", parentId);
		
		//On affiche le message
		System.out.print(jsonData + "\n\n");
		
		//Lancement de la connexion
		HttpURLconnection con = null;
		if (segmentId != null)
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
			//Le segment a bien ete modifie
			System.out.print("The segment dynamic  : " + name + " is update.");
		}
		con.disconnect();
	}

	
	//Fonctions ----

	
	//Ouverture de la connexion
	public static HttpURLconnection openConn(String url, String xKey) throws MalformedURLException, IOException
	{
		//Lancement de la connexion
		HttpURLconnection con = (HttpURLconnection)new URL(url).openconnection();
		
		//Mise en place du xKey et des options
		con.setRequestProperty("X-Key", xKey);
		con.setRequestProperty("Content-Type", "application/json");
		return (con);
	}
	
	//Fonction de connexion et envoie des informations
	public static HttpURLconnection connection(String url, String xKey, JSONObject jsonMessage, String method) throws IOException
	{
		//Lancement de la connexion pour remplir la requete
		HttpURLconnection con = openConn(url, xKey);
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

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONException;
import org.json.JSONObject;

public class createSegmentStatic 
{
	public static void main(String[] args) throws IOException, JSONException, InterruptedException
	{
		//Ici, renseignez la xKey et les parametres personnalises
		String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		String segmentId = "0123";	//Id du segment a modifier ('null' si le segment doit etre cree)
		String unicity = "test@test.com"; //L'adresse mail de la cible a mettre dans le segment ('null' pour ne rien ajouter au segment)
		
		String type = "static";	//Code pour creer un segment Statique
		String name = "SegmentStatic (java)";	//Nom du segment
		String description = "SegmentStatic (java)";	//Description du segment
		String expiration = "2016-01-08T12:11:00Z";	//Date d'expiration du segment
		Boolean isTest = true;	//Segment de test : oui = 'true' / non = 'false'


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
		
		//On affiche le message
		System.out.print(jsonData + "\n");
		
		//Lancement de la connexion
		HttpURLConnection con = null;
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
			//Le segment a bien ete cree
			System.out.print("The segment static  : " + name + " is created / update.\n\n");
			
			if (unicity != null && segmentId != null)
			{
				//Nouvelle url
				url = "http://v8.mailperformance.com/targets?unicity=" + unicity;
				
				//Connexion
				con = openConn(url, xKey);
				con.setRequestMethod("GET");
				
				//Verification des reponses
				responseCode = con.getResponseCode();
				if (responseCode != 200)
				{
					//Affichage de l'erreur
					System.out.print("Error : " + responseCode + " " + con.getResponseMessage());
				}
				else
				{
					//Lecture des donnees ligne par ligne
					BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
					String reply = buffRead.readLine();
					con.disconnect();
					buffRead.close();
				
					//On recupere l'id de l'action
					JSONObject jObject  = new JSONObject(reply);
					String idTarget = jObject.getString("id");
					

					//Nouvelle url
					url = "http://v8.mailperformance.com/targets/" + idTarget + "/segments/" + segmentId;
					
					//Connexion
					con = openConn(url, xKey);
					con.setRequestMethod("POST");
					con.setRequestProperty("Content-Length", "0");
					con.setDoOutput(true);
			        
					//Envoi des informations dans la connection
					OutputStreamWriter sendMessage = new OutputStreamWriter(con.getOutputStream());
					sendMessage.flush();
					sendMessage.close();
					
					//Verification des reponses
					responseCode = con.getResponseCode();
					if (responseCode != 204)
					{
						//Affichage de l'erreur
						System.out.print("Error : " + responseCode + " " + con.getResponseMessage());
					}
					else
					{
						System.out.print("The target : " + unicity + " is add to the segment : " + name);
					}
				}
			}
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
	
	//Fonction de connexion et envoi des informations
	public static HttpURLConnection connection(String url, String xKey, JSONObject jsonMessage, String method) throws IOException
	{
		//Lancement de la connexion pour remplir la requete
		HttpURLConnection con = openConn(url, xKey);
		con.setRequestProperty("Content-Length", Integer.toString(jsonMessage.length()));
		con.setRequestMethod(method);
		con.setDoOutput(true);
				        
		//Envoi des informations dans la connexion
		OutputStreamWriter sendMessage = new OutputStreamWriter(con.getOutputStream());
		sendMessage.write(jsonMessage.toString());
		sendMessage.flush();
		sendMessage.close();
		return (con);
	}
}

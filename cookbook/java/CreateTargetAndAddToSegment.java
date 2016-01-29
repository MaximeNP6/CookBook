import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONException;
import org.json.JSONObject;

public class CreatTargetAndAddToSegment
{
	public static void main(String[] args) throws IOException, JSONException
	{
		//Ici, renseignez l'email de la cible, la X-Key et l'id du segment
		String unicity = "test@test.com";
		String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		String idSegment = "7502";

		//Syntaxe pour les differents types d'informations :
		String string = "name";	//Chaine de caracteres
		String listOfValues = "Mr";	//Liste de valeurs
		String email = "test@test.com";	//E-mail
		String phoneNumber = "0123456789";	//Telephone
		String textZone = "150 caracters max";	//Zone de texte
		int numbers = 123;	//Valeur numerique
		String date = "01/01/2000";	//Date
		String listMultipleValues[] = {"valeur 1", "valeur 2"};	//Liste de valeurs multiples

		//Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur du champ"
		JSONObject data = new JSONObject();
		data.put("5398", string);
		data.put("5399", listOfValues);
		data.put("5400", email);
		data.put("5452", phoneNumber);
		data.put("5453", textZone);
		data.put("5454", numbers);
		data.put("5455", date);
		data.put("5456", listMultipleValues);
		
		
		//Lancement de la connexion pour remplir la requete 'GET'
		String url = "http://v8.mailperformance.com/targets?unicity=" + unicity;
		HttpURLConnection con = openConn(url, xKey);
		con.setRequestMethod("GET");
		
		//Verification des reponses
		int responseCode = con.getResponseCode();
		if (responseCode != 200)
		{
			if (responseCode != 404)
			{
				//Affichage de l'erreur et fin du programme
				System.out.print("Error : " + responseCode + ' ' + con.getResponseMessage());
				System.exit(0);
			}
			else
			{
				//La cible n'existe pas, il faut la creer
				System.out.print("Error : " + responseCode + " : Creation of the target.\n");
				
				//Nouvelle url
				url = "http://v8.mailperformance.com/targets/";
				
				//Requete 'POST' pour creer la cible
				con = postOrPutOnTarget(con, "POST", data, xKey, url);
			}
		}
		//Lecture des donnees
		BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
		String reply = buffRead.readLine();
		con.disconnect();
		buffRead.close();
		System.out.print(reply + "\n");

		//On recupere l'id de la cible
		JSONObject jObject  = new JSONObject(reply);
		String idTarget = jObject.getString("id");

		//Nouvelle url
		url = "http://v8.mailperformance.com/targets/" + idTarget + "/segments/" + idSegment;
		
		//On remplit la requete 'POST'
		con = postOrPutOnTarget(con, "POST", data, xKey, url);
		System.out.println("The target " + unicity + " is added to the segment " + idSegment + ".");
		con.disconnect();
	}
	
	public static HttpURLConnection openConn(String url, String xKey) throws MalformedURLException, IOException
	{
		//Lancement de la connexion
		HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();
		
		//Mise en place du xKey et des options
		con.setRequestProperty("X-Key", xKey);
		con.setRequestProperty("Content-Type", "application/json");
		return (con);
	}
	
	public static HttpURLConnection postOrPutOnTarget(HttpURLConnection con, String request, JSONObject data, String xKey, String url) throws MalformedURLException, IOException
	{
		con.disconnect();
		//Lancement de la connexion pour remplir la requete
		con = openConn(url, xKey);
		con.setRequestProperty("Content-Length", Integer.toString(data.toString().length()));
		con.setRequestMethod(request);
		con.setDoOutput(true);
		//Envoi des informations dans la connexion
	    DataOutputStream payload = new DataOutputStream(con.getOutputStream());
	    payload.write(data.toString().getBytes());
	    payload.flush();
	    payload.close();
		
		//Verification des reponses
		int responseCode = con.getResponseCode();
		if (responseCode != 200 && responseCode != 204)
		{
			//Affichage de l'erreur
			System.out.print("Error : " + responseCode + ' ' + con.getResponseMessage());
			return (null);
		}
		else
		{
			return (con);
		}	
	}
}

import java.io.IOException;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class createValueList 
{
	public static void main(String[] args) throws IOException, JSONException, InterruptedException
	{
		//Ici, renseignez la xKey et les parametres personnalises du message
		String xKey = "KPYN0N97FE6C4B49DA1D66D6D89D617D061AB27F98E4BD0672EEEA87429CBD6450474AE50EB81DA2DBFAD0FA0B6A5B17F4ABC571D375A7E3225E8BD";
		String valueListsId = "706";	//Id de la liste a modifier ('null' si la liste est a creer)

		String name = "createValueList (java)";	//Nom de la liste
		Boolean ordered = false;	//Ordonnee : oui = 'true' / non = 'false'
		
		JSONArray values = new JSONArray();	//Valeurs de la liste
			JSONObject value1 = new JSONObject();	//Premiere valeur
				value1.put("index", 1);	//Numeros de la valeur
				value1.put("value", "Mr");	//Valeur du champs
			JSONObject value2 = new JSONObject();	//Premiere valeur
				value2.put("index", 2);	//Numeros de la valeur
				value2.put("value", "Mme");	//Valeur du champs
			values.put(value1);
			values.put(value2);
		
			
		//On trouve l'addresse pour la requete
		String url = "http://v8.mailperformance.com/valueLists/";
		if (valueListsId != null)
		{
			url = "http://v8.mailperformance.com/valueLists/" + valueListsId;
		}
		
		//Creation du Json du message
		JSONObject jsonData = new JSONObject();
		jsonData.put("name", name);
		jsonData.put("ordered", ordered);
		jsonData.put("values", values);
		
		//On affiche le message
		System.out.print(jsonData + "\n\n");
		
		//Lancement de la connexion
		HttpURLConnection con = null;
		if (valueListsId != null)
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
			//La liste a bien ete cree
			System.out.print("The value list  : " + name + " is created / update.");
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
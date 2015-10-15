import java.io.IOException;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONException;
import org.json.JSONObject;

public class createField 
{
	public static void main(String[] args) throws IOException, JSONException, InterruptedException
	{
		//Ici, renseignez la xKey et les parametres personnalises
		String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		String fieldId = "0123";	//Id du critere a modifier ('null' si le critere doit etre cree)

		String type = "numeric";	//Code pour creer un critere :
			//Normales : email = 'email' / telephone = 'phone' / zone de texte = 'textArena' / chaine de caractere = 'textField' / valeur numerique = 'numeric' / date = 'date'
			//Listes : liste de valeurs = 'singleSelectList' / Liste de valeurs multiples = 'multipleSelectList'
		String name = "createField (java)";	//Nom du critere
		Boolean isUnicity = false;	//Unicite : oui = 'true' / non = 'false'
		Boolean isMandatory = false;	//Obligatoire : oui = 'true' / non = 'false'

		int constraintOperator = 0;	//Pour les valeurs numeriques et les dates : 1 = '>' / 2 = '<' / 3 = '>=' / 4 = '<=' / 5 = '==' ('0' pour rien)
		String constraintValue = "42";	//Valeur de la contrainte ('null' pour rien)

		int valueListId = 0;	//Id de la liste dans le cas d'un 'singleSelectList' ou un 'multipleSelectList' ('0' pour rien)


		//On trouve l'adresse pour la requete
		String url = "http://v8.mailperformance.com/fields/";
		if (fieldId != null)
		{
			url = "http://v8.mailperformance.com/fields/" + fieldId;
		}
		
		//Creation du Json du message
		JSONObject jsonData = new JSONObject();
		jsonData.put("type", type);
		jsonData.put("name", name);
		jsonData.put("isUnicity", isUnicity);
		jsonData.put("isMandatory", isMandatory);
		
		if (constraintOperator != 0 && constraintValue != null)
		{
			JSONObject constraint = new JSONObject();
			constraint.put("operator", constraintOperator);
			constraint.put("value", constraintValue);
			jsonData.put("constraint", constraint);
		}
		else if (valueListId != 0)
		{
			jsonData.put("valueListId", valueListId);
		}
		
		//On affiche le message
		System.out.print(jsonData + "\n\n");
		
		//Lancement de la connexion
		HttpURLConnection con = null;
		if (fieldId != null)
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
			//Le critere a bien ete cree
			System.out.print("The field  : " + name + " is created / update.");
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

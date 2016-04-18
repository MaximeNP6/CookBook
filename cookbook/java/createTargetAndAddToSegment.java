import java.io.*;

import java.net.HttpURLConnection;

import java.util.Map;
import java.util.Properties;

import org.json.JSONException;
import org.json.JSONObject;

import Utils.Request;

public class createTargetAndAddToSegment
{
	public static void main(String[] args) throws IOException, JSONException
	{
		/*
			Renseignez ici les paramètres que vous souhaitez
			@var idSegment			=> ID du segment auquelle on veut ajouter la cible
		*/
		String idSegment 			= "01234";

		// Syntaxe pour les differents types d'informations :
		String string 				= "name";					// Chaine de caracteres
		String listOfValues 		= "Mr";						// Liste de valeurs
		String email 				= "test@test.com";			// E-mail
		String phoneNumber 			= "0123456789";				// Telephone
		String textZone 			= "150 caracters max";		// Zone de texte
		int numbers 				= 123;						// Valeur numerique
		String date 				= "01/01/2000";				// Date
		String listMultipleValues[] = {"valeur 1", "valeur 2"};	// Liste de valeurs multiples

		// Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur du champ"
		JSONObject data = new JSONObject();
		data.put("XXXX", string);
		data.put("XXXX", listOfValues);
		data.put("XXXX", email);
		data.put("XXXX", phoneNumber);
		data.put("XXXX", textZone);
		data.put("XXXX", numbers);
		data.put("XXXX", date);
		data.put("XXXX", listMultipleValues);


		Properties properties = new Properties();
		try {
			properties.load(new FileInputStream("config.properties"));
		} catch (IOException e) {
			System.out.print(e.getMessage());
		}
		String xKey = properties.getProperty("xKey");
		String baseUrl = properties.getProperty("url");
		String url = baseUrl + "targets?unicity=" + email;

		//Lancement de la connexion pour remplir la requete 'GET'
		Map<String, String> resp = Request.connection(url, xKey, null, "GET");

		//Verification des reponses
		if (!resp.get("code").equals("200")) {
			if (!resp.get("code").equals("404")) {
				//Affichage de l'erreur et fin du programme
				System.out.print("Error : " + resp.get("code") + ' ' + resp.get("mess"));
				System.exit(0);
			}
			else {
				//La cible n'existe pas, il faut la creer
				System.out.print("Error : " + resp.get("code") + " : Target creation...\n");
				
				//Nouvelle url
				url = baseUrl + "targets/";

				//Requete 'POST' pour creer la cible
				resp = Request.connection(url, xKey, data, "POST");
				System.out.print("Done !\n");
			}
		}
		// Lecture des donnees
		System.out.print(resp.get("mess") + "\n");

		//On recupere l'id de la cible
		JSONObject jObject  = new JSONObject(resp.get("reply"));
		String idTarget = jObject.getString("id");

		//Nouvelle url
		url = baseUrl + "targets/" + idTarget + "/segments/" + idSegment;

		//On remplit la requete 'POST'
		resp = Request.connection(url, xKey, data, "POST");
		System.out.println("Target " + email + " has been added to the segment " + idSegment + ".");
	}
}

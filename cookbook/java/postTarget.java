import java.io.*;

import java.util.Map;
import java.util.Properties;

import org.json.JSONException;
import org.json.JSONObject;

import Utils.Request;

public class postTarget
{
	public static void main(String[] args) throws IOException, JSONException
	{
		//Syntaxe pour les differents types d'informations :
		String string = "name";	//Chaine de caracteres
		String listOfValues = "Mr";	//Liste de valeurs
		String email = "dare@test.com";	//E-mail
		String phoneNumber = "0123456789";	//Telephone
		String textZone = "150 caracters max";	//Zone de texte
		int numbers = 123;	//Valeur numerique
		String date = "01/01/2000";	//Date
		String listMultipleValues[] = {"valeur 1", "valeur 2"};	//Liste de valeurs multiples

		//Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur du champ"
		JSONObject data = new JSONObject();
		data.put("8630", string);
		data.put("8631", listOfValues);
		data.put("8628", email);


		Properties properties   = new Properties();

		try {
			properties.load(new FileInputStream("config.properties"));
		} catch (IOException e) {
			System.out.print(e.getMessage());
		}

		String xKey = properties.getProperty("xKey");
		String baseUrl = properties.getProperty("url");
		String url = baseUrl + "targets?unicity=" + email;

		Map<String, String> resp = Request.connection(url, xKey, null, "GET");

		url = baseUrl + "targets/";
		// Verification des reponses
		if (resp.get("code").equals("200")) {
			// Lecture des données
			System.out.print(resp.get("reply") + "\n");
			Request.connection(url, xKey, data, "PUT");
		}
		else {
			if (!resp.get("code").equals("404")) {
				// Affichage de l'erreur
				System.out.print("Error : " + resp.get("code") + ' ' + resp.get("reply"));
			}
			else {
				System.out.print("Error : " + resp.get("code") + " : Creation of the target.\n");
				Request.connection(url, xKey, data, "POST");
				System.out.print("The target : " + email + " has been created.\n");
      }
		}
	}
}

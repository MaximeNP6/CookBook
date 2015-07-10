package getTargetFromUnicity;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

public class getTargetFromUnicity
{
	public static void main(String[] args) throws IOException
	{
		//Ici, renseignez l'email dont vous voulez obtenir les valeurs des champs
		String unicity = "test@test.com";
		
		//Lancement de la connection pour remplir la requete
		String url = "http://v8.mailperformance.com/targets?unicity=" + unicity;
		HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();
		con.setRequestMethod("GET");
		
		//Mise en place du xKey et des options
		con.setRequestProperty("X-Key", "ABCDEFJHIJKLMNOPQRSTUVWXYZ0123456789");
		con.setRequestProperty("Content-Type", "application/json");
		
		//Verification des reponses
		String responseMessage = con.getResponseMessage();
		int responseCode = con.getResponseCode();
		if (responseCode != 200 && responseCode != 201)
		{
			//Affichage de l'erreur
			System.out.print("Error : " + Integer.toString(responseCode) + ' ' + responseMessage);
		}
		else
		{
			//Lecture des donnees ligne par lignes
			BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
			String reply = buffRead.readLine();
			buffRead.close();
            
			//Affichage des donnees
			System.out.print(reply);
		}
		con.disconnect();
	}
}


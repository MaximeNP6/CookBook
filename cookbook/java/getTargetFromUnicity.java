import java.io.FileInputStream;
import java.io.IOException;


import java.util.Map;
import java.util.Properties;

import Utils.Request;

public class getTargetFromUnicity
{
	public static void main(String[] args) throws IOException
	{
		/*
			Renseignez ici les paramètres que vous souhaitez
			@var unicity		=> le paramètre d'unicité que vous utilisez (ici l'e-mail)
		*/
		String unicity = "test@test.com";


		Properties properties = new Properties();

		try {
			properties.load(new FileInputStream("config.properties"));
		} catch (IOException e) {
			System.out.print(e.getMessage());
		}


		String xKey = properties.getProperty("xKey");
		String baseUrl = properties.getProperty("url");
		String url = baseUrl + "targets?unicity=" + unicity;

		Map<String, String> resp = null;
		resp = Request.connection(url, xKey, null, "GET");

		// Vérification de la réponse
		if (resp.get("code").equals("200")) {
			// Affichage des données
			System.out.print(resp.get("reply"));
		}
		else {
			// Affichage de l'erreur
			System.out.print("Error : " + resp.get("code") + " " + resp.get("mess"));
		}
	}
}

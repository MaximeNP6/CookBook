import org.json.JSONException;
import org.json.JSONObject;

import java.io.FileInputStream;
import java.io.IOException;

import java.util.Map;
import java.util.Properties;

import Utils.Request;

public class createCategory
{
    public static void main(String[] args) throws IOException, JSONException, InterruptedException
    {
        /*
            Renseignez ici les paramètres que vous souhaitez
            @var categoriesId 		=> ID de la catégorie à modifier (renseignez null pour la créer)
            @var name 				=> nom de la catégorie
            @var description 		=> description de la catégorie
        */
        String categoriesId = null;
        String name         = "Category java";
        String description  = "Category (java)";

        Properties properties = new Properties();

        try {
            properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
            System.out.print(e.getMessage());
        }

        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");
        String url = baseUrl + "categories/";

        if (categoriesId != null) {
            url += categoriesId;
        }

        // Creation du Json du message
        JSONObject jsonData = new JSONObject();
        jsonData.put("name", name);
        jsonData.put("description", description);

        // On affiche le message
        System.out.print(jsonData + "\n\n");


        // Lancement de la connexion
        Map<String, String> resp;
        if (categoriesId != null) {
            resp = Request.connection(url, xKey, jsonData, "PUT");
        }
        else {
            resp = Request.connection(url, xKey, jsonData, "POST");
        }

        // Verification des reponses
        if (resp.get("code").equals("200")) {
            // La categorie a bien ete créée
            System.out.print("Category : " + name + " has been created / updated.\n\n");
        }
        else {
            // Affichage de l'erreur
            System.out.print("Error : " + resp.get("code") + " " + resp.get("mess"));
        }
    }
}

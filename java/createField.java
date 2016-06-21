import java.io.FileInputStream;
import java.io.IOException;

import java.util.Map;
import java.util.Properties;

import org.json.JSONException;
import org.json.JSONObject;

import Utils.Request;

public class createField {
    public static void main(String[] args) throws IOException, JSONException, InterruptedException {
        /*
            Renseignez ici les paramètres que vous souhaitez
            @var fieldId			=> ID du champ à créer/modifier ('null' si le champ doit être créée)
            @var type				=> Type du champ à créer/modifier
                                       pour le détail des différents types : https://backoffice.mailperformance.com/doc/#api-Field)
            @var name				=> Nom du champ
            @var isUnicity			=> Défini si le champ doit être un critère d'unicité ou non
            @var isMandatory		=> Défini si le champ doit être oubligatoire ou non
            @var constraintOperator	=> Pour les valeurs numeriques et les dates : 1 = '>' / 2 = '<' / 3 = '>='
                                       4 = '<=' / 5 = '==' ('null' pour ne pas ajouter)
            @var constraintValue	=> Valeur de la contrainte ('null' pour ne pas ajouter)
            @var valueListId		=> ID de la liste associée ('null' pour ne pas ajouter)
        */
        String fieldId = null;
        String type = "numeric";
        String name = "createField (java)";
        Boolean isUnicity = false;
        Boolean isMandatory = false;
        int constraintOperator = 0;
        String constraintValue = "42";
        int valueListId = 0;


        Properties properties = new Properties();
        try {
            properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
            System.out.print(e.getMessage());
        }
        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");

        String url = baseUrl + "fields/";

        if (fieldId != null) {
            url += fieldId;
        }

        // Creation du Json du message
        JSONObject jsonData = new JSONObject();
        jsonData.put("type", type);
        jsonData.put("name", name);
        jsonData.put("isUnicity", isUnicity);
        jsonData.put("isMandatory", isMandatory);

        if (constraintOperator != 0 && constraintValue != null) {
            JSONObject constraint = new JSONObject();
            constraint.put("operator", constraintOperator);
            constraint.put("value", constraintValue);
            jsonData.put("constraint", constraint);
        } else if (valueListId != 0) {
            jsonData.put("valueListId", valueListId);
        }

        //On affiche le message
        System.out.print(jsonData + "\n\n");

        //Lancement de la connexion
        Map<String, String> resp = null;
        if (fieldId != null) {
            resp = Request.connection(url, xKey, jsonData, "PUT");
        } else {
            resp = Request.connection(url, xKey, jsonData, "POST");
        }

        //Verification des reponses
        if (resp.get("code").equals("200")) {
            //Le critere a bien ete cree
            System.out.print("The field  : " + name + " has been created / updated.");
        } else {
            // Affichage de l'erreur
            System.out.print("Error : " + resp.get("code") + " " + resp.get("mess"));
        }
    }
}

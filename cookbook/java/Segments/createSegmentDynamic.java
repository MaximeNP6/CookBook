package Segments;

import java.io.FileInputStream;
import java.io.IOException;

import java.util.Map;
import java.util.Properties;

import org.json.JSONException;
import org.json.JSONObject;

import Utils.Request;

public class createSegmentDynamic 
{
    public static void main(String[] args) throws IOException, JSONException, InterruptedException
    {
        /*
        ** Renseignez ici les paramètres que vous souhaitez
        ** @var segmentId           => ID du segment à modifier (renseignez null pour le créer)
        ** @var type                => Type du segment ('static' ou 'dynamic')
        ** @var name                => Nom du segment
        ** @var description         => Description du segment
        ** @var expiration          => Date d'expiration du segment
        ** @var isTest              => Définit si le segment est un segment de test ou non (true: segment de test // false: n'est pas segment de test)
        ** @var parentId            => ID du segment parent (renseignez null s'il n'en a pas)
        */
        String segmentId    = null;
        String type         = "dynamic";
        String name         = "SegmentDynamic (java)";
        String description  = "SegmentDynamic (java)";
        String expiration   = "2026-01-08T12:11:00Z";
        Boolean isTest      = true;
        String parentId     = null;


        Properties properties = new Properties();
        try {
            properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
            System.out.print(e.getMessage());
        }
        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");
        String url = baseUrl + "/V1/segments/";

        if (segmentId != null)
        {
            url += segmentId;
        }

        //Creation du Json du message
        JSONObject jsonData = new JSONObject();
        jsonData.put("type", type);
        jsonData.put("name", name);
        jsonData.put("description", description);
        jsonData.put("expiration", expiration);
        jsonData.put("isTest", isTest);
        jsonData.put("parentId", parentId);

        //On affiche le message
        System.out.print(jsonData + "\n\n");

        //Lancement de la connexion
        Map<String, String> resp;
        if (segmentId != null) {
            resp = Request.connection(url, xKey, jsonData, "PUT");
        }
        else {
            resp = Request.connection(url, xKey, jsonData, "POST");
        }

        //Verification des reponses
        if (resp.get("code").equals("200")) {
            //Le segment a bien ete modifie
            System.out.print("The segment dynamic  : " + name + " has been updated.");
        }
        else {
            //Affichage de l'erreur
            System.out.print("Error : " + resp.get("code") + " " + resp.get("reply"));
        }
    }
}

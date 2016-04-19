package Imports;

import Utils.Request;

import java.io.FileInputStream;
import java.io.IOException;

import java.util.Map;
import java.util.Properties;

public class getImports
{
    public static void main(String[] args) throws Exception
    {
        Properties properties = new Properties();
        try {
            properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
            System.out.print(e.getMessage());
        }
        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");

        //Lancement de la connexion pour remplir la requete
        String url = baseUrl + "imports/";

        Map<String, String> resp = Request.connection(url, xKey, null, "GET");

        if (resp.get("mess").equals("OK")) {
            //Lecture des donnees
            System.out.print(resp.get("reply") + "\n");
        }
        else {
            System.out.print("Error " + resp.get("code") + ": " + resp.get("mess") + "\n");
        }
    }
}

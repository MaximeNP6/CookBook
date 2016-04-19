import Utils.Request;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.*;

import java.util.Map;
import java.util.Properties;


public class sendMessageOnSegment
{

    public static void main(String[] args) throws Exception
    {
        String segmentId = "01234";	//Id du segment
        String actionId = "000ABC";	//Id de l'action a dupliquer

        Properties properties   = new Properties();

        try {
            properties.load(new FileInputStream("config.properties"));
        } catch (IOException e) {
            System.out.print(e.getMessage());
        }

        String xKey = properties.getProperty("xKey");
        String baseUrl = properties.getProperty("url");

        //Nouvelle Url
        String url = baseUrl + "segments/" + segmentId + "/targets/";

        Map<String, String> resp = Request.connection(url, xKey, null, "GET");

        //Verification des reponses
        if (!resp.get("code").equals("200")) {
            System.out.print("Error : " + resp.get("code") + ' ' + resp.get("mess") + ".");
            System.exit(0);
        }

        JSONArray jsonArray = new JSONArray(resp.get("reply"));
        JSONObject empty = new JSONObject();

        for (int i = 0; i < jsonArray.length(); i++){
            //Nouvelle url
            url = baseUrl + "actions/" + actionId + "/targets/" + jsonArray.get(i).toString();
            //SendMessage
            resp = Request.connection(url, xKey, empty, "POST");
            System.out.printf("url : " + url + "\n");
            if (resp.get("code").equals("204"))
                System.out.printf("OK\n");
            else
                System.out.println("Error: " + resp.get("code") + " " + resp.get("mess") + " with target \"" +
                        jsonArray.get(i).toString() + "\"");

        }
    }
}

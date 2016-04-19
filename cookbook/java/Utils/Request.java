package Utils;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.IOException;

import java.net.HttpURLConnection;
import java.net.URL;

import java.util.HashMap;
import java.util.Map;

import org.json.JSONException;
import org.json.JSONObject;

public class Request {
    /**
     * Initialisation de la connexion
     * @param url String
     * @param xKey String
     * @return La connexion http avec l'API
     * @throws IOException
    */
    public static HttpURLConnection openConn(String url, String xKey) throws IOException {
        HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();

        con.setRequestProperty("X-Key", xKey);
        con.setRequestProperty("Content-Type", "application/json");
        return (con);
    }

    /**
     * Connexion et envoie de la requête
     * @param url String
     * @param xKey String
     * @param jsonMessage JSONObject
     * @param method String
     * @return Reponses de la connection
     * @throws IOException
    */
    public static Map<String, String> connection(String url,
                                                 String xKey,
                                                 JSONObject jsonMessage,
                                                 String method) throws IOException {
        String jsonLength = "";
        Map<String, String> resp  = new HashMap<>();

        if (jsonMessage != null) {
            jsonLength = Integer.toString(jsonMessage.length());
        }
        HttpURLConnection con = openConn(url, xKey);
        con.setRequestProperty("Content-Length", jsonLength);
        con.setRequestMethod(method);
        con.setDoOutput(true);

        if (jsonMessage != null) {
            OutputStreamWriter sendMessage = new OutputStreamWriter(con.getOutputStream());
            sendMessage.write(jsonMessage.toString());
            sendMessage.flush();
            sendMessage.close();
        }
        resp.put("code", String.valueOf(con.getResponseCode()));
        if (con.getResponseMessage() != null)
            resp.put("mess", con.getResponseMessage());
        if (resp.get("mess").equals("OK") && con.getInputStream() != null) {
            BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
            resp.put("reply", "");
            while (buffRead.ready())
                resp.put("reply", resp.get("reply") + buffRead.readLine());
            buffRead.close();
        }
        con.disconnect();
        return (resp);
    }

    /**
     * Fonction d'attente de changement d'état de l'action
     * @param idAction String
     * @param xKey String
     * @param baseUrl String
     * @return Reponses de la connection
     * @throws InterruptedException
     * @throws IOException
     * @throws JSONException
     */
    public static int waitForState(String idAction,
                                   String xKey,
                                   String baseUrl) throws InterruptedException, IOException, JSONException
    {
        int actionState = 30;
        Map<String, String> resp;

        while (actionState != 38 && actionState != 20 && actionState != 10)
        {
            //On attend 20 secondes
            System.out.print("Wait 20sec...\n");
            Thread.sleep(20000);

            //Nouvelle adresse
            String url = baseUrl + "/actions/" + idAction;

            resp = Request.connection(url, xKey, null, "GET");

            //On recupere l'etat de l'action
            JSONObject jObject  = new JSONObject(resp.get("reply"));
            actionState = jObject.getJSONObject("informations").getInt("state");
        }
        return (actionState);
    }
}

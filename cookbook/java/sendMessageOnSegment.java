import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;


public class SendMessageOnSegment
{

    public static void main(String[] args) throws Exception
    {
        //Url de base
        String urlBase = "http://v8.mailperformance.com/";

        //X-Key
        String xKey = "ABCDEFJHIJKLMNOPQRSTUVWXYZ0123456789";

        String segmentId = "1234";	//Id du segment
        String actionId = "00012A";	//Id de l'action a dupliquer


        HttpURLConnection con = null;
        String verif = null;

        //Nouvelle Url
        String url = urlBase + "segments/" + segmentId + "/targets/";

        con = urlGet(url, xKey);
        if ((verif = verification(con)) == null)
            System.exit(0);
        System.out.printf("OK : " + verif + "\n\n");

        String idTarget[] = myTab(verif);

        //Pour chaque Taget on fait un SendMessage
        for (int i = 0; i < idTarget.length; i++)
        {
            //Nouvelle url
            url = urlBase + "actions/" + actionId + "/targets/" + idTarget[i];
            //SendMessage
            con = urlPostOrPut(con, "POST", "", xKey, url);
            System.out.printf("url : " + url + "\n");
            if ((verif = verification(con)) == null)
                System.exit(0);
            System.out.printf("OK\n\n");
        }

        con.disconnect();
    }


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DU PROGRAMME////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DES FONCTIONS/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static String[] myTab(String result)
    {
        String str = "";

        //On transforme la string en tableau de char
        char[] charArray = result.toCharArray();

        //On regarde tout le tableau de char
        for (int i = 0; i < charArray.length; i++)
        {
            //Si un caractere et bon, on le garde
            if (((int)(charArray[i]) >= (int)'0' && (int)(charArray[i]) <= (int)'9')
                    || ((int)(charArray[i]) >= (int)'A' && (int)(charArray[i]) <= (int)'Z')
                    || charArray[i] == ',')
            {
                str = str + charArray[i];
            }
        }
        //On transforme la string en tableau de string
        String[] tab = str.split(",");

        //On return
        return (tab);
    }

    public static String verification(HttpURLConnection con) throws IOException
    {
        //Verification des reponses
        int responseCode = con.getResponseCode();
        if (responseCode != 200 && responseCode != 204)
        {
            //Affichage de l'erreur
            System.out.print("Error : " + responseCode + ' ' + con.getResponseMessage() + ".");
            return (null);
        }
        return (readConnection(con));
    }

    public static String readConnection(HttpURLConnection con) throws IOException
    {
        String str = null;
        //Lecture des donnees
        BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
        String reply = buffRead.readLine();
        while ((str = buffRead.readLine()) != null)
        {
            reply = reply + str;
        }
        buffRead.close();
        if (reply == null)
            return ("");
        return (reply);
    }

    //Fonctions de connexion
    public static HttpURLConnection startCon(String url, String xKey) throws MalformedURLException, IOException
    {
        //Lancement de la connexion
        HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();

        //Mise en place du xKey et des options
        con.setRequestProperty("X-Key", xKey);
        con.setRequestProperty("Content-Type", "application/json");
        return (con);
    }

    public static HttpURLConnection urlGet(String url, String xKey) throws MalformedURLException, IOException
    {
        //On remplit la requete 'GET'
        HttpURLConnection con = startCon(url, xKey);
        con.setRequestMethod("GET");

        return (con);
    }

    public static HttpURLConnection urlPostOrPut(HttpURLConnection con, String request, Object dataJson, String xKey, String url) throws MalformedURLException, IOException
    {
        con.disconnect();
        //Lancement de la connexion pour remplir la requete
        con = startCon(url, xKey);
        con.setRequestProperty("Content-Length", Integer.toString(dataJson.toString().length()));
        con.setRequestMethod(request);
        con.setDoOutput(true);

        // Envoie des informations dans la connexion
        DataOutputStream streamCon = new DataOutputStream(con.getOutputStream());
        streamCon.write(dataJson.toString().getBytes());
        streamCon.flush();
        streamCon.close();

        //Verification des reponses
        int responseCode = con.getResponseCode();
        if (responseCode != 200 && responseCode != 204)
        {
            //Affichage de l'erreur
            System.out.print("Error : " + responseCode + ' ' + con.getResponseMessage() + ".\nEnd urlPostOrPut");
            System.exit(1);
        }
        return (con);
    }
}

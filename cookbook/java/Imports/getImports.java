import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

public class getImports
{
    public static void main(String[] args) throws Exception
    {
        //Url de base
        String urlBase = "http://v8.mailperformance.com/";
        //X-Key
        String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        //Lancement de la connexion pour remplir la requete
        String url = urlBase + "imports/";
        HttpURLConnection con = null;

        con = urlGet(url, xKey);

        //Lecture des donnees
        BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
        String reply = buffRead.readLine();
        buffRead.close();
        System.out.print(reply + "\n");

        con.disconnect();
    }


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DU PROGRAMME////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DES FONCTIONS/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //Fonctions de connexion
    public static HttpURLConnection startCon(String url, String xKey) throws MalformedURLException, IOException
    {
        //Lancement de la connexion
        HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();

        //Mise en place de la xKey et des options
        con.setRequestProperty("X-Key", xKey);
        con.setRequestProperty("Content-Type", "application/json");
        return (con);
    }

    public static HttpURLConnection urlGet(String url, String xKey) throws MalformedURLException, IOException
    {
        //On remplit la requete 'GET'
        HttpURLConnection con = startCon(url, xKey);
        con.setRequestProperty("Accept", "application/vnd.mperf.v8.import.v1+json");
        con.setRequestMethod("GET");

        return (con);
    }
}

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONException;
import org.json.JSONObject;

public class createMailCampaignAction
{
  public static void main(String[] args) throws IOException, JSONException, InterruptedException
  {
    Object NULL = null;
    //Ici, renseignez la xKey
    String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    String type = "mailCampaign";	//Code pour envoyer une campagne de mail
    String name = "MailCampaignFromApi (java)";	//Nom de l'action
    String description = "MailCampaignFromApi (java)";	//Description de l'action

    int informationFolder = 0123;	//Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
    int informationCategory = 0123;	//Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

    String contentHeadersFromPrefix = "prefix";	//Adresse expeditice
    String contentHeadersFromLabel = "label";	//Libelle expediteur
    String contentHeadersReply = "address@reply.com"; //Adresse de reponse

    String contentSubject = "Subject of the message";	//Objet du mail
    String contentHTML = "Html message";	//Message HTML
    String contentText = "Text message";	//Message texte

    int idTestSegment[] = {0123};	//Id du segment de test pour la validation
    int idSelectSegment[] = {0123};	//Ids des segments selectionnes



    //On trouve l'adresse pour la requete
    String url = "http://v8.mailperformance.com/actions";

    //Creation du Json du message
    JSONObject informations = new JSONObject();
    informations.put("folder", informationFolder);
    informations.put("category", informationCategory);

    JSONObject contentHeadersFrom = new JSONObject();
    contentHeadersFrom.put("prefix", contentHeadersFromPrefix);
    contentHeadersFrom.put("label", contentHeadersFromLabel);

    JSONObject contentHeaders = new JSONObject();
    contentHeaders.put("from", contentHeadersFrom);
    contentHeaders.put("reply", contentHeadersReply);

    JSONObject segments = new JSONObject();
    segments.put("selected", idSelectSegment);

    JSONObject scheduler = new JSONObject();
    scheduler.put("type", "asap");	//Envoie : immediat = 'asap' / Date = 'scheduled'
    //scheduler.put("startDate", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever
    scheduler.put("segments", segments);

    JSONObject content = new JSONObject();
    content.put("headers", contentHeaders);
    content.put("subject", contentSubject);
    content.put("html", contentHTML);
    content.put("text", contentText);

    JSONObject jsonData = new JSONObject();
    jsonData.put("type", type);
    jsonData.put("name", name);
    jsonData.put("description", description);
    jsonData.put("informations", informations);
    jsonData.put("scheduler", scheduler);
    jsonData.put("content", content);

    //On affiche le message
    System.out.print(jsonData + "\n");


    //Lancement de la connexion
    HttpURLConnection con = connection(url, xKey, jsonData);

    //Verification des reponses
    int responseCode = con.getResponseCode();
    if (responseCode != 200)
    {
      //Affichage de l'erreur
      System.out.print("Error : " + responseCode + " " + con.getResponseMessage());
    }
    else
    {
      System.out.print("Action : " + name + " created.\n\n");

      //Lecture des donnees ligne par lignes
      BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
      String reply = buffRead.readLine();
      con.disconnect();
      buffRead.close();

      //On recupere l'id de l'action
      JSONObject jObject  = new JSONObject(reply);
      String idAction = jObject.getString("id");

      //On valide l'action
      url = "http://v8.mailperformance.com/actions/" + idAction + "/validation";


      JSONObject jsonTest = new JSONObject();
      jsonTest.put("fortest", true);	//Phase de test
      jsonTest.put("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
      jsonTest.put("testSegments", idTestSegment);	//Les Ids des differents segments de tests
      jsonTest.put("mediaForTest", NULL);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
      jsonTest.put("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
      jsonTest.put("comments", NULL);	//Commentaire ('NULL' pour aucuns commentaires)

      //On affiche le Json
      System.out.print(jsonTest + "\n");

      //Lancement de la connexion
      con = connection(url, xKey, jsonTest);

      //Verification des reponses
      responseCode = con.getResponseCode();

      int actionState = waitForState(idAction, xKey);

      if (responseCode != 204 || actionState != 38)
      {
        //Affichage de l'erreur
        if (actionState == 20)
          System.out.print("Error : the test failed.");
        else if (actionState == 10)
          System.out.print("Error : check the campaign in the Backoffice.");
        else
          System.out.print("Error : " + responseCode + " " + con.getResponseMessage());
      }
      else
      {
        //La phase de test a reussi
        System.out.print("The action " + name + " is tested.\n\n");

        //Creation du Json du message pour la validation
        JSONObject jsonValid = new JSONObject();
        jsonValid.put("fortest", false);	//Phase de test
        jsonValid.put("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
        jsonValid.put("testSegments", NULL);	//Les Ids des differents segments de tests
        jsonValid.put("mediaForTest", NULL);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
        jsonValid.put("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
        jsonValid.put("comments", NULL);	//Commentaire ('NULL' pour aucuns commentaires)

        //On affiche le Json
        System.out.print(jsonValid + "\n");


        //Lancement de la connexion
        con = connection(url, xKey, jsonValid);

        //Verification des reponses
        responseCode = con.getResponseCode();
        if (responseCode != 204)
        {
          //Affichage de l'erreur
          System.out.print("Error : " + responseCode + " " + con.getResponseMessage());
        }
        else
        {
          //La phase de validation a reussi
          System.out.print("The action " + name + " is validated.\n\n");
        }
      }
    }
    con.disconnect();
  }


  //Fonctions ----


  //Ouverture de la connexion
  public static HttpURLConnection openConn(String url, String xKey) throws MalformedURLException, IOException
  {
    //Lancement de la connexion
    HttpURLConnection con = (HttpURLConnection)new URL(url).openConnection();

    //Mise en place du xKey et des options
    con.setRequestProperty("X-Key", xKey);
    con.setRequestProperty("Content-Type", "application/json");
    return (con);
  }

  //Fonction de connexion et envoi des informations
  public static HttpURLConnection connection(String url, String xKey, JSONObject jsonMessage) throws IOException
  {
    //Lancement de la connexion pour remplir la requete
    HttpURLConnection con = openConn(url, xKey);
    con.setRequestProperty("Content-Length", Integer.toString(jsonMessage.length()));
    con.setRequestMethod("POST");
    con.setDoOutput(true);

    //Envoi des informations dans la connexion
    OutputStreamWriter sendMessage = new OutputStreamWriter(con.getOutputStream());
    sendMessage.write(jsonMessage.toString());
    sendMessage.flush();
    sendMessage.close();
    return (con);
  }

  //Fonction d'attente de fin de test
  public static int waitForState(String idAction, String xKey) throws InterruptedException, MalformedURLException, IOException, JSONException
  {
    int actionState = 30;

    while (actionState != 38 && actionState != 20 && actionState != 10)
    {
      //On attend 20 secondes
      System.out.print("Wait 20sec...\n");
      Thread.sleep(2000);

      //Nouvelle adresse
      String url = "http://v8.mailperformance.com/actions/" + idAction;

      //Lancement de la connexion pour remplir la requete
      HttpURLConnection con = openConn(url, xKey);
      con.setRequestMethod("GET");

      //Lecture des donnees ligne par ligne
      BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
      String reply = buffRead.readLine();
      con.disconnect();
      buffRead.close();

      //On recupere l'etat de l'action
      JSONObject jObject  = new JSONObject(reply);
      actionState = jObject.getJSONObject("informations").getInt("state");
    }
    return (actionState);
  }
}

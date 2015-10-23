import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import org.json.JSONException;
import org.json.JSONObject;

public class duplicateAndValidate
{
  public static void main(String[] args) throws IOException, JSONException, InterruptedException
  {
    Object NULL = null;
    //Ici, renseignez la xKey et les parametres personnalises
    String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    String unicity = "test@test.com";	//Email de la cible
    int[] idSegment = {0123};	//Id du segment
    String idAction = "000ABC";	//Id de l'action a dupliquer
    int[] idTestSegment = {0123};	//Id du segment de test

    //On cherche la cible a ajouter dans le segment

    //Nouvelle url
    String url = "http://v8.mailperformance.com/targets?unicity=" + unicity;

    //Lancement de la connexion
    HttpURLConnection con = connection(url, xKey, null, "GET");

    //Verification des reponses
    int responseCode = con.getResponseCode();
    if (responseCode != 200)
    {
      //Affichage de l'erreur et fin du programme
      System.out.print("Error request 'GET' : " + responseCode + ' ' + con.getResponseMessage());
    }
    else
    {
      //Lecture des donnees
      BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
      String reply = buffRead.readLine();
      con.disconnect();
      buffRead.close();

      //On affiche la cible
      System.out.print(reply + "\n");

      //On recupere l'id de la cible
      JSONObject jObject  = new JSONObject(reply);
      String idTarget = jObject.getString("id");

      //Nouvelle url
      url = "http://v8.mailperformance.com/targets/" + idTarget + "/segments/" + idSegment[0];

      JSONObject data = new JSONObject();


      //On ajoute la cible au segment
      con = connection(url, xKey, data, "POST");

      //Verification des reponses
      responseCode = con.getResponseCode();
      if (responseCode != 204)
      {
        //Affichage de l'erreur et fin du programme
        System.out.print("Error request 'POST' : " + responseCode + ' ' + con.getResponseMessage());
      }
      else
      {
        System.out.println("The target " + unicity + " is added to the segment " + idSegment[0] + ".\n");

        //Duplication de l'action

        //Nouvelle url
        url = "http://v8.mailperformance.com/actions/" + idAction + "/duplication";

        //On duplique a l'identique
        con = connection(url, xKey, data, "POST");

        //Verification des reponses
        responseCode = con.getResponseCode();
        if (responseCode != 200)
        {
          //Affichage de l'erreur et fin du programme
          System.out.print("Error request 'POST' duplication : " + responseCode + ' ' + con.getResponseMessage());
        }
        else
        {
          //Lecture des donnees
          buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
          reply = buffRead.readLine();
          con.disconnect();
          buffRead.close();

          //On affiche la cible
          System.out.print("The action duplicate ( " + new JSONObject(reply).getString("name") + " ) is created.\n\n");
          System.out.print(reply + "\n");

          //On recupere l'id de l'action
          data  = new JSONObject(reply);
          String idNewAction = data.getString("id");


          //Modification de la nouvelle action
          JSONObject segments = new JSONObject();
          segments.put("selected", idSegment);

          JSONObject scheduler = new JSONObject();
          scheduler.put("type", "asap");	//Envoi : immediat = 'asap' / Date = 'scheduled'
          //scheduler.put("startDate", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever
          scheduler.put("segments", segments);

          data.put("scheduler", scheduler);

          //Nouvelle url
          url = "http://v8.mailperformance.com/actions/" + idNewAction;

          //On duplique a l'identique
          con = connection(url, xKey, data, "PUT");

          //Verification des reponses
          responseCode = con.getResponseCode();
          if (responseCode != 200)
          {
            //Affichage de l'erreur et fin du programme
            System.out.print("Error request 'PUT' modification : " + responseCode + ' ' + con.getResponseMessage());
          }
          else
          {
            System.out.print("The action duplicate ( " + data.getString("name") + " ) is update.\n\n");

            //On teste l'action

            //Nouvelle url
            url = "http://v8.mailperformance.com/actions/" + idNewAction + "/validation";

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
            con = connection(url, xKey, jsonTest, "POST");

            //Verification des reponses
            responseCode = con.getResponseCode();

            int actionState = waitForState(idNewAction, xKey);

            if (responseCode != 204 || actionState == 20)
            {
              //Affichage de l'erreur
              if (actionState == 20)
                System.out.print("Error : the test failed.");
              else
                System.out.print("Error : " + responseCode + " " + con.getResponseMessage());
            }
            else
            {
              //La phase de test a reussi
              System.out.print("The action " + data.getString("name") + " is tested.\n\n");

              //On valide l'action

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
              con = connection(url, xKey, jsonValid, "POST");

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
                System.out.print("The action " + data.getString("name") + " is validated.\n\n");
              }
            }
          }
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

  //Fonction de connexion et envoie des informations
  public static HttpURLConnection connection(String url, String xKey, JSONObject jsonMessage, String method) throws IOException
  {
    //Lancement de la connexion pour remplir la requete
    HttpURLConnection con = openConn(url, xKey);
    if (jsonMessage != null)
      con.setRequestProperty("Content-Length", Integer.toString(jsonMessage.length()));
    else
      con.setRequestProperty("Content-Length", "0");
    con.setRequestMethod(method);
    con.setDoOutput(true);

    // Envoi des informations dans la connexion
    if (jsonMessage != null)
    {
      OutputStreamWriter sendMessage = new OutputStreamWriter(con.getOutputStream());
      sendMessage.write(jsonMessage.toString());
      sendMessage.flush();
      sendMessage.close();
    }
    return (con);
  }

  //Fonction d'attente de fin de test
  public static int waitForState(String idAction, String xKey) throws InterruptedException, MalformedURLException, IOException, JSONException
  {
    int actionState = 30;

    while (actionState != 38 && actionState != 20)
    {
      //On attend 20 secondes
      System.out.print("Wait 20sec...\n");
      Thread.sleep(2000);

      //Nouvelle adresse
      String url = "http://v8.mailperformance.com/actions/" + idAction;

      //Lancement de la connexion pour remplir la requete
      HttpURLConnection con = openConn(url, xKey);
      con.setRequestMethod("GET");

      //Lecture des donnees ligne par lignes
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

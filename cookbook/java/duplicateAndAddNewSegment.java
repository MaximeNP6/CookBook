import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.ArrayList;

import org.json.JSONException;
import org.json.JSONObject;

public class duplicateAndAddNewSegment
{
	public static JSONObject getTargetJson(String unicity) throws JSONException
	{
		//Syntaxe pour les differents types d'informations :
		String targetString = "name";	//Chaine de caracteres
		String tagetListOfValues = "Mr";	//Liste de valeurs
		String targetEmail = unicity;	//E-mail
		String tagetPhoneNumber = "0123456789";	//Telephone
		String targetTextZone = "150 caracters max";	//Zone de texte
		int targetNumbers = 123;	//Valeur numerique
		String targetDate = "01/01/2000";	//Date
		String largetListMultipleValues[] = {"valeur 1", "valeur 2"};	//Liste de valeurs multiples

		//Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur du champ"
		JSONObject targetJson = new JSONObject();
		targetJson.put("5398", targetString);
		targetJson.put("5399", tagetListOfValues);
		targetJson.put("5400", targetEmail);
		targetJson.put("5452", tagetPhoneNumber);
		targetJson.put("5453", targetTextZone);
		targetJson.put("5454", targetNumbers);
		targetJson.put("5455", targetDate);
		targetJson.put("5456", largetListMultipleValues);

		//On affiche le message
		System.out.print("New Json : " + targetJson + "\n");
		
		return (targetJson);
	}
	
	public static JSONObject getSegmentJson() throws JSONException
	{
		//Syntaxe pour les differents types d'informations :
		String segmentType = "static";	//Code pour creer un segment Statique
		String segmentName = "Nom du segment";	//Nom du segment
		String segmentDescription = "Description";	//Description du segment
		String segmentExpiration = "2016-01-08T12:11:00Z";	//Date d'expiration du segment
		Boolean segmentIsTest = true;	//Le test est un segment de test : oui = 'true' / non = 'false'

		//Creation du Json du message
		JSONObject segmentJson = new JSONObject();
		segmentJson.put("type", segmentType);
		segmentJson.put("name", segmentName);
		segmentJson.put("description", segmentDescription);
		segmentJson.put("expiration", segmentExpiration);
		segmentJson.put("isTest", segmentIsTest);

		//On affiche le message
		System.out.print("\nNew Json : " + segmentJson + "\n");
		
		return (segmentJson);
	}
	
	public static JSONObject getNewActionDuplicateJson(JSONObject actionDuplicateJson, String segmentId) throws JSONException
	{
		int[] segmentList = {Integer.parseInt(segmentId)};
		
		//Modification de la nouvelle action
		JSONObject segments = new JSONObject();
		segments.put("selected", segmentList);
		
		JSONObject scheduler = new JSONObject();
		scheduler.put("type", "asap");	//Envoie : immediat = 'asap' / Date = 'scheduled'
		//scheduler.put("type", "2015-07-27T08:15:00Z");	//Si type = 'scheduled' sinon a enlever
		scheduler.put("segments", segments);
		
		actionDuplicateJson.put("scheduler", scheduler);
		
		return (actionDuplicateJson);
	}
	
	public static JSONObject getActionValidation(String segmentId) throws JSONException
	{
		Object NULL = null;
		int[] segmentList = {Integer.parseInt(segmentId)};
		
		JSONObject actionValidationJson = new JSONObject();
		actionValidationJson.put("fortest", true);	//Phase de test
		actionValidationJson.put("campaignAnalyser", false);	//Campaign Analyzer : 'true' = oui / 'false' = non
		actionValidationJson.put("testSegments", segmentList);	//Les Ids des differents segments de tests
		actionValidationJson.put("mediaForTest", NULL);	//Rediriger tous les tests vers une seule adresse ('NULL' pour aucune valeur)
		actionValidationJson.put("textandHtml", false);	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
		actionValidationJson.put("comments", NULL);	//Commentaire ('NULL' pour aucuns commentaires)
		
		//On affiche le Json
		System.out.print(actionValidationJson + "\n");
		
		return (actionValidationJson);
	}
	
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DES CREATION DES JSONS//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DU PROGRAMME//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	
	public static void main(String[] args) throws Exception
	{
		//Url de base
		String urlBase = "http://v8.mailperformance.com/";

		//Ici, renseignez l'email de la cible, la X-Key
		String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		String unicity = "test@test.com";	//L'adresse mail de la cible a mettre dans le segment ("null" pour ne rien ajouter au segment)
		String segmentId = "0123";	//Id du segment a modifier ("null" si le segment est a creer)
		String actionId = "000ABC";	//Id de l'action a dupliquer
		
		//Ne pas remplir
		String targetId = null;
		ArrayList result = new ArrayList();
		
		
		//Lancement de la connexion pour remplir la requete
		String url = urlBase + "targets?unicity=" + unicity;
		HttpURLConnection con = null;
		
		con = urlGet(url, xKey);
		
		result = createTarget(con, unicity, xKey, urlBase);
		con = (HttpURLConnection) result.get(0);
		targetId = (String) result.get(1);
		
		result = createSegment(segmentId, urlBase, con, xKey);
		con = (HttpURLConnection) result.get(0);
		segmentId = (String) result.get(1);

		con = targetToSegment(urlBase, targetId, segmentId, con, xKey);
		
		result = duplicateAction(urlBase, actionId, con, xKey);
		con = (HttpURLConnection) result.get(0);
		String actionDuplicateId = (String) result.get(1);
		JSONObject actionDuplicateJson = (JSONObject) result.get(2);
		
		con = updateActionDuplicate(urlBase, actionDuplicateId, con, actionDuplicateJson, segmentId, xKey);
		
		con = testActionDuplicate(urlBase, actionDuplicateId, con, xKey, segmentId);
		
		con.disconnect();
		System.exit(1);
	}
	
	
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DU PROGRAMME////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DES FONCTIONS/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	
	public static ArrayList createTarget(HttpURLConnection con, String unicity, String xKey, String urlBase) throws IOException, JSONException
	{
		String url = urlBase + "targets/";
		
		//Verification des reponses
		int responseCode = con.getResponseCode();
		if (responseCode != 200)
		{
			if (responseCode != 404)
			{
				//Affichage de l'erreur
				System.out.print("Error : " + responseCode + ' ' + con.getResponseMessage());
				System.exit(1);
			}
			else
			{
				System.out.print("Error : " + responseCode + " : Creation of the target.\n");
				con = urlPostOrPut(con, "POST", getTargetJson(unicity), xKey, url);
			}
		}
		else
		{
			//Lecture des donnees
			BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
			String reply = buffRead.readLine();
			buffRead.close();
			System.out.print(reply + "\n");
			
			con = urlPostOrPut(con, "PUT", getTargetJson(unicity), xKey, url);
		}
		
		//Lecture des donnees
		BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
		String reply = buffRead.readLine();
		buffRead.close();
		System.out.print(reply + "\nData base changed.\n\n");
		
		//On recupere l'id de l'action
		JSONObject jObject  = new JSONObject(reply);
		String targetId = jObject.getString("id");
		
		ArrayList result = new ArrayList();
		result.add(con);
		result.add(targetId);
		
		return (result);
	}
	
	public static ArrayList createSegment(String segmentId, String urlBase, HttpURLConnection con, String xKey) throws MalformedURLException, IOException, JSONException
	{
		//Lancement de la connexion
		if (segmentId != null)
		{
			String url =  urlBase + "segments/" + segmentId;
			con = urlPostOrPut(con, "PUT", getSegmentJson(), xKey, url);
		}
		else
		{
			String url =  urlBase + "segments/";
			con = urlPostOrPut(con, "POST", getSegmentJson(), xKey, url);
		}
		
		//Verification des reponses
		int responseCode = con.getResponseCode();
		if (responseCode != 200)
		{
			//Affichage de l'erreur
			System.out.print("Error : " + responseCode + " " + con.getResponseMessage());
			System.exit(1);
		}
		
		//Lecture des donnees ligne par lignes
		BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
		String reply = buffRead.readLine();
		con.disconnect();
		buffRead.close();
	
		//On recupere l'id de l'action
		JSONObject jObject  = new JSONObject(reply);
		String segmentName = jObject.getString("name");
		segmentId = Integer.toString(jObject.getInt("id"));
		
		
		//Le segment a bien ete cree
		System.out.print("The segment static : " + segmentName + " is created / update.\n\n");
		
		ArrayList result = new ArrayList();
		result.add(con);
		result.add(segmentId);
		
		return (result);
	}
	
	public static HttpURLConnection targetToSegment(String urlBase, String targetId, String segmentId, HttpURLConnection con, String xKey) throws MalformedURLException, IOException
	{
		//Nouvelle url
		String url = urlBase + "targets/" + targetId + "/segments/" + segmentId;
		
		//Connexion
		con = urlPostOrPut(con, "POST", new JSONObject(), xKey, url);
		
		System.out.print("The target (" + targetId + ") is add to the segment (" + segmentId + ").\n\n");
		
		return (con);
	}
	
	public static ArrayList duplicateAction(String urlBase, String actionId, HttpURLConnection con, String xKey) throws MalformedURLException, IOException, JSONException
	{
		//Nouvelle url
		String url = urlBase + "actions/" + actionId + "/duplication";
		
		//On duplique a l'identique
		con = urlPostOrPut(con, "POST", new JSONObject(), xKey, url);
						
		//Lecture des donnees
		BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
		String reply = buffRead.readLine();
		con.disconnect();
		buffRead.close();
		
		//On affiche la cible
		System.out.print("The action duplicate ( " + new JSONObject(reply).getString("name") + " ) is created :\n");
		System.out.print(reply + "\n\n");
		
		//On recupere l'id de la nouvelle action
		JSONObject actionDuplicateJson  = new JSONObject(reply);
		String actionDuplicateId = actionDuplicateJson.getString("id");
		
		ArrayList result = new ArrayList();
		result.add(con);
		result.add(actionDuplicateId);
		result.add(actionDuplicateJson);
		
		return (result);
		
	}
	
	public static HttpURLConnection updateActionDuplicate(String urlBase, String actionDuplicateId, HttpURLConnection con, JSONObject actionDuplicateJson, String segmentId, String xKey) throws MalformedURLException, IOException, JSONException
	{
		//Nouvelle url
		String url = urlBase + "actions/" + actionDuplicateId;
			
		//On duplique a l'identique
		con = urlPostOrPut(con, "PUT", getNewActionDuplicateJson(actionDuplicateJson, segmentId), xKey, url);

		//Lecture des donnees
		BufferedReader buffRead = new BufferedReader(new InputStreamReader(con.getInputStream()));
		String reply = buffRead.readLine();
		con.disconnect();
		buffRead.close();
		
		System.out.print("The action duplicate ( " + new JSONObject(reply).getString("name") + " ) is update:\n");
		System.out.print(reply + "\n\n");
		
		return (con);
	}
	
	public static HttpURLConnection testActionDuplicate(String urlBase, String actionDuplicateId, HttpURLConnection con, String xKey, String segmentId) throws MalformedURLException, InterruptedException, IOException, JSONException
	{
		//Nouvelle url
		String url = urlBase + "actions/" + actionDuplicateId + "/validation";
		
		//Lancement de la connexion
		con = urlPostOrPut(con, "POST", getActionValidation(segmentId), xKey, url);
		
		int actionState = waitForState(actionDuplicateId, xKey, urlBase);
		
		if (actionState == 20)
		{
			//Affichage de l'erreur
			System.out.print("Error : the test failed.");
		}
		else
		{
			//La phase de test a reussi
			System.out.print("The action is tested.\n\n");
		}
		return (con);
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
	
	public static HttpURLConnection urlPostOrPut(HttpURLConnection con, String request, JSONObject dataJson, String xKey, String url) throws MalformedURLException, IOException
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
	
	//Fonction d'attente de fin de test
	public static int waitForState(String idAction, String xKey, String urlBase) throws InterruptedException, MalformedURLException, IOException, JSONException
	{
		int actionState = 30;
		
		while (actionState != 38 && actionState != 20)
		{
			//On attend 20 secondes
			System.out.print("Wait 20sec...\n");
			Thread.sleep(20000);
			
			//Nouvelle adresse
			String url = urlBase + "actions/" + idAction;
			
			//Lancement de la connexion pour remplir la requete
			HttpURLConnection con = urlGet(url, xKey);
			
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

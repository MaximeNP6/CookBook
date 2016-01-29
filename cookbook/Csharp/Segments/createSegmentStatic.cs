using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace createSegmentStatic
{
	class Program
	{
		static void Main(string[] args)
		{
			//Ici, renseignez la xKey et les parametres personnalises
			String xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			String segmentId = "0123";	//Id du segment a modifier ('null' si le segment doit etre cree)
			String unicity = "test@test.com"; //L'adresse mail de la cible a mettre dans le segment ('null' pour ne rien ajouter au segment)

			String type = "static";	//Code pour creer un segment Statique
			String name = "SegmentStatic (Csharp)";	//Nom du segment
			String description = "SegmentStatic (Csharp)";	//Description du segment
			String expiration = "2016-01-08T12:11:00Z";	//Date d'expiration du segment
			Boolean isTest = true;	//Le test est un segment de test : oui = 'true' / non = 'false'


			//On trouve l'adresse pour la requete
			String url = "http://v8.mailperformance.com/segments/";
			if (segmentId != null)
			{
				url = "http://v8.mailperformance.com/segments/" + segmentId;
			}

			//Creation du Json du message
			JObject jsonData = new JObject();
			jsonData.Add("type", type);
			jsonData.Add("name", name);
			jsonData.Add("description", description);
			jsonData.Add("expiration", expiration);
			jsonData.Add("isTest", isTest);

			//On affiche le message
			Console.Write(jsonData + "\n");


			//Lancement de la connexion pour remplir la requete
			Object[] connection = null;

			if (segmentId != null)
			{
				connection = allConnection(url, xKey, jsonData, "PUT");
			}
			else
			{
				connection = allConnection(url, xKey, jsonData, "POST");
			}

			int response = (int)connection[0];
			HttpWebRequest con = (HttpWebRequest)connection[1];
			HttpWebResponse httpResponse = (HttpWebResponse)connection[2];
			String responseString = (String)connection[3];

			//Verification des reponses
			if (response != 200)
			{
				//Affichage de l'erreur
				Console.Write("\nError : " + response);
			}
			else
			{
				//Le segment a bien ete cree
				Console.Write("The segment static  : " + name + " is created / update.\n\n");

				if (unicity != null && segmentId != null)
				{
					//Nouvelle url
					url = "http://v8.mailperformance.com/targets?unicity=" + unicity;

					//Connexion
					connection = allConnection(url, xKey, null, "GET");

					response = (int)connection[0];
					con = (HttpWebRequest)connection[1];
					httpResponse = (HttpWebResponse)connection[2];
					responseString = (String)connection[3];

					//Verification des reponses
					if (response != 200)
					{
						//Affichage de l'erreur
						Console.Write("\nError : " + response);
					}
					else
					{
						//On prend le idTarget
						dynamic responseJson = JsonConvert.DeserializeObject(responseString);
						string idTarget = responseJson.id;

						//Nouvelle url
						url = "http://v8.mailperformance.com/targets/" + idTarget + "/segments/" + segmentId;

						//Connexion
						connection = allConnection(url, xKey, null, "POST");

						response = (int)connection[0];
						con = (HttpWebRequest)connection[1];
						httpResponse = (HttpWebResponse)connection[2];
						responseString = (String)connection[3];

						//Verification des reponses
						if (response != 200)
						{
							//Affichage de l'erreur
							Console.Write("\nError : " + response);
						}
						else
						{
							Console.Write("The target : " + unicity + " is add to the segment : " + name);
						}
					}
				}
			}
			//Attente de lecture (optionnel)
			Console.ReadLine();

			httpResponse.Close();
		}



		//Fonctions ----



		//Fonction de connexion
		static HttpWebRequest Connect(string url, string xKey, string method)
		{
			//Lancement de la connexion pour remplir la requete
			HttpWebRequest con = (HttpWebRequest)WebRequest.Create(url);
			con.Method = method;

			//Mise en place du xKey et des options
			con.Headers.Add("X-Key", xKey);
			con.ContentType = "application/json";

			return (con);
		}

		//Fonction de connexion et envoie des informations
		static Object[] allConnection(String url, String xKey, JObject jsonMessage, String method)
		{
			HttpWebRequest con = Connect(url, xKey, method);
			con.ContentLength = 0;
			if (jsonMessage != null)
			{
				con.ContentLength = jsonMessage.ToString().Length;
				var streamWriter = new StreamWriter(con.GetRequestStream());
				streamWriter.Write(jsonMessage);
				streamWriter.Flush();
				streamWriter.Close();
			}

			//Test de l'envoie
			HttpWebResponse httpResponse = null;
			int response = 0;
			string responseString = null;
			try
			{
				httpResponse = (HttpWebResponse)con.GetResponse();
				response = 200;

				//Lecture des donnees
				StreamReader reader = new StreamReader(con.GetResponse().GetResponseStream());
				responseString = reader.ReadToEnd();
				httpResponse.Close();
				reader.Close();
			}
			//Reception du signal 
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					httpResponse = (HttpWebResponse)ex.Response;
					response = (int)httpResponse.StatusCode;
				}
			}
			Object[] result = { response, con, httpResponse, responseString };
			return (result);
		}
	}
}

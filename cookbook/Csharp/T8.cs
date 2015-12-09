using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace T8
{
    class Program
    {
        static void Main(string[] args)
        {
            //Texte dans lequel il y a les liens a transformer
            String text = "This is the exemple : first link : https://www.google.com / second link : https://www.youtube.com/";

            //Fonction qui copie le texte avec des liens T8
            Regex rgx = new Regex("\\b(https?|ftp|file)://[-a-zA-Z0-9+&@#/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#/%=~_|]");
            string result = rgx.Replace(text, createT8);

            result = addOpeningLink(result);

            //Attente de lecture (optionnel)
            Console.Write(result);
            Console.ReadLine();
        }


        //Fonction qui cree les liens T8
        static String createT8(Match url)
        {
            //La clef de hachage md5 (peut etre change)
            String keyMd5 = "ABCD";

            //les differentes variables
            String urlT8 = "http://t8.mailperformance.com/";	//Adresse du catcher
            String redirectUrl = "redirectUrl";	//Nom de l'api de redirection
            String GV1 = findGV1(1);	//Identifie la demande ( utilisation de la fonction "findGV1()" )
            String linkId = "nameOfTheLink";	//Nom du lien
            String targetUrl = WebUtility.UrlEncode(url.Value);	//L'url de redirection souhaitee
            String h = findH(keyMd5, url.Value);	//Valeur de hachage base sur l'url de redirection et un code specifique au client ( utilisation de la fonction "findH()" )

            //Creation du lien avec toutes les valeurs
            String finalUrl = urlT8 + redirectUrl + "?GV1=" + GV1 + "&linkId=" + linkId + "&targetUrl=" + targetUrl + "&h=" + h;

            //Retourne le nouveau lien
            return (finalUrl);
        }

        //On ajoute le lien pour tracker les ouvertures
        static String addOpeningLink(string result)
        {
            String endHtml = "</body></html>";  //EndHtml
            String urlT8 = "http://t8.mailperformance.com/";	//Adresse du catcher

            String openLink = "<img src=\"" + urlT8 + "o5.aspx?GV1=" + findGV1(2) + "\">"; // Open Link

            //Position de la derniere occurence connu
            int position = result.LastIndexOf(endHtml);

            if (position == -1) //if failed
                result = result + openLink;
            else
                result = result.Replace(endHtml, openLink + endHtml);

            return (result);
        }

        //Fonction pour trouver le GV1
        static String findGV1(int option)
        {
            String GV1 = null;

            String agenceId = "ABCD";	//Id de l'agence
            String customerId = "0AB";	//Id du compte client
            String actionId = "000ABC";	//Id de l'action
            String targetId = "000ABCDE";	//Id de la cible

            //Creation du GV1
            if (option == 1) //Ouverture de liens
                GV1 = agenceId + customerId + actionId + targetId + '0';
            else if (option == 2) //Ouverture de l'email
                GV1 = agenceId + customerId + "00000" + actionId + targetId;
            else //Erreur
                GV1 = null;
            return (GV1);
        }

        //Fonction qui cree la valeur de hachage
        static String findH(String keyMd5, String url)
        {
            //Calcul du MD5 hash depuis l'input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(keyMd5 + url);
            byte[] hash = md5.ComputeHash(inputBytes);

            //Converti les bits en String
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            //Envoi des donnees
            String h = sb.ToString();
            return (h);
        }
    }
}

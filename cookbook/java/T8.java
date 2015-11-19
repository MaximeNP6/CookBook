import java.io.UnsupportedEncodingException;
import java.math.BigInteger;
import java.net.URLEncoder;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class T8
{
    public static void main(String[] args) throws UnsupportedEncodingException, NoSuchAlgorithmException
    {
        //Texte dans lequel il y a les liens a transformer
        String text = "This is the exemple : first link : https://www.google.com / second link : https://www.youtube.com/";

        //Fonction qui copie le texte avec des liens T8
        Pattern pattern = Pattern.compile("\\b(https?|ftp|file)://[-a-zA-Z0-9+&@#/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#/%=~_|]");
        Matcher matcher = pattern.matcher(text);
        StringBuffer sb = new StringBuffer();
        while (matcher.find())
        {
            matcher.appendReplacement(sb, createT8(matcher.group()));
        }
        matcher.appendTail(sb);

        //On affiche le nouveau texte
        String newText = sb.toString();
        System.out.print(newText);
    }

    //Fonction qui cree les liens T8
    public static String createT8(String url) throws UnsupportedEncodingException, NoSuchAlgorithmException
    {
        //La clef de hachage md5 (peut etre change)
        String keyMd5 = "ABCD";

        //les differentes variables
        String urlT8 = "http://t8.mailperformance.com/";	//adresse du catcher
        String redirectUrl = "redirectUrl";	//nom de l'api de redirection
        String GV1 = findGV1();	//identifie la demande ( utilisation de la fonction "findGV1()" )
        String linkId = "nameOfTheLink";	//Nom du lien
        String targetUrl = URLEncoder.encode(url,"UTF-8");	//l'url de redirection souhaitee
        String h = findH(keyMd5, url);	//valeur de hachage base sur l'url de redirection et un code specifique au client ( utilisation de la fonction "findH()" )

        //Creation du lien avec toutes les valeurs
        String finalUrl = urlT8 + redirectUrl + "?GV1=" + GV1 + "&linkId=" +  linkId + "&targetUrl=" + targetUrl + "&h=" + h;

        return (finalUrl);
    }

    //Fonction pour trouver le GV1
    public static String findGV1()
    {
        String agenceId = "ABCD";	//Id de l'agence
        String customerId = "0AB";	//Id du compte client
        String actionId = "000ABC";	//Id de l'action
        String targetId = "000ABCDE";	//Id de la cible

        //Creation du GV1
        String GV1 = agenceId + customerId + actionId + targetId + '0';
        return (GV1);
    }

    //Fonction qui cree la valeur de hachage
    public static String findH(String keyMd5, String url) throws UnsupportedEncodingException, NoSuchAlgorithmException
    {
        String input = keyMd5 + url;
        String h = new String();

        //Creation de la valeur avec l'algorithme md5
        MessageDigest md = MessageDigest.getInstance("MD5");
        md.update(input.getBytes());
        BigInteger hash = new BigInteger(1, md.digest());
        h = hash.toString(16);
        while(h.length() < 32)
        {
            h = "0" + h;
        }

        return (h);
    }
}

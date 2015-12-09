<?php

//Texte dans lequel il y a les liens a transformer
$text = "This is the exemple : first link : https://www.google.com / second link : https://www.youtube.com/";

//Fonction qui copie le texte avec des liens T8
$newText = preg_replace('#(?:https?|ftp)://(?:[\w%?=,:;+\#@./-]|&amp;)+#e', "createT8('$0')", $text);
$newText = addOpeningLink($newText);

echo $newText;

//Fonction qui cree les liens T8
function createT8($url)
{
    //La clef de hachage md5 (peut etre change)
    $keyMd5 = 'ABCD';

    //les differentes variables
    $urlT8 = 'http://t8.mailperformance.com/';	//adresse du catcher
    $redirectUrl = 'redirectUrl';	//nom de l'api de redirection
    $GV1 = findGV1(1);	//identifie la demande ( utilisation de la fonction "findGV1()" )
    $linkId = 'nameOfTheLink';	//Nom du lien
    $targetUrl = $url;	//l'url de redirection souhaitee
    $h = findH($keyMd5, $url);	//valeur de hachage base sur l'url de redirection et un code specifique au client ( utilisation de la fonction "findH()" )

    //Creation du lien avec toutes les valeurs
    $data = array('GV1' => $GV1, 'linkId' => $linkId, 'targetUrl' => $targetUrl, 'h' => $h);
    $FinalUrl = $urlT8 . $redirectUrl . '?' . http_build_query($data);

    //Retourne le nouveau lien
    return ($FinalUrl);
}

//On ajoute le lien pour tracker les ouvertures
function addOpeningLink($text)
{
    $endHtml = "</body></html>";	//EndHtml
    $urlT8 = 'http://t8.mailperformance.com/';	//adresse du catcher

    $openLink = '<img src="' . $urlT8 . 'o5.aspx?GV1=' . findGV1(2) . '">';	// Open Link

    //Position de la derniere occurence connu
    $pos = strrpos($text, $endHtml);

    if($pos !== false)
        $newText = substr_replace($text, $openLink . $endHtml, $pos, strlen($endHtml));
    else	//if failed
        $newText = $text . $openLink;

    return ($newText);
}

//Fonction pour trouver le GV1
function findGV1($option)
{
    $agenceId = 'ABCD';	//Id de l'agence
    $customerId = '0AB';	//Id du compte client
    $actionId = '000ABC';	//Id de l'action
    $targetId = '000ABCDE';	//Id de la cible

    //Creation du GV1
    if ($option == 1) //Ouverture de liens
        $GV1 = $agenceId . $customerId . $actionId . $targetId . '0';
    else if ($option == 2) //Ouverture de l'email
        $GV1 = $agenceId . $customerId . '00000' . $actionId . $targetId;
    return ($GV1);
}

//Fonction qui cree la valeur de hachage
function findH($keyMd5, $url)
{
    //Creation de la valeur avec l'algorithme md5
    $h = md5($keyMd5 . urldecode($url));
    return ($h);
}

?>

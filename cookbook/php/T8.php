<?php

//Texte dans lequel il y a les liens a transformer
$text = "This is the exemple : first link : https://www.google.com / second link : https://www.youtube.com/";

//Fonction qui copie le texte avec des liens T8
$newText = preg_replace('#(?:https?|ftp)://(?:[\w%?=,:;+\#@./-]|&amp;)+#e', "createT8('$0')", $text);
echo $newText;

//Fonction qui cree les liens T8
function createT8($url)
{
	//La clef du code md5 (peut etre changer)
	$keyMd5 = '012345678';
	
	//les differentes variables
	$urlT8 = 'http://t8.mailperformance.com/';	//adresse du catcher
	$redirectUrl = 'redirectUrl';	//nom de l'api de redirection
	$GV1 = findGV1();		//identifie la demande ( utilisation de la fonction "findGV1()" )
	$linkId = 'nameOfTheLink';		//Nom du lien
	$targetUrl = $url;	//l'url de redirection souhaitee
	$h = findH($keyMd5, $url);		//valeur de hachage base sur l'url de redirection et un code specifique au client ( utilisation de la fonction "findH()" )
	
	//Creation du lien avec toutes les valeurs
	$data = array('GV1' => $GV1, 'linkId' => $linkId, 'targetUrl' => $targetUrl, 'h' => $h);
	$FinalUrl = $urlT8 . $redirectUrl . '?' . http_build_query($data);
	
	//Retourne le nouveau lien
	return ($FinalUrl);
}

//Fonction pour trouver le GV1
function findGV1()
{
	$agenceId = 'ABCD';	//Id de l'agence
	$clientId = '0AB';	//Id du client
	$actionId = '000ABC';		//Id de l'action
	$targetId = '000ABCDE';	//Id de la cible
	
	//Creation du GV1
	$GV1 = $agenceId . $clientId . $actionId . $targetId . '0';
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
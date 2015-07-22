<?php

//Ici, renseignez l'email de la cible, la xKey, l'id du message et les parametres personnalises
$unicity = 'test@test.com';
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
$idMessage = '000ABC';

$htmlMessage = 'html message';
$textMessage = 'text message';
$subject = 'subject of the message';
$mailFrom = 'mail@address.com';
$replyTo = 'mail@return.com';


//On trouve l'addresse pour la requete
$url = 'http://v8.mailperformance.com/targets?unicity='. $unicity;

//On remplit la requete
$req = startCurlInit($url);
curl_setopt($req,CURLOPT_CUSTOMREQUEST,'GET');

//Mise en place du xKey et des options
curl_setopt($req, CURLOPT_HTTPHEADER, array(
'X-Key: ' . $xKey,
'Content-Type: application/json'));

//Execution de la requete
$result = curl_exec($req);

//Verification des reponses
if ($result == false)
{
	//Affichage de l'erreur
	$info = curl_getinfo($req);
	echo 'Error : ' . $info['http_code'];
}
else
{
	//On recupere l'id de la cible
	$tab = json_decode($result, TRUE);
	$idTarget = $tab['id'];

	//On affiche la cible
	echo $result . "\n";

	//Nouvelle url en fonction de l'id du message et de la cible
	$url = 'http://v8.mailperformance.com/actions/' . $idMessage . '/targets/' . $idTarget;
	
	//Creation du Json du message
	$arr = array('content' => array('html' => $htmlMessage, 'text' => $textMessage),
	'header' => array('subject' => $subject, 'mailFrom' => $mailFrom, 'replyTo' => $replyTo));
	$message = json_encode($arr);

	//On remplit la requete
	$req = startCurlInit($url);
	curl_setopt($req, CURLOPT_CUSTOMREQUEST, 'POST');

	//Mise en place du xKey et des options
	curl_setopt($req, CURLOPT_HTTPHEADER, array(
	'X-Key: ' . $xKey,
	'Content-Type: application/json',
	'Content-Length: ' . strlen($message)));
	curl_setopt($req, CURLOPT_POSTFIELDS, $message);

	//Execution de la requete
	$result = curl_exec($req);

	//Verification des reponses
	$info = curl_getinfo($req);
	if ($info['http_code'] != 204)
	{
		echo 'Error : ' . $info['http_code'];
	}
	else
	{
		echo 'Message sent to ' . $unicity;
	}
}

curl_close($req);



//Fonctions -----



//Utilisation de cURL pour remplir les requetes
function startCurlInit($url)
{
	$init = curl_init();
	curl_setopt($init,CURLOPT_URL, $url);
	curl_setopt($init, CURLOPT_RETURNTRANSFER, true);
	return ($init);
}

?>
<?php

//Ici, renseignez l'email dont vous voulez obtenir les valeurs des champs, et l'id du message
$unicity = 'test@test.com';
$idMessage = '000ABC';

//On trouve l'addresse pour la requete
$url = 'http://v8.mailperformance.com/targets?unicity='. $unicity;

//Utilisation de cURL pour remplir les requetes
function startCurlInit($url)
{
	$init = curl_init();
	curl_setopt($init,CURLOPT_URL, $url);
	curl_setopt($init, CURLOPT_RETURNTRANSFER, true);
	return ($init);
}

//On remplit la requete
$req = startCurlInit($url);
curl_setopt($req,CURLOPT_CUSTOMREQUEST,'GET');

//Mise en place du xKey et des options
curl_setopt($req, CURLOPT_HTTPHEADER, array(
'X-Key: ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789',
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
	$targetId = $tab['id'];
	
	//Nouvelle url en fonction de l'id du message et de la cible
	$url = 'http://v8.mailperformance.com/actions/' . $idMessage . '/targets/' . $targetId;

	//On remplit la requete
	$req = startCurlInit($url);
	curl_setopt($req, CURLOPT_POST, true);
	
	//Mise en place du xKey et des options
	curl_setopt($req, CURLOPT_HTTPHEADER, array(
	'X-Key: ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789',
	'Content-Type: application/json',
	'Content-Length: 0'));

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

?>
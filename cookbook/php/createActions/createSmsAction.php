<?php

//Ici, renseignez la xKey
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';

$type = 'smsMessage';	//Code pour envoyer un SMS
$name = 'SMSFromApi (php)';	//Nom de l'action
$description = 'SMSFromApi (php)';	//Description de l'action

$informationFolder = 0123;	//Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier)
$informationCategory = 0123;	//Id de la categorie de campagne (Infos compte > Parametrage > Categories de campagnes)

$textContent = 'Text message / max 160';	//Message texte


//On trouve l'addresse pour la requete
$url = 'http://v8.mailperformance.com/actions';

//Creation du Json du message
$arr = array(
	'type' => $type,
	'name' => $name,
	'description' => $description,
	'informations' => array(
		'folder' => $informationFolder,
		'category' => $informationCategory),
	'content' => array(
		'textContent' => $textContent));
		
		
//On affiche le message
$message = json_encode($arr);
echo $message . "\n";

//Connexion
$con = connect($url, $xKey, $message);

$result = $con['result'];
$info = $con['info'];
$req = $con['req'];

//Verification des reponses
if ($info['http_code'] != 200)
{
	echo 'Error : ' . $info['http_code'];
}
else
{
	//L'action a bien ete cree
	echo 'Action : ' . $name . " created.\n\n";
	echo 'Don\'t forget to normalize your phone numbers with your country.';
}
curl_close($req);



//Fonctions -----



//Utilisation de cURL pour remplir les requetes
function startCurlInit($url)
{
	$init = curl_init();
	curl_setopt($init, CURLOPT_URL, $url);
	curl_setopt($init, CURLOPT_RETURNTRANSFER, true);
	return ($init);
}

//Fonction de connexion
function connect($url, $xKey, $message)
{
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
	
	return (array('result' => $result, 'info' => $info, 'req' => $req));
}

?>
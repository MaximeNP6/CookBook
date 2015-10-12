<?php

//Ici, renseignez l'email de la cible, la X-Key et l'id du segment
$unicity = 'test@test.com';
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
$idSegment = '7503';

//Syntaxe pour les differents types d'informations :
$string = "name";	//Chaine de caracteres
$listOfValues = "Mr";	//Liste de valeurs
$email = "test@test.com";	// E-mail
$phoneNumber = "0123456789";	// Telephone
$textZone = "150 caracters max";	//Zone de texte
$numbers = 123;	//Valeur numerique
$date = "01/01/2000";	//Date
$listMultipleValues = array("valeur 1", "valeur 2");	//Liste de valeurs multiples

//Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur du champ"
$data = array("5398" => $string,
		"5399" => $listOfValues,
		"5400" => $email,
		"5452" => $phoneNumber,
		"5453" => $textZone,
		"5454" => $numbers,
		"5455" => $date,
		"5456" => $listMultipleValues);
$dataJson = json_encode($data);

//On trouve l'adresse pour la requete
$url = 'http://v8.mailperformance.com/targets?unicity=' . $unicity;

//On remplit la requete 'GET'
$req = startCurlInit($url);
curl_setopt($req, CURLOPT_CUSTOMREQUEST, 'GET');

//Mise en place du xKey et des options
curl_setopt($req, CURLOPT_HTTPHEADER, array(
'X-Key: ' . $xKey,
'Content-Type: application/json'));

//Execution de la requete
$result = curl_exec($req);

$resultDisplay = 0;

//Verification des reponses
if ($result == false)
{
	//La cible n'existe pas, nous devons la creer
	
	//Affichage de l'erreur
	$info = curl_getinfo($req);
	echo 'Error : ' . $info['http_code'] . " : Creation of the target.\n";
	
	//nouvelle url
	$url = 'http://v8.mailperformance.com/targets/';
	
	//On remplit la requete 'POST' et on cree la cible
	$result = request($req, 'POST', $dataJson, $xKey, $url);
	echo $result . "\n";
	$resultDisplay = 1;
}

//La cible existe
curl_close($req);

//On recupere l'id de la cible
$tab = json_decode($result, TRUE);
$idTarget = $tab['id'];

//On affiche les anciennes valeurs
if ($resultDisplay != 1)
	echo $result . "\n";

//Nouvelle url
$url = 'http://v8.mailperformance.com/targets/' . $idTarget . '/segments/' . $idSegment;

//On remplit la requete 'POST'
$result = request($req, 'POST', null, $xKey, $url);
if ($result != 'error')
{
	echo "The target " . $unicity . " is added to the segment " . $idSegment . ".";
}



//Fonctions



//Utilisation de cURL pour remplir les requetes
function startCurlInit($url)
{
	$init = curl_init();
	curl_setopt($init, CURLOPT_URL, $url);
	curl_setopt($init, CURLOPT_RETURNTRANSFER, true);
	return ($init);
}

function request($req, $request, $dataJson, $xKey, $url)
{
	//On remplit la requete avec le bon verbe ($request) : GET / POST / PUT
	$req = startCurlInit($url);
	curl_setopt($req, CURLOPT_CUSTOMREQUEST, $request);
	if (strlen($dataJson) != 0)
		curl_setopt($req, CURLOPT_POSTFIELDS, $dataJson);
	
	//Mise en place du xKey et des options
	curl_setopt($req, CURLOPT_HTTPHEADER, array(
	'X-Key: ' . $xKey,
	'Content-Type: application/json',
	'Content-Length: ' . strlen($dataJson)));

	//Execution de la requete
	$result = curl_exec($req);

	//Verification des reponses
	$info = curl_getinfo($req);
	curl_close($req);
	if ($info['http_code'] != 200 && $info['http_code'] != 204)
	{
		echo "Error : " . $info['http_code'] . "\n";
		return ('error');
	}
	return ($result);
}

?>
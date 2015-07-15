<?php

//Ici, renseignez l'email dont vous voulez obtenir les valeurs des champs, la X-Key et les informations à changer dans le tableau
$unicity = 'test@test.com';
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';

//Syntaxe pour les différents types d'informations :
$string = "name";	//Chaine de caracteres
$listOfValues = "Mr";	//Liste de valeurs
$email = "test@test.com";	// E-mail
$phoneNumber = "0123456789";	// Telephone
$textZone = "150 caracters max";	//Zone de texte
$numbers = 123;	//Valeur numérique
$date = "01/01/2000";	//Date
$listMultipleValues = array("valeur 1", "valeur 2");	//Liste de valeurs multiples

//Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur de l'information"
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
$url = 'http://backoffice.mailperformance.dev/targets?unicity=' . $unicity;

//On remplit la requete 'GET'
$req = startCurlInit($url);
curl_setopt($req, CURLOPT_CUSTOMREQUEST, 'GET');

//Mise en place du xKey et des options
curl_setopt($req, CURLOPT_HTTPHEADER, array(
'X-Key: ' . $xKey,
'Content-Type: application/json'));

//Execution de la requete
$result = curl_exec($req);


//Verification des reponses
if ($result == false)
{
	//La cible n'existe pas, nous devons la creer
	
	//Affichage de l'erreur
	$info = curl_getinfo($req);
	echo 'Error : ' . $info['http_code'] . " : Creation of the target.\n";

	//On remplit la requete 'POST'
	$req = postOrPutOnTarget($req, 'POST', $dataJson, $xKey);
}
else
{
	//La cible existe, nous la mettons a jour
	
	//On affiche les anciennes valeurs
	echo $result . "\n";
	
	//On remplit la requete 'PUT'
	$req = postOrPutOnTarget($req, 'PUT', $dataJson, $xKey);
}
curl_close($req);



//Utilisation de cURL pour remplir les requetes
function startCurlInit($url)
{
	$init = curl_init();
	curl_setopt($init, CURLOPT_URL, $url);
	curl_setopt($init, CURLOPT_RETURNTRANSFER, true);
	return ($init);
}

function postOrPutOnTarget($req, $request, $dataJson, $xKey)
{
	//Nouvelle url
	$url = 'http://backoffice.mailperformance.dev/targets/';

	//On remplit la requete avec le bon verbe ($request) : GET / PUSH / PUT
	$req = startCurlInit($url);
	curl_setopt($req, CURLOPT_CUSTOMREQUEST, $request);
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
	if ($info['http_code'] != 200)
		echo 'Error : ' . $info['http_code'];
	else
		echo $result . "\nData base changed.";
	return ($req);
}

?>
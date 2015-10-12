<?php

//Ici, renseignez la xKey et les parametres personnalises
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
$categorieId = '0123';	//Id de la categorie a modifier ('null' si la categorie doit etre cree)

$name = 'Category php';	//Nom de la categorie
$description = 'Category (php)';	//Description de la categorie

//On trouve l'adresse pour la requete
$url = 'http://v8.mailperformance.com/categories/' . $categorieId;

//Creation du Json du message
$arr = array(
	'name' => $name,
	'description' => $description);


//On affiche le message
$message = json_encode($arr);
echo $message . "\n";

//Connexion
if ($categorieId != null)
	$con = connect($url, $xKey, $message, 'PUT');
else
	$con = connect($url, $xKey, $message, 'POST');
	
$result = $con['result'];
$info = $con['info'];
$req = $con['req'];

if ($info['http_code'] != 200)
{
	echo 'Error : ' . $info['http_code'];
}
else
{
	//La categorie a bien ete cree
	echo "\nThe category  : " . $name . " is created / update.";
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
function connect($url, $xKey, $message, $method)
{
	//On remplit la requete
	$req = startCurlInit($url);
	curl_setopt($req, CURLOPT_CUSTOMREQUEST, $method);
		
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
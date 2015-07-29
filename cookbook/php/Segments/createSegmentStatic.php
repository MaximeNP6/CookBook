<?php

//Ici, renseignez la xKey et les parametres personnalises du message
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
$segmentId = 0123;	//Id du segment a modifier ('null' si le segment est a creer)
$unicity = 'test@test.com'; //L'adresse mail de la cible a mettre dans le segment ('null' pour ne rien ajouter au segment)

$type = 'static';	//Code pour creer un segment Statique
$name = 'SegmentStatic (php)';	//Nom du segment
$description = 'SegmentStatic (php)';	//Description du segment
$expiration = '2016-01-08T12:11:00Z';	//Date d'expiration du segment
$isTest = true;	//Segment de test : oui = 'true' / non = 'false'

//On trouve l'adresse pour la requete
$url = 'http://v8.mailperformance.com/segments/' . $segmentId;


//Creation du Json du message
$arr = array(
	'type' => $type,
	'name' => $name,
	'description' => $description,
	'expiration' => $expiration,
	'isTest' => $isTest);
		

//On affiche le message
$message = json_encode($arr);
echo $message . "\n";

//Connexion
if ($segmentId != null)
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
	//Le segment a bien ete cree
	echo 'The segment static  : ' . $name . " is created / update.\n\n";
	
	if ($unicity != null && $segmentId != null)
	{
		//Nouvelle url
		$url = 'http://v8.mailperformance.com/targets?unicity=' . $unicity;
	
		//Connexion
		$con = connect($url, $xKey, null, 'GET');
	
		$result = $con['result'];
		$info = $con['info'];
		$req = $con['req'];
	
	
		if ($info['http_code'] != 200)
		{
			echo 'Error : ' . $info['http_code'];
		}
		else
		{
			$tab = json_decode($result, TRUE);
			$idTarget = $tab['id'];
	
			//Nouvelle url
			$url = 'http://v8.mailperformance.com/targets/' . $idTarget . '/segments/' . $segmentId;
	
			//Connexion
			$con = connect($url, $xKey, null, 'POST');
		
			$result = $con['result'];
			$info = $con['info'];
			$req = $con['req'];
	
			if ($info['http_code'] != 204)
				echo 'Error : ' . $info['http_code'];
			else
				echo 'The target : ' . $unicity . ' is add to the segment : ' . $name;
		}
	}
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
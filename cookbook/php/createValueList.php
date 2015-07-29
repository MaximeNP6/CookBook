<?php

//Ici, renseignez la xKey et les parametres personnalises du message
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
$valueListsId = 012;	//Id de la liste a modifier ('null' si le critere est a creer)

$name = 'createValueList (php)';	//Nom de la liste
$ordered = false;	//Ordonnee : oui = 'true' / non = 'false'
$values = array(	//Valeurs de la liste
			array(	//Premiere valeur
				'index' => 1,	//Numeros de la valeur
				'value' => "Mr"),	//Valeur du champs
			array(	//Deuxieme valeur
				'index' => 2,	//Numeros de la valeur
				'value' => "Mme")	//Valeur du champs
			);

//On trouve l'addresse pour la requete
$url = 'http://v8.mailperformance.com/valueLists/' . $valueListsId;

//Creation du Json du message

$arr = array(
		'name' => $name,
		'ordered' => $ordered,
		'values' => $values);


//On affiche le message
$message = json_encode($arr);
echo $message . "\n";

//Connexion
if ($valueListsId != null)
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
	//La liste de valeurs a bien ete cree
	echo 'The value list : ' . $name . " is created / update.";
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
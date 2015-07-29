<?php

//Ici, renseignez la xKey et les parametres personnalises du message
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
$fieldId = 0123;	//Id du critere a modifier ('null' si le critere est a creer)

$type = 'numeric';	//Code pour creer un critere :
	//Types : email = 'email' / telephone = 'phone' / zone de texte = 'textArena' / chaine de caractere = 'textField' / valeur numerique = 'numeric' / date = 'date'
	//Listes : liste de valeurs = 'singleSelectList' / Liste de valeurs multiples = 'multipleSelectList'
$name = 'createField (php)';	//Nom du critere
$isUnicity = false;	//Unicite : oui = 'true' / non = 'false'
$isMandatory = true;	//Obligatoire : oui = 'true' / non = 'false'

$constraintOperator = 1;	//Pour les valeurs numeriques et les dates : 1 = '>' / 2 = '<' / 3 = '>=' / 4 = '<=' / 5 = '=' ('null' pour rien)
$constraintValue = "42";	//Valeur de la contrainte ex : ["42"] ('null' pour rien)

$valueListId = null;	//Id de la liste dans le cas d'un 'singelSelectList' ou un 'multipleSelectList' ('null' pour rien)

//On trouve l'addresse pour la requete
$url = 'http://v8.mailperformance.com/fields/' . $fieldId;

//Creation du Json du message
if ($constraintOperator != null && $constraintValue != null)
{
	$arr = array(
			'type' => $type,
			'name' => $name,
			'isUnicity' => $isUnicity,
			'isMandatory' => $isMandatory,
			'constraint' => array(
					'operator' => $constraintOperator,
					'value' => $constraintValue));
	
}
else if ($valueListId != null)
{
	$arr = array(
			'type' => $type,
			'name' => $name,
			'isUnicity' => $isUnicity,
			'isMandatory' => $isMandatory,
			'valueListId' => $valueListId);
}
else
{
	$arr = array(
			'type' => $type,
			'name' => $name,
			'isUnicity' => $isUnicity,
			'isMandatory' => $isMandatory);
}

//On affiche le message
$message = json_encode($arr);
echo $message . "\n";

//Connexion
if ($fieldId != null)
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
	//Le critere a bien ete cree
	echo 'The field  : ' . $name . " is created / update.\n\n";
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
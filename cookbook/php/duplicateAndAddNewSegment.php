<?php

//Url de base
$urlBase = 'http://v8.mailperformance.com/';

//Ici, renseignez l'email de la cible, la X-Key
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
$unicity = 'test@test.com';	//L'adresse mail de la cible a mettre dans le segment ('null' pour ne rien ajouter au segment)
$segmentId = 0123;	//Id du segment a modifier ('null' si le segment est a creer)
$actionId = "000ABC";	//Id de l'action a dupliquer


function getTargetJson($unicity)
{
	//Syntaxe pour les différents types d'informations de la cible :
	$targetString = "name";	//Chaine de caracteres
	$tagetListOfValues = "Mr";	//Liste de valeurs
	$targetEmail = $unicity;	// E-mail
	$tagetPhoneNumber = "0123456789";	// Telephone
	$targetTextZone = "150 caracters max";	//Zone de texte
	$targetNumbers = 123;	//Valeur numérique
	$targetDate = "01/01/2000";	//Date
	$largetListMultipleValues = array("valeur 1", "valeur 2");	//Liste de valeurs multiples
	
	//Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur de l'information"
	$targetData = array("5398" => $targetString,
			"5399" => $tagetListOfValues,
			"5400" => $targetEmail,
			"5452" => $tagetPhoneNumber,
			"5453" => $targetTextZone,
			"5454" => $targetNumbers,
			"5455" => $targetDate,
			"5456" => $largetListMultipleValues);
	$targetJson = json_encode($targetData);
	
	return ($targetJson);
}

function getSegmentJson()
{
	$segmentType = 'static';	//Code pour creer un segment Statique
	$segmentName = 'Nom du segment';	//Nom du segment
	$segmentDescription = 'Description';	//Description du segment
	$segmentExpiration = '2016-09-23T11:37:49.686Z';	//Date d'expiration du segment
	$segmentIsTest = true;	//Le test est un segment de test : oui = 'true' / non = 'false'	
		
	//Creation du Json du message
	$segmentData = array(
		'type' => $segmentType,
		'name' => $segmentName,
		'description' => $segmentDescription,
		'expiration' => $segmentExpiration,
		'isTest' => $segmentIsTest);
	
	//On affiche le message
	$segmentJson = json_encode($segmentData);
	echo $segmentJson . "\n";
	
	return ($segmentJson);
}

function getNewActionDuplicateJson($actionDuplicateJson, $segmentId)
{
	$actionDuplicateJson = json_decode($actionDuplicateJson, true);
	
	//Modification de la nouvelle action
	$actionDuplicateJson['scheduler'] = array(
		"type" => "asap",	//Envoie : immédiat = 'asap' / Date = 'scheduled'
		//"type" => "2015-07-27T08:15:00Z",	//Si type = 'scheduled' sinon a enlever
		"segments" => array(
			"selected" => array($segmentId)));
	$actionDuplicateJson['content']['html'] = "New message.";
	
	$newActionDuplicateJson = json_encode($actionDuplicateJson);
	echo $newActionDuplicateJson . "\n";
	
	return ($newActionDuplicateJson);
}

function getActionValidation($segmentId)
{
	//Creation du Json du message pour le test
	$actionValidationData = array(
		'fortest' => true,	//Phase de teste
		'campaignAnalyser' => false,	//Campaign Analyzer : 'true' = oui / 'false' = non
		'testSegments' => array($segmentId),	//Les Ids des differents segments de testes
		'mediaForTest' => null,	//Rediriger tous les tests vers une seule adresse ('null' pour aucune adresse)
		'textandHtml' => true,	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
		'comments' => null);	//Commentaire ('null' pour aucuns commentaires)
	
	//On affiche le Json
	$actionValidationJson = json_encode($actionValidationData);
	echo $actionValidationJson . "\n";
	
	return ($actionValidationJson);
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DES CREATION DES JSONS///////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DU PROGRAMME///////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//On trouve l'adresse pour la requete
$url = $urlBase . 'targets?unicity=' . $unicity;

//Requete 'GET'
$return = urlGet($url, $xKey);
$result = $return["result"];
$req = $return["req"];

$tab = createTarget($result, $req, $urlBase, $xKey, $unicity);
$req = $tab['req'];
$targetId = $tab['targetId'];

$tab = createSegment($urlBase, $segmentId, $xKey, $req);
$req = $tab['req'];
$segmentId = $tab['segmentId'];

$req = targetToSegment($urlBase, $targetId, $segmentId, $req, $xKey, $unicity);

$tab = duplicateAction($urlBase, $actionId, $req, $xKey);
$req = $tab['req'];
$actionDuplicateId = $tab['actionDuplicateId'];
$actionDuplicateJson = $tab['result'];

$req = updateActionDuplicate($urlBase, $actionDuplicateId, $req, $actionDuplicateJson, $segmentId, $xKey);

$req = testActionDuplicate($urlBase, $actionDuplicateId, $req, $segmentId, $xKey);
	

curl_close($req);


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DU PROGRAMME/////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DES FONCTIONS//////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function createTarget($result, $req, $urlBase, $xKey, $unicity)
{
	//Verification des reponses
	if ($result == false)
	{
		//La cible n'existe pas, nous devons la creer

		//Affichage de l'erreur
		$info = curl_getinfo($req);
		echo 'Error : ' . $info['http_code'] . " : Creation of the target.\n";

		//Nouvelle url
		$url = $urlBase . 'targets/';

		//On remplit la requete 'POST'
		$con = urlPostOrPut($req, 'POST', getTargetJson($unicity), $xKey, $url);
		echo $con['result'] . "\n";
	}
	else
	{
		//La cible existe, nous la mettons a jour

		//On affiche les anciennes valeurs
		echo $result . "\n";

		//Nouvelle url
		$url = $urlBase . 'targets/';

		//On remplit la requete 'PUT'
		$con = urlPostOrPut($req, 'PUT', getTargetJson($unicity), $xKey, $url);
	}
	$result = $con['result'];
	$info = $con['info'];
	$req = $con['req'];
	
	if ($info['http_code'] != 200)
	{
		echo 'Error : ' . $info['http_code'] . '. End';
		exit (1);
	}
	
	$tab = json_decode($result, TRUE);
	$targetId = $tab['id'];
	
	return (array('req' => $req, 'targetId' => $targetId));
}


function createSegment($urlBase, $segmentId, $xKey, $req)
{
	//On trouve l'addresse pour la requete
	$url = $urlBase . 'segments/' . $segmentId;

	//Connexion pour le segment
	if ($segmentId != null)
		$con = urlPostOrPut($req, 'PUT', getSegmentJson(), $xKey, $url);
	else
		$con = urlPostOrPut($req, 'POST', getSegmentJson(), $xKey, $url);

	$result = $con['result'];
	$info = $con['info'];
	$req = $con['req'];

	if ($info['http_code'] != 200)
	{
		echo 'Error : ' . $info['http_code'] . '. End';
		exit (1);
	}
	
	if ($segmentId == null)
	{
		$tab = json_decode($result, TRUE);
		$segmentId = $tab['id'];
	}
	
	return (array('req' => $req, 'segmentId' => $segmentId));
}

function targetToSegment($urlBase, $targetId, $segmentId, $req, $xKey, $unicity)
{
	echo "Target : " . $targetId . " add to the segment : " . $segmentId . "\n\n";
	
	//Nouvelle url
	$url = $urlBase . 'targets/' . $targetId . '/segments/' . $segmentId;

	//Connexion
	$con = urlPostOrPut($req, 'POST', null, $xKey, $url);

	$result = $con['result'];
	$info = $con['info'];
	$req = $con['req'];

	if ($info['http_code'] != 204)
	{
		echo 'Error : ' . $info['http_code'] . '. Fin';
		exit (1);
	}
	else
		echo 'The target : ' . $unicity . " is add to the segment.\n";
	
	return ($req);
}

function duplicateAction($urlBase, $actionId, $req, $xKey)
{
	//Nouvelle url
	$url = $urlBase . 'actions/' . $actionId . '/duplication';

	//On duplique a l'identique
	$con = urlPostOrPut($req, 'POST', null, $xKey, $url);

	$result = $con['result'];
	$info = $con['info'];
	$req = $con['req'];

	if ($info['http_code'] != 200)
	{
		echo "Error request 'POST' duplication : " . $info['http_code'];
		exit (1);
	}
	else
	{
		//On affiche la cible
		echo "\nThe action duplicate ( " . json_decode($result, TRUE)['name'] . " ) is created.\n";
		echo $result . "\n\n";
	}

	//On recupere l'id de l'action
	$tab = json_decode($result, TRUE);
	$actionDuplicateId = $tab['id'];

	return (array('req' => $req, 'actionDuplicateId' => $actionDuplicateId, 'result' => $result));
}

function updateActionDuplicate($urlBase, $actionDuplicateId, $req, $actionDuplicateJson, $segmentId, $xKey)
{
	//Nouvelle url
	$url = $urlBase . 'actions/' . $actionDuplicateId;
	
	//On modifie l'action
	$con = urlPostOrPut($req, 'PUT', getNewActionDuplicateJson($actionDuplicateJson, $segmentId), $xKey, $url);

	$result = $con['result'];
	$info = $con['info'];
	$req = $con['req'];

	if ($info['http_code'] != 200)
	{
		echo "Error request 'PUT' modification : " . $info['http_code'];
		exit (1);
	}
	else
	{
		echo "The action duplicate ( " . json_decode($result, TRUE)['name'] . " ) is update.\n";
	}

	return ($req);
}

function testActionDuplicate($urlBase, $actionDuplicateId, $req, $segmentId, $xKey)
{
	//On test l'action
	
	//Nouvelle url
	$url = $urlBase . 'actions/' . $actionDuplicateId . '/validation';
	
	echo "\n\n" . $url . "\n\n";
	
	//Connexion
	$con = urlPostOrPut($req, 'POST', getActionValidation($segmentId), $xKey, $url);
	$result = $con['result'];
	$info = $con['info'];
	$req = $con['req'];
	
	//On attend que la phase de test soit terminee (peut prendre plusieurs minutes)
	$actionState = waitForState($actionDuplicateId, $xKey, $urlBase);
	
	//On verifie les reponses
	if ($info['http_code'] != 204 || $actionState == 20)
	{
		if ($actionState == 20)
			echo 'Error : the test failed';
		else
			echo 'Error : ' . $info['http_code'];
	}
	else
	{
		//La phase de test a reussi
		echo "The action is tested.\n\n";
	}
	
	return ($req);	
}

//Utilisation de cURL pour remplir les requetes
//Fonctions de connexion
function startCurlInit($url)
{
	$init = curl_init();
	curl_setopt($init, CURLOPT_URL, $url);
	curl_setopt($init, CURLOPT_RETURNTRANSFER, true);
	return ($init);
}

function urlGet($url, $xKey)
{
	//On remplit la requete 'GET'
	$req = startCurlInit($url);
	curl_setopt($req, CURLOPT_CUSTOMREQUEST, 'GET');

	//Mise en place du xKey et des options
	curl_setopt($req, CURLOPT_HTTPHEADER, array(
	'X-Key: ' . $xKey,
	'Content-Type: application/json'));

	//Execution de la requete
	$result = curl_exec($req);
	return (array('req' => $req, 'result' => $result));
}

function urlPostOrPut($req, $request, $dataJson, $xKey, $url)
{
	//On remplit la requete avec le bon verbe ($request) : POST / PUT
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
	
	return (array('result' => $result, 'info' => $info, 'req' => $req));
}

//Fonction qui verifie que la phase de test soit fini (peut prendre plusieurs minutes)
function waitForState($idAction, $xKey, $urlBase)
{
	$actionState = 30;

	while ($actionState != 38 && $actionState != 20)
	{
		//On attend 20 secondes
		echo "Wait 20sec...\n";
		sleep(20);

		//On trouve l'adresse pour la requete
		$url = $urlBase . 'actions/' . $idAction;

		//On remplit la requete 'GET'
		$req = startCurlInit($url);
		curl_setopt($req, CURLOPT_CUSTOMREQUEST, 'GET');

		//Mise en place du xKey et des options
		curl_setopt($req, CURLOPT_HTTPHEADER, array(
		'X-Key: ' . $xKey,
		'Content-Type: application/json'));

		//Execution de la requete
		$result = curl_exec($req);

		$tab = json_decode($result, TRUE);
		$actionState = $tab['informations']['state'];
	}
	return ($actionState);
}

?>
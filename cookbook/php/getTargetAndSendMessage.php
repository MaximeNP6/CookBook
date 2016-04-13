<?php

/**
 * Ce script permet de récuperer une cible grace à son caractère d'unicité et de
 * lui envoyer un message.
 *
 * @package cookbook
 */

require 'utils.php';

/**
 * Variable contenant les configurations pour se connecter à l'API
 *
 * @var array
 */
$configs = parse_ini_file("config.ini");
/**
 * Caractère d'unicité de la cible
 *
 * @var integer
 */
$unicity = 'test@test.com';
/**
 * Id du message à envoyer
 *
 * @var integer
 */
$idMessage = '00000';

// On trouve l'adresse pour la requete
$url = $configs['url'] . 'targets?unicity='. $unicity;
$con = connect($url, $configs['xKey'], null, 'GET');

// Verification des reponses
if ($con['result'] == true) {
	// On recupere l'id de la cible
	$tab = json_decode($con['result'], TRUE);
	$targetId = $tab['id'];

	// Nouvelle url en fonction de l'id du message et de la cible
	$url = $configs['url'] . 'actions/' . $idMessage . '/targets/' . $targetId;
	$con = connect($url, $configs['xKey'], null, 'POST');

	if ($con['info']['http_code'] != 204) {
		echo 'Error : ' . $con['info']['http_code'];
	}
	else {
		echo 'Message sent to ' . $unicity;
	}
}
else {
	// Affichage de l'erreur
	$info = curl_getinfo($req);
	echo 'Error : ' . $info['http_code'];
}

?>

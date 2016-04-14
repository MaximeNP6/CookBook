<?php

/**
 * Ce script permet d'envoyer un message personnalisés à une cible.
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
 * Caractère d'unicité qui vous permet d'identifier la cible
 *
 * @var string
 */
$unicity = 'test@test.com';
/**
 * Id du message qui sera utilisé (il est obligatoire de fournir un id valable)
 *
 * @var string
 */
$idMessage = 'XXXXX';
/**
 * Creation du JSON contenant les informations
 * (plus de détails : http://v8.mailperformance.com/doc/#api-Action-SendMessage)
 *
 * @var array
 */
$arr = array(
						'content' => array(
															'html' 		 => 'html message',
															'text' 		 => 'text message'
															),
						'header'	=> array(
															'subject'	 => 'subject of the message',
															'mailFrom' => 'mail@address.com',
															'replyTo'	 => 'mail@return.com'
															)
);


// On trouve l'adresse pour la requete
$url = $configs['url'] . 'targets?unicity='. $unicity;

$con = connect($url, $configs['xKey'], null, 'GET');

// Verification des reponses
if ($con['result'] == true) {
	// On recupere l'id de la cible
	$tab = json_decode($con['result'], TRUE);
	$idTarget = $tab['id'];

	// On affiche la cible
	echo $con['result'] . "\n";

	// Nouvelle url en fonction de l'id du message et de la cible
	$url = $configs['url'] . 'actions/' . $idMessage . '/targets/' . $idTarget;

	$dataJson = json_encode($arr);

	$con = connect($url, $configs['xKey'], $dataJson, 'POST');

	if ($con['info']['http_code'] == 204) {
		echo 'Message sent to ' . $unicity;
	}
	else {
		echo 'Error : ' . $con['info']['http_code'];
	}
}
else {
	// Affichage de l'erreur
	echo 'Error : ' . $con['info']['http_code'];
}

?>

<?php

/**
 * Ce script permet de créer un SMS.
 *
 * @package cookbook
 */

require __DIR__ . '/../utils.php';

/**
 * Variable contenant les configurations pour se connecter à l'API.
 *
 * @var array
 */
$configs = parse_ini_file(__DIR__. '/../config.ini');
/**
 * Code pour envoyer un SMS.
 *
 * @var string
 */
$type = 'smsMessage';
/**
 * Nom du SMS.
 *
 * @var string
 */
$name = 'SMSFromApi (php)';
/**
 * Description de l'action.
 *
 * @var string
 */
$description = 'SMSFromApi (php)';
/**
 * Id du dossier dans lequel vous voulez mettre l'action ('null' pour aucun dossier).
 *
 * @var integer
 */
$informationFolder = null;
/**
 * Id de la categorie de campagne
 * (Infos compte > Parametrage > Categories de campagnes)
 * ('null' pour aucune campagne).
 *
 * @var integer
 */
$informationCategory = null;
/**
 * Message texte contenu dans le sms.
 *
 * @var string
 */
$textContent = 'Text message / max 160';

// Creation du Json du message
$arr = array(
	'type'					=> $type,
	'name'					=> $name,
	'description'		=> $description,
	'informations'	=> array(
		'folder'			=> $informationFolder,
		'category'		=> $informationCategory
	),
	'content'				=> array('textContent' => $textContent)
);

// On affiche le message
$dataJson = json_encode($arr);
echo $dataJson . "\n";

// Connexion
$url = $configs['url'] . 'actions';
$con = connect($url, $configs['xKey'], $dataJson, 'POST');

// Verification des reponses
if ($con['info']['http_code'] == 200) {
	// L'action a bien été crée
	echo "Action : " . $name . " has been created.\n\n";
	echo 'Don\'t forget to normalize your phone numbers with your country.';
}
else {
	echo 'Error : ' . $con['info']['http_code'];
}
?>

<?php

/**
 * Ce script permet de récuperer une cible grace à son caractère d'unicité.
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
 * Caractère d'unicité qui vous permet d'identifier la cible que vous recherchez
 *
 * @var string
 */
$unicity = 'test@test.com';

// On trouve l'adresse pour la requete
$url = $configs['url'] . 'targets?unicity='. $unicity;
$con = connect($url, $configs['xKey'], null, 'GET');

// Verification des reponses
if ($con['result'] == true) {
	echo $con['result'];
}
else {
	// Affichage de l'erreur
	echo 'Error : ' . $con['info']['http_code'];
}

?>

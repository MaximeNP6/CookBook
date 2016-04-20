<?php

/**
 * Ce script permet de récuperer la listes des imports.
 *
 * @package cookbook
 */

require __DIR__ . '/../utils.php';

/**
 * Variable contenant les configurations pour se connecter à l'API
 *
 * @var array
 */
$configs = parse_ini_file(__DIR__. '/../config.ini');

$url = $configs['url'] . 'imports/';

$con = connect($url, $configs['xKey'], null, 'GET');

if ($con['result'] == false) {
  // Erreur lors de la requete

  // Affichage de l'erreur
  echo 'Error : ' . $con['info']['http_code'] . "\n";
}
else {
  // On affiche la reponse
  echo $con['result'] . "\n";
}

?>

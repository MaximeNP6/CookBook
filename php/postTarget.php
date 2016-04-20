<?php

/**
 * Ce script permet de créer une cible.
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
 * Creation du JSON en fonction des champs que vous voulez renseigner.
 * Remplacez les XXXX par l'id des champs que vous voulez renseigner.
 * Les valeurs renseignez ici sont à remplacer par les votres.
 *
 * @var array
 */
$data = array(
							"XXXX" => "Toto",
							"XXXX" => "Mr",
							"XXXX" => "test@test.com",
							"XXXX" => "0123456789",
							"XXXX" => "150 caracters max",
							"XXXX" => 123,
							"XXXX" => "01/01/2000",
							"XXXX" => array("valeur 1", "valeur 2"),
);
$dataJson = json_encode($data);


/**
 * On créé le liens pour verifier si la cible n'existe pas, remplacez XXXX par
 * l'id d'un champ d'unicité.
 */
$url = $configs['url'] . 'targets?unicity=' . $data['XXXX'];

$con = connect($url, $configs['xKey'], null, 'GET');

// Verification des reponses
if ($con['result'] == false) {
  // La cible n'éxiste pas, nous devons la créer

  // Affichage de l'erreur
  echo "Error : " .
       $con['info']['http_code'] .
       " : Creation of the target.\n";

  $con = connect($url, $configs['xKey'], $dataJson, 'POST');
	echo "The target " .
			 $data['XXXX'] .
			 " has been created.";
}
else {
  // La cible existe, nous la mettons a jour

  // On affiche les anciennes valeurs
  echo $con['result'] . "\n";

  // On remplit la requete 'PUT'
  $con = connect($url, $configs['xKey'], $dataJson, 'PUT');
  echo "The target " .
       $data['XXXX'] .
       " has been updated.";
}

?>

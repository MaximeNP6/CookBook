<?php

/**
 * Ce script permet de créer une cible si elle n'existe pas et de l'ajouter
 * à un ségment.
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
 * Id du segment vers lequel sera ajouté la cible
 *
 * @var string
 */
$idSegment = 'XXXXX';
/**
 * Creation du JSON en fonction des champs que vous voulez renseigner.
 * Remplacez les XXXX par l'id des champs que vous voulez renseigner.
 * Les valeurs renseignez ici sont à remplacer par les votres.
 *
 * @var array
 */
$data = array(
	"XXXX" => "name",
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
$url = $configs['url'] . 'targets?unicity=' . $data['8628'];

$con = connect($url, $configs['xKey'], null, 'GET');

// Verification des reponses
if ($con['result'] == false) {
  // La cible n'éxiste pas, nous devons la créer

  // Affichage de l'erreur
  echo "Error : " .
       $con['info']['http_code'] .
       " : Creation of the target.\n";

  $con = connect($url, $configs['xKey'], $dataJson, 'POST');
}

// On affiche les valeurs renvoyé par l'API
echo $con['result'] . "\n";

// On recupere l'id de la cible
$tab = json_decode($con['result'], TRUE);
$idTarget = $tab['id'];

//On remplit la requete 'POST'
$url = $configs['url'] . 'targets/' . $idTarget . '/segments/' . $idSegment;
$con = connect($url, $configs['xKey'], null, 'POST');
if ($con['result'] != 'error') {
	echo "The target " .
			 $data['8628'] .
			 " has been added to the segment " .
			 $idSegment . ".";
}

?>

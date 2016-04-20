<?php

/**
 * Ce script permet de créer ou modifier une Catégorie.
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
 * Id de la catégorie à créer/modifier ('null' si la catégorie doit être créée)
 *
 * @var integer
 */
$categorieId = null;
/**
 * Nom de la catégorie
 *
 * @var string
 */
$name = 'Category php';
/**
 * Description de la catégorie
 *
 * @var string
 */
$description = 'Category (php)';


/**
 * Creation du JSON contenant les informations
 * (pour plus de détails : http://v8.mailperformance.com/doc/#api-Category)
 *
 * @var array
 */
$data = array(
	'name'	 			=> $name,
	'description' => $description);
$dataJson = json_encode($data);

// On affiche le message
echo $dataJson . "\n";

// Connexion
$url = $configs['url'] . 'categories/' . $categorieId;
if ($categorieId != null) {
	$con = connect($url, $configs['xKey'], $dataJson, 'PUT');
}
else {
	$con = connect($url, $configs['xKey'], $dataJson, 'POST');
}
$result = $con['result'];
$info   = $con['info'];

if ($info['http_code'] == 200) {
	// La catégorie a bien été créée
	echo "\nThe category  : " . $name . " has been created / updated.";
}
else {
	echo 'Error : ' . $info['http_code'];
}
?>

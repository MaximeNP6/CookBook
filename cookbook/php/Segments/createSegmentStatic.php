<?php

/**
 * Ce script permet de créer un Segment Statique.
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
 * Id du segment à modifier. ('null' si le segment doit être créé)
 *
 * @var integer
 */
$segmentId = null;
/**
 * Code pour créer un segment Statique.
 *
 * @var string
 */
$type = 'static';
/**
 * Nom du segment.
 *
 * @var string
 */
$name = 'SegmentStatic (php)';
/**
 * Description du segment.
 *
 * @var string
 */
$description = 'SegmentStatic (php)';
/**
 * Date d'expiration du segment.
 *
 * @var string
 */
$expiration = '2026-08-08T12:11:00Z';
/**
 * Défini si le Segment est un Segment de test ou non.
 *
 * @var bool
 */
$isTest = true;

// Creation du Json du message
$arr = array(
            'type'        => $type,
            'name'        => $name,
            'description' => $description,
            'expiration'  => $expiration,
            'isTest'      => $isTest
            );
// On affiche le message
$dataJson = json_encode($arr);
echo $dataJson . "\n";

// Connexion
$url = $configs['url'] . 'V1/segments/' . $segmentId;
if ($segmentId != null) {
  $con = connect($url, $configs['xKey'], $dataJson, 'PUT');
}
else {
  $con = connect($url, $configs['xKey'], $dataJson, 'POST');
}

if ($con['info']['http_code'] == 200) {
  // Le segment a bien ete cree
  echo 'The segment static  : ' . $name . " has been created / updated.\n\n";
}
else {
  echo 'Error : ' . $con['info']['http_code'];
}

?>

<?php

/**
 * Ce script permet d'envoyer un message à un ségment.
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
 * Id du segment
 *
 * @var integer
 */
$idSegment = 1234;
/**
 * Id du message
 *
 * @var string
 */
$idAction = "XXXXX";


$url = $configs['url'] . "segments/" . $idSegment . "/targets/";
// On Recupere toutes les id Targets
$con = connect($url, $configs['xKey'], null, 'GET');

if ($con['info']['http_code'] != 200) {
  echo "Error: while trying to get targets of segment with id: " . $idSegment;
  return;
}

$targets = json_decode($con['result']);

// Pour chaque Target on fait un SendMessage
foreach ($targets as $idTarget) {
  // Url d'envoie du message à la cible
  $url = $configs['url'] . "actions/" . $idAction . "/targets/" . $idTarget;

  //SendMessage
  $con = connect($url, $configs['xKey'], null, 'POST');

  if ($con['info']['http_code'] != 204) {
    echo "Error " . $con['info']['http_code'] .
         ": while trying to send message to the targets with id: " .
         $idTarget . "\n";
  }
}

?>

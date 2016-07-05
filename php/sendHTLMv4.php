<?php

/**
 * Ce script permet d'envoyer un message v4 personnalisés à une cible.
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

?>

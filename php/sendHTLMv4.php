<?php

/**
 * Ce script permet d'envoyer un message v4 personnalisés à une cible.
 *
 * @package cookbook
 */

require "utils.php";

/**
 * Fonction d'envoie de mail v4
 *
 * @param string $url
 * @param string $xKey
 * @param string $dataJson
 *
 * @return array
 */
function sendV4messages($url, $xKey, $dataJson) {
    $ch = startCurl($url);

    // Ajout du Accept specifique pour l'envoie de mail v4
    $header = [
        "X-Key: " . $xKey,
        "Content-Type: application/json",
        "Accept: application/vnd.mperf.v8.message",
        "Content-Length: " . strlen($dataJson)
    ];

    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
    curl_setopt($ch, CURLOPT_HTTPHEADER, $header);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $dataJson);

    // Execution de la requete
    $result = curl_exec($ch);

    // Verification des reponses
    $info = curl_getinfo($ch);

    stopCurl($ch);

    return (["result" => $result, "info" => $info]);
}

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
$unicity = "hp@np6.com";
/**
 * Id du message qui sera utilisé (il est obligatoire de fournir un id valable)
 *
 * @var string
 */
$idMessage = "0011K1";
/**
 * Creation du JSON contenant les informations
 * (plus de détails : https://backoffice.mailperformance.com/doc/#api-Action-SendMessage)
 *
 * @var array
 */
$arr = [
    "views"             => [
        "html"          => "html message",
        "text"          => "text message",
    ],
];


// On trouve l'adresse pour la requete
$url = $configs["url"] . "targets?unicity=". $unicity;

$con = connect($url, $configs["xKey"], null, "GET");

// Verification des reponses
if ($con["result"] == true)
{
    // On recupere l'id de la cible
    $tab = json_decode($con["result"], TRUE);
    $idTarget = $tab["id"];

    // On affiche la cible
    echo "target JSON =\n" . $con["result"] . "\n\n";

    // Nouvelle url en fonction de l'id du message et de la cible
    $url = $configs["url"] . "actions/" . $idMessage . "/targets/" . $idTarget;

    $dataJson = json_encode($arr);

    echo "Url = " . $url . "\n";
    echo "JSON =\n" . $dataJson . "\n\n";

    // Envoie du mail surchargé
    $con = sendV4messages($url, $configs["xKey"], $dataJson);

    if ($con["info"]["http_code"] == 204)
    {
        echo "Message sent to " . $unicity . "\n";
    }
    else
    {
        echo "Error : " . $con["info"]["http_code"] . "\n";
    }
}
else
{
    // Affichage de l'erreur
    echo "Error : " . $con["info"]["http_code"] . " while looking for the target" . "\n";
}

?>

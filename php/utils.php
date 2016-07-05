<?php

/**
 * Ce fichier contient les fonctions permettant de se connecter à l'API.
 *
 * @package cookbook
 */

/**
 * Fonction d'initialisation de cURL
 *
 * @param string $url
 *
 * @return resource
 */
function startCurl($url) {
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    return ($ch);
}

/**
 * Fonction d'arrêt de cURL
 *
 * @param resource $ch
 */
function stopCurl($ch) {
 curl_close($ch);
}

/**
 * Fonction de connexion à l'API
 *
 * @param string $url
 * @param string $xKey
 * @param string $dataJson
 * @param string $method
 *
 * @return array
 */
function connect($url, $xKey, $dataJson, $method) {
    $ch = startCurl($url);

    $header = [
        "X-Key: " . $xKey,
        "Content-Type: application/json",
        "Content-Length: " . strlen($dataJson)
    ];

    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, $method);
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
 * Fonction qui verifie que la phase de test soit fini
 * (peut prendre plusieurs minutes)
 *
 * @param string $idAction
 * @param string $baseUrl
 * @param string $xKey
 *
 * @return integer
 */
function waitForState($idAction, $baseUrl, $xKey) {
  $actionState = 30;

  while ($actionState != 38 && $actionState != 20 && $actionState != 10) {
    // On attend 20 secondes
    echo "Wait 20sec...\n";
    sleep(20);

    $url = $baseUrl . "actions/" . $idAction;

    $con = connect($url, $xKey, null, "GET");

    $tab = json_decode($con["result"], TRUE);
    $actionState = $tab["informations"]["state"];

    }
  return ($actionState);
}

/**
 * Fonction d'envoie de fichier à l'API
 *
 * @param string $url
 * @param string $xKey
 * @param string $dataJson
 * @param string $method
 *
 * @return array
 */
function sendFile($url, $xKey, $dataJson, $method = "PUT") {
    $ch = startCurl($url);

    $header = [
        "X-Key: " . $xKey,
        "Accept: application/vnd.mperf.v8.import.v1+json",
        "Content-Type: application/octet-stream",
        "Content-Disposition: form-data; filename=" . $dataJson["name"]
    ];

    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, $method);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $dataJson["data"]);
    curl_setopt($ch, CURLOPT_HTTPHEADER, $header);

    // Execution de la requete
    $result = curl_exec($ch);

    // Verification des reponses
    $info = curl_getinfo($ch);

    stopCurl($ch);

    return (["result" => $result, "info" => $info]);
}

?>

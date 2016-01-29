<?php

//Url de base
$urlBase = 'http://v8.mailperformance.com/';

//X-Key
$xKey = 'ABCDEFJHIJKLMNOPQRSTUVWXYZ0123456789';

$idSegment = 1234;	//Id du segment
$idAction = "00012A";	//Id de l'action


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DU PROGRAMME///////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

$req = NULL;

//Nouvelle url
$url = $urlBase . "segments/" . $idSegment . "/targets/";
//On Recupere toutes les id Targets
$con = urlGet($url, $xKey);
if (($req = verification($con)) == null)
    return (0);

$result = $con['result'];
$req = $con['req'];
$tab = myTab($result);

//Pour chaque Target on fait un SendMessage
foreach ($tab as $idTarget)
{
    //Nouvelle url
    $url = $urlBase . "actions/" . $idAction . "/targets/" . $idTarget;
    //SendMessage
    $con = urlPostOrPut($req, 'POST', null, $xKey, $url, 1);
    if (($req = verification($con)) == null)
        return (0);
    $req = $con['req'];
}

curl_close($req);

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DU PROGRAMME/////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DES FONCTIONS//////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function myTab($result)
{
    $str = null;

    //On transforme la string en tableau de char
    $tabChar = str_split($result, 1);

    //On regarde tout le tableau de char
    foreach ($tabChar as $value)
    {
        //Si un caractere est bon, on le garde
        if ((ord($value) >= ord("0") && ord($value) <= ord("9"))
                || (ord($value) >= ord("A") && ord($value) <= ord("Z"))
                || $value == ",")
            $str = $str . $value;
    }

    //On transforme la string en tableau de string
    $tab = explode(",", $str);

    //On return
    return ($tab);
}

function verification($con)
{
    $result = $con['result'];
    $info = $con['info'];
    $req = $con['req'];

    //Verification des reponses
    if ($info['http_code'] != 200 && $info['http_code'] != 204)
    {
        //Erreur lors de la requete

        //Affichage de l'erreur
        $info = curl_getinfo($req);
        echo 'Error : ' . $info['http_code'] . "\n" . $result;
        return (null);
    }
    elseif ($info['http_code'] == 200)
    {
        //On affiche la reponse
        echo 'OK : ' . $result . "\n\n";
        return ($req);
    }
    else
    {
        //On affiche la reponse
        echo "OK\n\n";
        return ($req);
    }
}



//Utilisation de cURL pour remplir les requetes
//Fonctions de connexion
function startCurlInit($url)
{
    echo 'URL = ' . $url . "\n";
    $init = curl_init();
    curl_setopt($init, CURLOPT_URL, $url);
    curl_setopt($init, CURLOPT_RETURNTRANSFER, true);
    return ($init);
}

function urlGet($url, $xKey)
{
    //On remplit la requete 'GET'
    $req = startCurlInit($url);
    curl_setopt($req, CURLOPT_CUSTOMREQUEST, 'GET');

    //Mise en place du xKey et des options
    curl_setopt($req, CURLOPT_HTTPHEADER, array(
    'X-Key: ' . $xKey,
    'Content-Type: application/json'));

    //Execution de la requete
    $result = curl_exec($req);

    //Verification des reponses
    $info = curl_getinfo($req);

    return (array('result' => $result, 'info' => $info, 'req' => $req));
}

function urlPostOrPut($req, $request, $dataJson, $xKey, $url, $source)
{
    //On remplit la requete avec le bon verbe ($request) : POST / PUT
    $req = startCurlInit($url);
    curl_setopt($req, CURLOPT_CUSTOMREQUEST, $request);

    //Mise en place du xKey et des options
    curl_setopt($req, CURLOPT_POSTFIELDS, $dataJson);
    curl_setopt($req, CURLOPT_HTTPHEADER, array(
            'X-Key: ' . $xKey,
            'Content-Type: application/json',
            'Content-Length: ' . strlen($dataJson)
    ));

    //Execution de la requete
    $result = curl_exec($req);

    //Verification des reponses
    $info = curl_getinfo($req);

    return (array('result' => $result, 'info' => $info, 'req' => $req));
}

?>

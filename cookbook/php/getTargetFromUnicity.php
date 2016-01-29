<?php

//Ici, renseignez l'email
$unicity = 'test@test.com';

//Utilisation de cURL pour remplir la requete
$req = curl_init();
curl_setopt($req,CURLOPT_URL,'http://v8.mailperformance.com/targets?unicity='. $unicity);
curl_setopt($req,CURLOPT_CUSTOMREQUEST,'GET');
curl_setopt($req, CURLOPT_RETURNTRANSFER, true);

//Mise en place du xKey et des options
curl_setopt($req, CURLOPT_HTTPHEADER, array(
'X-Key: ABCDEFJHIJKLMNOPQRSTUVWXYZ0123456789',
'Content-Type: application/json'));

//Execution de la requete
$result = curl_exec($req);

//Verification des reponses
if ($result == false)
{
	//Affichage de l'erreur
	$info = curl_getinfo($req);
	echo 'Error : ' . $info['http_code'];
}
else
{
	//Affichage des donnees
	echo $result;
}
curl_close($req);

?>
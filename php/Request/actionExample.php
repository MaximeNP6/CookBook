<?php

/*
** =====================================================================================================
** Create Reminder
** =====================================================================================================
*/

//Parametres
$id = '000123';
$parentId = '0123';
$startDate = '2016-01-08T12:11:00Z';
$criterion = 0;
$useReminder = false;

//Creation du Json du message
$json = array(
	'id' => $id,
	'parentId' => $parentId,
	'startDate' => $startDate,
	'criterion' => $criterion,
	'useReminder' => $useReminder);

//On affiche le message
$message = json_encode($json);
echo $message . "\n";


/*
** =====================================================================================================
** Create an action
** =====================================================================================================
*/

//Parametres
$type = 'mailMessage'; // mailMessage - mailCampaign - smsMessage - smsCampaign
$name = 'Action';
$description = 'Action';

$informationFolder = 0123;
$informationCategory = 0123;

$contentHeadersFromPrefix = 'prefix';
$contentHeadersFromLabel = 'label';
$contentHeadersReply = 'address@reply.com';

$contentSubject = 'Subject of the message';
$contentHTML = 'Html message';
$contentText = 'Text message';

//Creation du Json du message
$json = array(
	'type' => $type,
	'name' => $name,
	'description' => $description,
	'informations' => array(
		'folder' => $informationFolder,
		'category' => $informationCategory),
	'content' => array(
		'headers' => array(
			'from' => array(
				'prefix' => $contentHeadersFromPrefix,
				'label' => $contentHeadersFromLabel),
			'reply' => $contentHeadersReply),
		'subject' => $contentSubject, 
		'html' => $contentHTML, 
		'text' => $contentText));

//On affiche le message
$message = json_encode($json);
echo $message . "\n";


/*
** =====================================================================================================
** Delete an action
** =====================================================================================================
*/

$id = '000123';
$url = 'https://backoffice.mailperformance.com/actions/' . $id;

echo $url . "\n";


/*
** =====================================================================================================
** Duplicate an action
** =====================================================================================================
*/

//Parametres
$type = 'mailMessage'; // mailMessage - mailCampaign - smsMessage - smsCampaign
$id = '000123';
$name = 'Action';
$description = 'Action';

$informationFolder = 0123;
$informationCategory = 0123;

$contentHeadersFromPrefix = 'prefix';
$contentHeadersFromLabel = 'label';
$contentHeadersReply = 'address@reply.com';

$contentSubject = 'Subject of the message';
$contentHTML = 'Html message';
$contentText = 'Text message';

//Creation du Json du message
$json = array(
	'type' => $type,
	'id' => $id,
	'name' => $name,
	'description' => $description,
	'informations' => array(
		'folder' => $informationFolder,
		'category' => $informationCategory),
	'content' => array(
		'headers' => array(
			'from' => array(
				'prefix' => $contentHeadersFromPrefix,
				'label' => $contentHeadersFromLabel),
			'reply' => $contentHeadersReply),
		'subject' => $contentSubject, 
		'html' => $contentHTML, 
		'text' => $contentText));

//On affiche le message
$message = json_encode($json);
echo $message . "\n";


/*
** =====================================================================================================
** Export a list of all recipients of an action
** =====================================================================================================
*/

$id = '000123';
$url = 'https://backoffice.mailperformance.com/actions/' . $id . '/export/recipients';

echo $url . "\n";


/*
** =====================================================================================================
** Export a list of bounced recipients of an action
** =====================================================================================================
*/

$id = '000123';
$url = 'https://backoffice.mailperformance.com/actions/' . $id . '/export/feedback/bounces';

echo $url . "\n";


/*
** =====================================================================================================
** Export a list of recipients of an action who have complained on receipt of the action
** =====================================================================================================
*/

$id = '000123';
$url = 'https://backoffice.mailperformance.com/actions/' . $id . '/export/feedback/complaints';

echo $url . "\n";


/*
** =====================================================================================================
** Export a list of recipients who have clicked a link on receipt of an action
** =====================================================================================================
*/

$id = '000123';
$url = 'https://backoffice.mailperformance.com/actions/' . $id . '/export/tracking/clicks';

echo $url . "\n";


/*
** =====================================================================================================
** Export a list of recipients who have opened an action
** =====================================================================================================
*/

$id = '000123';
$url = 'https://backoffice.mailperformance.com/actions/' . $id . '/export/tracking/opens';

echo $url . "\n";


/*
** =====================================================================================================
** Export a list of recipients who have unsubscribed on receipt of an action
** =====================================================================================================
*/

$id = '000123';
$url = 'https://backoffice.mailperformance.com/actions/' . $id . '/export/feedback/unsubscribes';

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve all actions
** =====================================================================================================
*/

$url = 'https://backoffice.mailperformance.com/actions/';

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve an action
** =====================================================================================================
*/

$id = '000123';
$url = 'https://backoffice.mailperformance.com/actions/' . $id;

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve the estimated number of recipients of an action
** =====================================================================================================
*/

$id = '000123';
$url = 'https://backoffice.mailperformance.com/actions/' . $id . '/recipients/count';

echo $url . "\n";


/*
** =====================================================================================================
** Send a message (email or sms) to a target. For email messages only, optionally replace the message's configured content and headers with those passed as parameters
** =====================================================================================================
*/

//Parametres
$html = 'Html message';
$text = 'Text message';

$subject = 'Subject of the message';
$mailFrom = 'address@from.com';
$replyTo = 'address@reply.com';


//Creation du Json du message
$json = array(
	'content' => array(
		'html' => $html, 
		'text' => $text),
	'header' => array(
		'subject' => $subject,
		'mailFrom' => $mailFrom,
		'replyTo' => $replyTo));

//On affiche le message
$message = json_encode($json);
echo $message . "\n";


/*
** =====================================================================================================
** Unvalidate an action
** =====================================================================================================
*/

$id = '000123';
$url = 'https://backoffice.mailperformance.com/actions/' . $id . '/validation';

echo $url . "\n";


/*
** =====================================================================================================
** Update an action
** =====================================================================================================
*/

//Parametres
$type = 'mailMessage'; // mailMessage - mailCampaign - smsMessage - smsCampaign
$id = '000123';
$name = 'MailMessage';
$description = 'Action';

$informationFolder = 0123;
$informationCategory = 0123;

$contentHeadersFromPrefix = 'prefix';
$contentHeadersFromLabel = 'label';
$contentHeadersReply = 'address@reply.com';

$contentSubject = 'Subject of the message';
$contentHTML = 'Html message';
$contentText = 'Text message';

//Creation du Json du message
$json = array(
	'type' => $type,
	'id' => $id,
	'name' => $name,
	'description' => $description,
	'informations' => array(
		'folder' => $informationFolder,
		'category' => $informationCategory),
	'content' => array(
		'headers' => array(
			'from' => array(
				'prefix' => $contentHeadersFromPrefix,
				'label' => $contentHeadersFromLabel),
			'reply' => $contentHeadersReply),
		'subject' => $contentSubject, 
		'html' => $contentHTML, 
		'text' => $contentText));

//On affiche le message
$message = json_encode($json);
echo $message . "\n";


/*
** =====================================================================================================
** Validate an action or launch the test phase of an action
** =====================================================================================================
*/

//Parametres
$idTestSegment = 0123;

//Creation du Json du message pour le test
$json = array(
	'fortest' => false,
	'campaignAnalyser' => false,
	'testSegments' => array($idTestSegment),
	'mediaForTest' => null,
	'textandHtml' => false,
	'comments' => null);

//On affiche le message
$message = json_encode($json);
echo $message . "\n";

?>
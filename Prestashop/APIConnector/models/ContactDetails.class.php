<?php
/**
 * 2014-2014 NP6 SAS
*
* NOTICE OF LICENSE
*
* This source file is subject to the Academic Free License (AFL 3.0)
* that is bundled with this package in the file LICENSE.txt.
* It is also available through the world-wide-web at this URL:
* http://opensource.org/licenses/afl-3.0.php
* If you did not receive a copy of the license and are unable to
* obtain it through the world-wide-web, please send an email
* to license@prestashop.com so we can send you a copy immediately.
*
*  @author    NP6 SAS <contact@np6.com>
*  @copyright 2014-2014 NP6 SAS
*  @license   http://opensource.org/licenses/afl-3.0.php  Academic Free License (AFL 3.0)
*  International Registered Trademark & Property of NP6 SAS
*/

require_once (dirname(__FILE__).DIRECTORY_SEPARATOR.'..'.DIRECTORY_SEPARATOR.'APIConnectorIncludes.php');
require_once (dirname(__FILE__).DIRECTORY_SEPARATOR.'Entity.class.php');

class ContactDetails extends EntityAbstract
{
	var $id;
	var $politness;
	var $first_name;
	var $last_name;
	var $user_name;
	var $email;
	var $creator;
	var $creation_date;
	var $expiration_date;
	var $auto_login_url;
	var $status;

	/**
	 * constructor
	 */
	public function __construct()
	{
		$ctp = func_num_args();
		if ($ctp == 1)
		{
			$args = func_get_args();
			$this->parse($args[0]);
		}
	}

	/**
	 * parse un tableau en attribut
	 *
	 * @param array $record        
	 */
	protected function parse($record)
	{
		$this->id = $record['id'];
		$this->politness = $record['politness'];
		$this->email = $record['email'];
		$this->auto_login_url = $record['autoLoginUrl'];
		$this->creation_date = $record['creationDate'];
		$this->first_name = $record['firstName'];
		$this->last_name = $record['lastName'];
		$this->user_name = $record['userName'];
		$this->status = $record['status'];
		$this->creator = $record['creator'];
		if (isset($record['expire']))
			$this->expiration_date = $record['expire'];
	}

	/**
	 * check if the json array is valid
	 * @param array $json
	 * @return boolean
	 */
	public static function isJsonValid($json)
	{
		if (isset($json['id']) && isset($json['firstName']) && isset($json['lastName'])
				&& isset($json['userName']) && isset($json['email']))
			return true;
		return false;
	}
}

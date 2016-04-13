"""
Ce script permet de passer une action en phase de test puis de validation
"""

import json
import sys
import requests
import Utils


def send_BAT_and_validate_campaign(config, action_id):
    """
    Création de la phase de test puis de la phase de validation

    Pour plus d'informations : http://v8.mailperformance.com/doc/#api-Action-ValidateAsync

    Description du fichier validation.json

    fortest             => Phase de test
    campaignAnalyser    => Activer le Campaign Analyzer
    testSegments        => IDs des differents segments de test
    mediaForTest        => Rediriger tous les tests vers une seule adresse ('null' pour aucune adresse)
    textandHtml         => Envoyer la version texte et la version html
    comments            => Commentaire ('null' pour aucuns commentaires)

    """

    with open('Models/validation.json') as data_file:
        data = json.load(data_file)

    data_json = json.JSONEncoder().encode(data)

    url = config['API']['url'] + 'actions/' + action_id + '/validation'
    headers = Utils.create_headers(config['API']['xKey'], len(data_json))
    req = requests.post(url, data=data_json, headers=headers)
    req.raise_for_status()

    print("Beginning of Test.")

    action_state = Utils.wait_for_state(action_id, config['API']['url'], config['API']['xKey'])

    if action_state == 38:
        print("Test passed.")
    else:
        print("Error : the test has failed.", file=sys.stderr)

    # On fait passer la demande de test en demande de validation

    data['fortest'] = False
    data['testSegments'] = None

    data_json = json.JSONEncoder().encode(data)

    print("Beginning of Validation.")

    url = config['API']['url'] + 'actions/' + action_id + '/validation'
    headers = Utils.create_headers(config['API']['xKey'], len(data_json))
    req = requests.post(url, data=data_json, headers=headers)
    req.raise_for_status()

    print("Validation passed.")

def main():
    """
    Pensez à modifier l'id de l'action à valider.
    """
    action_id = "XXXXXXXX"
    config = Utils.load_config()
    send_BAT_and_validate_campaign(config, action_id)

if __name__ == '__main__':
    main()

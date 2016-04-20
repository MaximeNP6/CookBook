"""
Fichier contenant toutes les fonctions nécessaires au fonctionnement
des différents scripts.
"""

import configparser
import json
import time
import requests

def load_config():
    """Retourne le fichier de configuration"""
    config = configparser.ConfigParser()
    config.read('config.ini')
    return config

def create_headers(x_key, data_json_length):
    """Retourne le header necessaire pour la connexion à l'API"""
    headers = {
        'X-Key': x_key,
        'Content-Type': 'application/json',
        'Content-Length': str(data_json_length)
        }
    return headers

def wait_for_state(action_id, base_url, x_key):
    """
    Fonction qui verifie que la phase de test soit fini
    (peut prendre plusieurs minutes)
    """
    action_state = 30

    while action_state != 38 and action_state != 20 and action_state != 10:
        # On attend 20 secondes
        print("Wait 20sec...")
        time.sleep(20)


        url = base_url + 'actions/' + action_id

        headers = create_headers(x_key, 0)
        req = requests.get(url, headers=headers)

        action_state = json.loads(req.text)['informations']['state']
    return action_state

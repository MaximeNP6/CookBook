"""
Ce script permet d'envoyer un message à un ségment.
"""

import requests
import Utils

def get_targets_from_segment(config, segment_id):
    """Récupération des cibles"""
    url = config['API']['url'] + 'segments/' + segment_id + '/targets/'

    # Connexion à l'API
    print("Getting all targets from segment.")
    headers = Utils.create_headers(config['API']['xKey'], 0)
    req = requests.get(url, headers=headers)
    req.raise_for_status()
    print("All targets acquired.")
    return req.json()

def send_action_to_targets(config, targets, action_id):
    """Envoie du message aux cibles"""

    print("Sending message to each target.")
    for target in targets:
        url = config['API']['url'] + "actions/" + action_id + "/targets/" + target

        # SendMessage
        headers = Utils.create_headers(config['API']['xKey'], 0)
        req = requests.post(url, headers=headers)
        req.raise_for_status()
    print("Message sent.")

def main():
    """
    Pensez à modifier les ID.
    """
    action_id = "XXXXXXX"
    segment_id = "XXXXXX"
    config = Utils.load_config()
    targets = get_targets_from_segment(config, segment_id)
    send_action_to_targets(config, targets, action_id)

if __name__ == '__main__':
    main()

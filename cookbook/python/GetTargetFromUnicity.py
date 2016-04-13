"""
Ce script permet de récupérer une Cible grace à un des ses caractères d'unicité.
"""

import requests
import Utils

def get_target(config, unicity):
    """Récupération de la cible"""
    url = config['API']['url'] + 'targets?unicity=' + unicity

    # Connexion à l'API
    headers = Utils.create_headers(config['API']['xKey'], 0)
    req = requests.get(url, headers=headers)
    req.raise_for_status()
    print(req.text)

def main():
    """
    Pensez à modifier le caractère d'unicité.
    """
    unicity = "test@test.com"
    config = Utils.load_config()
    get_target(config, unicity)

if __name__ == '__main__':
    main()

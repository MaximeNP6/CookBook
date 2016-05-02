"""
Ce script permet de créer un Segment puis importe un fichier dans celui ci.
"""

import json
import requests
import Utils

from CreateSegment import create_segment

def get_import_file():
    """
    Récupère le contenu du fichier puis l'ajoute à un JSON.
    """

    #
    # You've got to change the path
    #
    path = './'
    file_name = 'import.csv'
    with open(path + file_name) as import_file:
        import_data = import_file.read()

    import_json = {'data': import_data, 'name': file_name}

    return import_json

def create_import(config, segment_json):
    """
    Créer un Import et l'exécute directement.
    """
    with open('Models/import.json') as data_file:
        data_json = json.load(data_file)

    data_json['features'][0]['segmentId'] = segment_json['id']
    data = json.JSONEncoder().encode(data_json)

    print('Creation of import.')
    url = config['API']['url'] + 'imports/'
    headers = Utils.create_headers(config['API']['xKey'], len(data))
    req = requests.post(url, data=data, headers=headers)
    req.raise_for_status()

    print('Import has been Created.')

    import_id = json.loads(req.text)['id']

    # On execute l'import
    print('Execution of import.')
    url = url + str(import_id) + '/executions'
    data_json = get_import_file()
    data = json.JSONEncoder().encode(data_json)
    headers = Utils.create_headers(config['API']['xKey'], len(data))
    headers['Content-Type'] = 'application/octet-stream'
    headers['Content-Disposition'] = 'form-data; filename=' + data_json['name']
    req = requests.post(url, data=data, headers=headers)
    req.raise_for_status()
    print('Execution of import Done.')

def main():
    config = Utils.load_config()
    segment_json = create_segment(config)
    create_import(config, segment_json)

if __name__ == '__main__':
    main()

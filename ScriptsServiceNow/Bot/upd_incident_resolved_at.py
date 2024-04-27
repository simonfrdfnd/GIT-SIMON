import requests
import time

def get_incident(number):
    url = 'https://nexioprod.service-now.com/api/now/table/incident?sysparm_query=number={}&sysparm_limit=1'.format(number)
    
    # Faire la demande GET pour récupérer l'incident
    response = requests.get(url, auth=(user, pwd), headers=headers)

    if response.status_code == 200:
        incident_data = response.json()['result'][0]  # Accessing the first element of the list
        # Récupérer le champ sys_updated_on de l'incident
        sys_id = incident_data.get('sys_id')
        sys_updated_on = incident_data.get('sys_updated_on')
        resolved_at = incident_data.get('resolved_at')
        return [sys_id, sys_updated_on]
    else:
        print("Échec de la récupération de l'incident. Code d'erreur:", response.status_code)

def upd_incident(number):
    [sys_id, sys_updated_on] = get_incident(number)
    
    url = 'https://nexioprod.service-now.com/api/now/table/incident/{}'.format(sys_id)
    
    # Créer un dictionnaire pour la mise à jour de l'incident
    update_data = {
        "resolved_at": sys_updated_on
    }
    # Faire une demande POST pour mettre à jour l'incident
    post_response = requests.patch(url, auth=(user, pwd), headers=headers, json=update_data)

    # Vérifier si la mise à jour a réussi (code de statut 200)
    if post_response.status_code == 200:
        print("La mise à jour de l'incident a réussi.")
    else:
        print("Échec de la mise à jour de l'incident. Code d'erreur:", post_response.status_code)


# Paramètres de connexion à ServiceNow
user = 'simon.froidefond@nexiogroup.com'
pwd = 'Nexio171296SF'    
headers = {"Content-Type":"application/json","Accept":"application/json"}


upd_incident('INC0010430')
upd_incident('INC0011105')
upd_incident('INC0011106')
upd_incident('INC0012136')
upd_incident('INC0015999')
upd_incident('INC0017034')
upd_incident('INC0017136')
upd_incident('INC0017035')
upd_incident('INC0018111')
upd_incident('INC0018311')
upd_incident('INC0018274')
upd_incident('INC0018227')
upd_incident('INC0018286')
upd_incident('INC0018288')
upd_incident('INC0018585')
upd_incident('INC0019176')
upd_incident('INC0020272')
upd_incident('INC0019952')

           

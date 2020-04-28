# -*- coding: utf-8 -*-
"""
Created on Fri Apr 17 20:43:41 2020

@author: Roman
"""

# %%
import sys
import glob
import json
import yaml
import os
import ipaddress

# %%
workingDirectory = sys.argv[1]
if( workingDirectory[-1] != "/" ):
    workingDirectory = workingDirectory + "/"
    
# %%
resultDocument = dict()
resultDocument['env'] = []
resultDocument['machines'] = []
resultDocument['vlans'] = []
resultDocument['firewall-rules'] = []
resultDocument['machine-groups'] = []
resultDocument['redis'] = []
# %%
inventoryFiles = glob.glob(workingDirectory + "**/*.yaml", recursive=True)

# %%
for inventoryFile in inventoryFiles:
    with open(inventoryFile) as f:
        inventory = yaml.load(f)
        
        for key in resultDocument:
            inventoryField = inventory.get(key)
            if inventoryField is not None:
                resultDocument[key] += inventoryField
           
# %%
idx = 1
for firewallRule in resultDocument['firewall-rules']:
    firewallRule['id'] = idx
    idx += 1
                
# %%
networks = dict()

for vlan in resultDocument['vlans']:
    network = ipaddress.ip_network(vlan['cidr']).network_address
    network = str(network)
    networks[network] = vlan


# %%
for machine in resultDocument['machines']:
    for networkInterface in machine['network-interfaces']:
        network = networkInterface['ipv4-network']
        networkInterface['ipv4-cidr']= networks[network]['cidr']
        networkInterface['ipv4-vlan']= networks[network]['vlan']
        
# %%
resultPath = workingDirectory+"cmdb.json"

if os.path.exists(resultPath):
    os.remove(resultPath)        

with open(resultPath, 'w') as data_file:
    json.dump(resultDocument, data_file)
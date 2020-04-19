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

workingDirectory = sys.argv[1]
env = sys.argv[2]

# %%

if( workingDirectory[-1] != '/' ):
    workingDirectory = workingDirectory + "/"

# %%
jsons = glob.glob(workingDirectory + "*.json")


# %%

for jsonFile in jsons:
    results = []
    
    
    with open(jsonFile) as f:
        facts = json.load(f)
        
        resultItem = dict()
        resultItem['name'] = facts['ansible_nodename']
        resultItem['env'] = env
        resultItem['description'] = facts.get('cmdb_description')
        resultItem['vcpu'] = facts["ansible_processor_vcpus"]
        resultItem['memory'] = facts['ansible_memtotal_mb']
        resultItem['operating-system-class'] = facts['ansible_system']
        resultItem['operating-system-distribution'] = facts['ansible_distribution']
        resultItem['operating-system'] = facts['ansible_lsb']['description']
        resultItem['fqdn'] = facts['ansible_fqdn']
        resultItem['template'] = facts.get('cmdb_template')
        networkInterfaces = []
        
        for networkInterfaceName in facts['ansible_interfaces']:
            networkInterface = facts['ansible_' + networkInterfaceName]
            networkInterfaceType = networkInterface['type']
            if networkInterfaceType != 'ether':
                continue
            
            networkInterfaceResult = dict()
            networkInterfaceResult['name'] = networkInterface['device']
            networkInterfaceResult['ipv4-address'] = networkInterface['ipv4']['address']
            networkInterfaceResult['ipv4-netmask'] = networkInterface['ipv4']['netmask']
            networkInterfaceResult['ipv4-network'] = networkInterface['ipv4']['network']
            networkInterfaces.append(networkInterfaceResult)
            
        resultItem['network-interfaces'] = networkInterfaces
        
        dataVolumes = []
        
        for mount in facts['ansible_mounts']:
            dataVolume = dict()
            dataVolume['device'] = mount['device']
            dataVolume['size'] = str(round(mount['size_total'] / 1024/1024/1024 ,2)) + 'GB'
            dataVolume['mount'] = mount['mount']
            dataVolume['fstype'] = mount['fstype']
            dataVolumes.append(dataVolume)
            
        resultItem['data-volumes'] = dataVolumes
        results.append(resultItem)
        
    resultPath = workingDirectory+"machines.yaml"
   
    if os.path.exists(resultPath):
        os.remove(resultPath)        
    
    resultDocument = dict()
    resultDocument['machines']=results
    
    with open(resultPath, 'w') as yaml_file:
        yaml.dump(resultDocument, yaml_file, default_flow_style=False, sort_keys=False)

"""
    "name": "some nice name",
    "size": 156,
    "mount": "/data"

"""
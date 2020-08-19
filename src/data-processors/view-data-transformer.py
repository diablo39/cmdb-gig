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
import datetime
from itertools import chain
from itertools import groupby

# %%
workingDirectory = sys.argv[1]
if( workingDirectory[-1] != "/" ):
    workingDirectory = workingDirectory + "/"

# %%
outputDirectory = sys.argv[2]
if( outputDirectory[-1] != "/" ):
    outputDirectory = outputDirectory + "/"
    
# %% set result document structure
resultDocument = dict()
resultDocument['generated'] = datetime.datetime.utcnow().strftime("%Y-%m-%dT%H:%M:%SZ")
resultDocument['env'] = []
resultDocument['machines'] = []
resultDocument['vlans'] = []
resultDocument['firewall-rules'] = []
resultDocument['machine-groups'] = []
resultDocument['redis'] = []
resultDocument['load-balancers'] = []

# %% get all inventory files
inventoryFiles = glob.glob(workingDirectory + "**/*.yaml", recursive=True)

# %% merge all of inventory files
for inventoryFile in inventoryFiles:
    with open(inventoryFile) as f:
        inventory = yaml.load(f)
        
        for key in resultDocument:
            inventoryField = inventory.get(key)
            if inventoryField is not None:
                resultDocument[key] += inventoryField
           
# %% set autorincreemnt PK for network rules
idx = 1
for firewallRule in resultDocument['firewall-rules']:
    firewallRule['id'] = idx
    idx += 1
                
# %% index vlans by network & create list/details for vlans
vlansByNetwork = dict()
firewallhosts = dict()

for vlan in resultDocument['vlans']:
    network = ipaddress.ip_network(vlan['cidr']).network_address
    network = str(network)
    vlansByNetwork[network] = vlan
    vlan['machines'] = []
    vlan['outgoing-traffic'] = []
    vlan['incoming-traffic'] = []
    vlan['vlan'] = "{:0>3d}".format(vlan['vlan'])
    firewallhosts[vlan['cidr']] = "VLAN:" + vlan['vlan'] + " " + vlan['name']
    
# %% generate machine details
machinesList = []
machinesDetails = resultDocument['machines']
    
for machine in machinesDetails:
    
    machine['outgoing-traffic'] = []
    machine['incoming-traffic'] = []
    
    machineListItem = dict()
    machineListItem['name'] = machine.get('name')
    machineListItem['env'] = machine.get('env')
    machineListItem['group'] = machine.get('group')
    machineListItem['vcpu'] = machine.get('vcpu')
    machineListItem['memory'] = machine.get('memory')
    machineListItem['fqdn'] = machine.get('fqdn')
    machineListItem['description'] = machine.get('description')
    machineListItem['operating-system-distribution'] = machine.get('operating-system-distribution')
    machineListItem['operating-system-version'] = machine.get('operating-system-version')
    machineListItem['vlans'] = []
    machinesList.append(machineListItem)
        
    for networkInterface in machine['network-interfaces']:
        
        network = networkInterface['ipv4-network']
        firewallhosts[networkInterface['ipv4-address']] = machine.get('fqdn')
        if network in vlansByNetwork :
            currentVlan = vlansByNetwork[network]
            
            currentVlan['machines'].append(machineListItem)
            
            if(not networkInterface['ipv4-cidr']):
                networkInterface['ipv4-cidr'] = currentVlan['cidr']
            networkInterface['ipv4-vlan'] = currentVlan['vlan']
            machineListItem['vlans'].append(currentVlan['vlan'])
            # it will be nice to generate dict of firewall rules
            for firewallRule in resultDocument['firewall-rules']:
                if firewallRule['source-ipv4'] == networkInterface['ipv4-address']:
                    cp = firewallRule.copy()
                    cp['scope'] = 'IP'
                    machine['outgoing-traffic'].append(cp)
                    currentVlan['outgoing-traffic'].append(cp)
                if firewallRule['source-ipv4'] == networkInterface['ipv4-cidr']:
                    cp = firewallRule.copy()
                    cp['scope'] = 'VLAN'
                    machine['outgoing-traffic'].append(cp)
                    currentVlan['outgoing-traffic'].append(cp)
                if firewallRule['destination-ipv4'] == networkInterface['ipv4-address']:
                    cp = firewallRule.copy()
                    cp['scope'] = 'IP'
                    machine['incoming-traffic'].append(cp)
                    currentVlan['incoming-traffic'].append(cp)
                if firewallRule['destination-ipv4'] == networkInterface['ipv4-cidr']:
                    cp = firewallRule.copy()
                    cp['scope'] = 'VLAN'
                    machine['incoming-traffic'].append(cp)
                    currentVlan['incoming-traffic'].append(cp)
                    


resultDocument['machines'] = machinesList

# %% generate vlan list details

vlanDetails = []

for vlan in resultDocument['vlans']:
    vlanDetails.append(vlan.copy())
    vlan['machines-count'] = len(vlan['machines'])
    
    del vlan['machines']
    del vlan['outgoing-traffic']
    del vlan['incoming-traffic']

# %%

for lb in resultDocument['load-balancers'] :
    firewallhosts[lb['ip-v4']] = "LB: " + lb.get('name') if lb.get('name') else lb['ip-v4']

# %% process firewall rules
grouppedFirewallRules = []
rulesSortedBySource = sorted(resultDocument['firewall-rules'], key=lambda x: x['source-ipv4'])
                                
for source, sourceG in groupby(rulesSortedBySource, lambda x: x['source-ipv4'] ):
    rulesSortedByDestination = sorted(sourceG, key=lambda x: x['destination-ipv4'])
    for destication, destinationG in groupby(rulesSortedByDestination, lambda y: y['destination-ipv4']) :
        destinations = list(destinationG)
        destinationPorts = map(lambda x: str(x['destination-port']).split(','), destinations)
        destinationPorts = sorted(list(chain.from_iterable(destinationPorts)))
        destinationPortsString = ', '.join(destinationPorts)
        grouppedFirewallRules.append(
            {
                'source-ipv4': source,
                'source-host': firewallhosts.get(source),
                'source-env': destinations[0]['source-env'],
                'destination-ipv4':destication, 
                'destination-host':firewallhosts.get(destication),
                'destination-env': destinations[0]['destination-env'],
                'rules': destinations,
                'destination-ports': destinationPortsString
            })

resultDocument['firewall-rules'] = grouppedFirewallRules        

# %% save results
resultPath = outputDirectory+"cmdb.json"

if os.path.exists(resultPath):
    os.remove(resultPath)        

with open(resultPath, 'w') as data_file:
    json.dump(resultDocument, data_file)

# save machines details
machinesDirectory = outputDirectory + 'machines/'

if not os.path.exists(machinesDirectory):
    os.mkdir(machinesDirectory)

for machine in machinesDetails :
    machinePath = machinesDirectory + machine['fqdn'] + '.json'
    if os.path.exists(machinePath):
        os.remove(machinePath)        

    with open(machinePath, 'w') as data_file:
        json.dump(machine, data_file)        
        
# %% save vlan details

vlansDirectory = outputDirectory + 'vlans/'

if not os.path.exists(vlansDirectory):
    os.mkdir(vlansDirectory)
    
for vlan in vlanDetails :
    vlanPath = vlansDirectory + str(vlan['vlan']) + '.json'
    if os.path.exists(vlanPath):
        os.remove(vlanPath)        

    with open(vlanPath, 'w') as data_file:
        json.dump(vlan, data_file)   

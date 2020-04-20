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

# %%
workingDirectory = 'C:\\Sources\\Architecture Team\\cmdb' # sys.argv[1]
if( workingDirectory[-1] != os.path.sep ):
    workingDirectory = workingDirectory + os.path.sep
    
# %%
resultDocument = dict()
resultDocument['env'] = []
resultDocument['machines'] = []
resultDocument['vlans'] = []
resultDocument['firewall-rules'] = []
resultDocument['machine-templates'] = []
resultDocument['redis'] = []
# %%
inventoryFiles = glob.glob(workingDirectory + "**"+ os.path.sep + "*.yaml", recursive=True)

# %%
for inventoryFile in inventoryFiles:
    with open(inventoryFile) as f:
        inventory = yaml.load(f)
        
        for key in resultDocument:
            inventoryField = inventory.get(key)
            if inventoryField is not None:
                resultDocument[key] += inventoryField
                ddd='v'

# %%
resultPath = workingDirectory+"data.json"

if os.path.exists(resultPath):
    os.remove(resultPath)        

with open(resultPath, 'w') as data_file:
    json.dump(resultDocument, data_file)
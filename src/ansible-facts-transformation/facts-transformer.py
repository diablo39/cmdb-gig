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


# %%

if( workingDirectory[-1] != '/' ):
    workingDirectory = workingDirectory + "/"

# %%
jsons = glob.glob(workingDirectory + "*.json")


# %%

for jsonFile in jsons:
    results = []
    
    
    with open(jsonFile) as f:
        resultItem = dict()
        facts = json.load(f)
        resultItem['vcpu'] = facts["ansible_processor_vcpus"]
        results.append(resultItem)
        
    resultPath = workingDirectory+"machines.yaml"
   
    if os.path.exists(resultPath):
        os.remove(resultPath)        
    with open(resultPath, 'w') as yaml_file:
        yaml.dump(results, yaml_file)

"""
- name: DEV-APP-SERVERS00
  env: DEV
  template: 
  description: 
  vcpu: 4
  memory: 32
  operating-system-class: Linux
  operating-system: Ubuntu 18.10
"""
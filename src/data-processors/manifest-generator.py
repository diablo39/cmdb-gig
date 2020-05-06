# -*- coding: utf-8 -*-
"""
Created on Fri Apr 17 20:43:41 2020

@author: Roman
"""

# %%
import sys
import json
import os
from checksumdir import dirhash

# %%
  
workingDirectory = sys.argv[1]
if( workingDirectory[-1] != "/" ):
    workingDirectory = workingDirectory + "/"

# %%

hash = dirhash(workingDirectory, 'sha1')

# %%

result = dict()
result['hash'] =hash


resultPath = workingDirectory + "manifest.json"
if os.path.exists(resultPath):
    os.remove(resultPath)        

with open(resultPath, 'w') as data_file:
    json.dump(result, data_file)
# To add a new cell, type '# %%'
# To add a new markdown cell, type '# %% [markdown]'
# %%
import psutil 
import yaml


# %%
result = dict()


# %%
cpunum =psutil.cpu_count()
print(cpunum)

result['vcpu'] = cpunum


# %%
mem = psutil.virtual_memory()
result['memory']= round(mem.total/1024/1024/1024) #gb


# %%
addresses = psutil.net_if_addrs()


# %%
# for nic in addresses
#     nicinfo = dict()
#     nicinfo['name'] = nic


# %%
print(yaml.dump(result))



# %%

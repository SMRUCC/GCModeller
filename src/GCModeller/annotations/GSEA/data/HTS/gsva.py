import GCModeller

from gseakit import GSVA 
from visualkit import visualPlot

expr0    = read.csv(`${@dir}/metabolome.csv`, row.names = 1)
metainfo = read.csv(`${@dir}/metainfo.csv`, row.names = None)
metaSet  = read.csv(`${@dir}/kegg_enrichment.xls`, row.names = None, check.names = False, tsv = True)

print(expr0, max.print = 13)
print(metaSet, max.print = 13)
print(metainfo, max.print = 13)
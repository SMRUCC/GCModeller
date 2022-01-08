import GCModeller

from gseakit import GSVA 
from visualkit import visualPlot

expr0    = read.csv(`${@dir}/metabolome.csv`, row.names = 1)
metainfo = read.csv(`${@dir}/metainfo.csv`, row.names = None)

print(expr0, max.print = 13)
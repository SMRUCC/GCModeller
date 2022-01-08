import GCModeller

from gseakit import GSVA 
from visualkit import visualPlot
from phenotype_kit import geneExpression

expr0    = read.csv(`${@dir}/metabolome.csv`, row.names = 1)
metainfo = read.csv(`${@dir}/metainfo.csv`, row.names = None)
metaSet  = read.csv(`${@dir}/kegg_enrichment.xls`, row.names = None, check.names = False, tsv = True)
metaSet[, "names"] = None

metainfo[, "KEGG"] = unique.names(metainfo[, "KEGG"])
metainfo = as.list(metainfo, byrow = True)
names(metainfo, sapply(metainfo, r -> r$KEGG))
metainfo = lapply(metainfo, r -> r$name)

print(expr0, max.print = 13)
print(metaSet, max.print = 13)
# str(metainfo)

def getCluster(term):
    
    kegg_id = strsplit(term$compounds, ";")
    names = sapply(kegg_id, id -> metainfo[[id]])
    
    return names
    

metaSet = lapply(as.list(metaSet, byrow = True), r -> getCluster(term = r), names = r -> r$term)

# print("view of the background dataset:")
# str(metaSet)

scores = GSVA::gsva(expr0, metaSet)

print("preview of your gsva score matrix:")
print(as.data.frame(scores), max.print = 13)

write.expr_matrix(scores, file = `${@dir}/gsva.csv`, id = "pathway Name")
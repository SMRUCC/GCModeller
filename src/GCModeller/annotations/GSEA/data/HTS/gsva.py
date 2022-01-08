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
    
def run_gsva():

    metaSet = lapply(as.list(metaSet, byrow = True), r -> getCluster(term = r), names = r -> r$term)

    # print("view of the background dataset:")
    # str(metaSet)

    scores = GSVA::gsva(expr0, metaSet)

    print("preview of your gsva score matrix:")
    print(as.data.frame(scores), max.print = 13)

    write.expr_matrix(scores, file = `${@dir}/gsva.csv`, id = "pathway Name")

    return as.data.frame(scores)

gsva_score = run_gsva()

# test group diff
A01 = ["A01.1", "A01.2", "A01.3", "A01.4", "A01.5", "A01.6"]
CK  = ["CK1", "CK2", "CK3", "CK4", "CK5", "CK6"]

A01       = gsva_score[, A01]
CK        = gsva_score[, CK]
pathNames = rownames(gsva_score)

def diff(name, g1, g2):
    
    mean1  = mean(g1)
    mean2  = mean(g2)
    sd1    = sd(g1)
    sd2    = sd(g2)
    t      = as.object(t.test(g1, g2))
    pvalue = t$Pvalue
    t      = t$TestValue
    
    return (name, mean1, mean2, sd1, sd2, t, pvalue)    
    
compares   = lapply(1:nrow(gsva_score), i -> diff(pathNames[i], unlist(as.list(A01[i, ])), unlist(as.list(CK[i, ]))))
foldchange = sapply(compares, i -> (i$mean1) / (i$mean2))
log2fc     = log(foldchange, 2)
m1         = sapply(compares, i -> i$mean1)
m2         = sapply(compares, i -> i$mean2)
sd1        = sapply(compares, i -> i$sd1)
sd2        = sapply(compares, i -> i$sd2)
t          = sapply(compares, i -> i$t)
pvalue     = sapply(compares, i -> i$pvalue)
pathName   = sapply(compares, i -> i$name)

gsva_diff = data.frame(pathNames = pathName, mean_A01 = m1, mean_CK = m2, sd_A01 = sd1, sd_CK = sd2, foldchange = foldchange, log2fc = log2fc, t = t, pvalue = pvalue)

print(gsva_diff, max.print = 13)

write.csv(gsva_diff, file = `${@dir}/A01_vs_CK_gsva.csv`, row.names = False)



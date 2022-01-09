import GCModeller

from gseakit import GSVA 
from visualkit import visualPlot

data = read.csv(`${@dir}/A01_vs_CK_gsva.csv`, row.names = None)
data = data[data[, "t"] > 0 || data[, "t"] < -6, ]

print("previews of the gsva diff result between A01 and CK group:")
print(data, max.print = 6)

data = GSVA::matrix_to_diff(data, "pathNames", "t","pvalue")
plt = plot(data, padding = "padding: 100px 200px 200px 200px", size = [3300, 3600])

bitmap(plt, file = `${@dir}/A01_vs_CK.png`)


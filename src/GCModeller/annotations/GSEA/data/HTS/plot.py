import GCModeller

from gseakit import GSVA 
from visualkit import visualPlot

data = read.csv(`${@dir}/A01_vs_CK_gsva.csv`, row.names = None)

print("previews of the gsva diff result between A01 and CK group:")
print(data, max.print = 6)

data = GSVA::matrix_to_diff(data, "pathNames", "t","pvalue")

bitmap(plot(data), file = `${@dir}/A01_vs_CK.png`, size = [2100, 3700])


imports "multi_omics" from "phenotype_kit.dll";

setwd(!script$dir);

let n.genes as integer = 1000;
let geneId as string = sprintf("gene_%s", as.character(1:n.genes));
let x = runif(n.genes, -100, 100);
let y = runif(n.genes, -100, 100);

x <- lapply(1:n.genes, i -> x[i], names = i -> geneId[i]);
y <- lapply(1:n.genes, i -> y[i], names = i -> geneId[i]);

omics.2D_scatter(x,y)
:> save.graphics(file = "plot.png");

require(GCModeller);

imports "bioseq.patterns" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

setwd(@dir);

# read sequence data and found motif via gibbs scan method
let raw = read.fasta("../Staphylococcaceae_LexA___Staphylococcaceae.fasta");
let motif = gibbs_scan(raw, width = 18);

# draw sequence logo of the generated motif
bitmap(file = "LexA.png") {
    plot.seqLogo(motif, title = "LexA");
}
svg(file = "LexA.svg") {
    plot.seqLogo(motif, title = "LexA");
}
pdf(file = "LexA.pdf") {
    plot.seqLogo(motif, title = "LexA");
}

#cast motif data result as dataframe and export to table file
motif = as.data.frame(motif);
motif = motif[order(motif$score),];

print(motif, max.print = 13);

write.csv(motif, file = "LexA.csv");
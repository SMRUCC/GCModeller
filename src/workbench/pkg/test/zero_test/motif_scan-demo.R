require(GCModeller);

imports "bioseq.patterns" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";
imports "GenBank" from "seqtoolkit";
imports "genomics_context" from "seqtoolkit";

setwd(@dir);

# read sequence data and found motif via gibbs scan method
let raw = read.fasta("../Staphylococcaceae_LexA___Staphylococcaceae.fasta");
let motif = gibbs_scan(raw, width = 18);

# draw sequence logo of the generated motif
bitmap(file = "LexA.png") {
    plot(motif, title = "LexA");
}

let gb_asm = GenBank::read.genbank("G:\BlueprintCAD\demo\Escherichia coli str. K-12 substr. MG1655.gbff");
let nt = gb_asm |> TSS_upstream( upstream_len =150);

#cast motif data result as dataframe and export to table file
motif = as.data.frame(motif);
motif = motif[order(motif$score),];

print(motif, max.print = 13);

write.csv(motif, file = "LexA.csv");
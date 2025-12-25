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
let motif_sites = motif.find_sites(motif, nt, parallel = TRUE);

# cast motif site data result as dataframe and export to table file
motif_sites = as.data.frame(motif_sites);
motif_sites = motif_sites[order(motif_sites$identities, decreasing=TRUE),];

print(motif_sites, max.print = 13);

write.csv(motif_sites, file = "LexA_sites.csv");
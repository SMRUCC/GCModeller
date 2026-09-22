require(GCModeller);

imports "bioseq.patterns" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let raw = read.fasta("G:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\Staphylococcaceae_LexA___Staphylococcaceae.fasta");
let motif = gibbs_scan(raw, width = 18);

setwd(@dir);

bitmap(file = "./LexA.png") {
    plot.seqLogo(motif, title = "LexA");
}
svg(file = "./LexA.svg") {
    plot.seqLogo(motif, title = "LexA");
}
pdf(file = "./LexA.pdf") {
    plot.seqLogo(motif, title = "LexA");
}


motif 
|> JSON::json_encode()
|> writeLines(con = "./LexA.json")
;

motif = as.data.frame(motif);
motif = motif[order(motif$score),];

print(motif, max.print = 13);

write.csv(motif, file = "./LexA.csv");

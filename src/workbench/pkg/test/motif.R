require(GCModeller);

imports "bioseq.patterns" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let raw = read.fasta("G:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\Staphylococcaceae_LexA___Staphylococcaceae.fasta");
let motif = gibbs_scan(raw, width = 18);

pdf(file = "./LexA.pdf") {
    plot.seqLogo(motif, title = "LexA");
}

motif = as.data.frame(motif);
motif = motif[order(motif$score),];

print(motif);


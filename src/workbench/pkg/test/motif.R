require(GCModeller);

imports "bioseq.patterns" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let raw = read.fasta("G:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\Staphylococcaceae_LexA___Staphylococcaceae.fasta");
let motif = gibbs_scan(raw, width = 16);

print(as.data.frame(motif));
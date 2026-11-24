require(GCModeller);

imports "bioseq.blast" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let q = read.fasta( "F:\ecoli\29.0\data\protseq.fsa");
let s = read.fasta("G:\cell-render\data\KO.fasta");

diamond("Fast") 
|> diamond_search(q,s) 
|> write.csv(file = here("ecoli_ko.csv"))
;
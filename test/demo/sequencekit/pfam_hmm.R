require(GCModeller);

imports "hmmer" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let model = hmmer::load_hmmer("C:\Users\Administrator\Downloads\Pfam-A.hmm");
let seqs = read.fasta("N:\pan-genome\screen-env\proteins\Kocuria_varians.faa");
let result = hmmer::hmmer_search(model, seqs);

write.csv(result, file = "Z:/aa.csv");
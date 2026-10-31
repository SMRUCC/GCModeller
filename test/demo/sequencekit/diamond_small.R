require(GCModeller);

imports "bioseq.blast" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let q = read.fasta("G:/GCModeller/test/demo/sequencekit/tmp_ecoli_small.fsa");
let s = read.fasta("G:/GCModeller/test/demo/sequencekit/tmp_ko_small.fasta");

print(`queries: ${length(q)}`);
print(`subjects: ${length(s)}`);

diamond("Fast")
|> diamond_search(q, s)
|> write.csv(file = here("ecoli_ko_small.csv"))
;

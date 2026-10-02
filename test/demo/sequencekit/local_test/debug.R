require(GCModeller);

imports "hmmer" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let m = hmmer::parse_hmmer_model("G:/GCModeller/test/demo/sequencekit/local_test/models/test_models.hmm");
print(as.object(m)$Length);
print(as.object(m)$Name);

let models = hmmer::load_hmmer("G:/GCModeller/test/demo/sequencekit/local_test/models");
print(as.object(models)$ModelCount);

let seqs = read.fasta("G:/GCModeller/test/demo/sequencekit/local_test/proteins.faa");
print(length(seqs));

let result = hmmer::hmmer_search(models, seqs);
print(length(result));
print(result);

require(GCModeller);

imports "hmmer" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

# 1. load the hmmer model collection from a local directory
let models = hmmer::load_hmmer("G:/GCModeller/test/demo/sequencekit/local_test/models");

# 2. run the protein annotation
let result = hmmer::hmmer_search(models, read.fasta("G:/GCModeller/test/demo/sequencekit/local_test/proteins.faa"));
write.csv(result, file = "G:/GCModeller/test/demo/sequencekit/local_test/result1.csv");

# 3. save the model collection as a zip package archive
let package = hmmer::save_hmmer(models, "G:/GCModeller/test/demo/sequencekit/local_test/models.zip");
print(`model package saved at: ${package}`);

# 4. reload the model collection from the zip package archive
let models2 = hmmer::load_hmmer("G:/GCModeller/test/demo/sequencekit/local_test/models.zip");

# 5. run the protein annotation again with the reloaded model data
let result2 = hmmer::hmmer_search(models2, read.fasta("G:/GCModeller/test/demo/sequencekit/local_test/proteins.faa"));
write.csv(result2, file = "G:/GCModeller/test/demo/sequencekit/local_test/result2.csv");

print("local test done.");

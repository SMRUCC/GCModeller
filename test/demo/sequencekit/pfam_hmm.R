require(GCModeller);

imports "hmmer" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let model = hmmer::load_hmmer("C:\Users\Administrator\Downloads\Pfam-A.hmm");
let seqs = read.fasta("N:\pan-genome\screen-env\proteins\Kocuria_varians.faa");
let result = hmmer::hmmer_search(model, seqs);

write.csv(result, file = "Z:\aa.csv");

# ----------------------------------------------------------------
# regression test: save the hmmer model collection as a zip package
# archive, and then reload the model data from the zip package and
# run the protein annotation again for result consistency checking.
# ----------------------------------------------------------------

let package = "Z:\pfam_model_package.zip";
let saved   = hmmer::save_hmmer(model, package);

print(`hmmer model package saved at: ${saved}`);

let model2  = hmmer::load_hmmer(package);
let result2 = hmmer::hmmer_search(model2, seqs);

write.csv(result2, file = "Z:\aa_reload.csv");

print("reload search test done.");

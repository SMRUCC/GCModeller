require(GCModeller);

imports "bioseq.fasta" from "seqtoolkit";

print(MSA.of(["ACGT","ACG","AGT"]));
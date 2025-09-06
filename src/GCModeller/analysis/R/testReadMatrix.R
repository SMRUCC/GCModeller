require(GCModeller);

imports "bioseq.fasta" from "seqtoolkit";

let mat = read.fingerprint_bson("D:\datapool\nt_seed.dat");
let tree = make_clusterTree(mat);
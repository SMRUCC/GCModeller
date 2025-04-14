require(GCModeller);

imports "proteinKit" from "seqtoolkit";

let prot = read.pdb(gzfile("F:\pdb\pdb1htq.ent.gz", open = "open"));

print(prot);


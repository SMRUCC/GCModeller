require(GCModeller);

imports "proteinKit" from "seqtoolkit";

for(let filepath in list.files("F:\pdb\pdb", pattern = "*.gz")) {
    str(filepath);
}


let prot = read.pdb(gzfile("F:\pdb\pdb1htq.ent.gz", open = "open"));

print(prot);


require(GCModeller);

imports "proteinKit" from "seqtoolkit";

for(let filepath in list.files("F:\pdb\pdb", pattern = "*.gz")) {
    str(filepath);
    print(read.pdb(gzfile(filepath, open = "open")));
}



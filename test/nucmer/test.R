require(GCModeller);

imports "annotation.genomics" from "seqtoolkit";

print(read.nucmer(relative_work("alignments.delta.txt")));
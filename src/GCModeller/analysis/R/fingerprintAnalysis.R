require(GCModeller);

imports "bioseq.fasta" from "seqtoolkit";

let nt = open.fasta("D:\datapool\nt.fasta", read=TRUE);
let matrix = open.fingerprint_writer("D:\datapool\nt_seed.dat");

matrix 
|> write_fingerprint(seqs = nt, debug = 500000)
|> close()
;

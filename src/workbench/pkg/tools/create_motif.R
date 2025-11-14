require(GCModeller);

imports ["bioseq.patterns","bioseq.fasta"] from "seqtoolkit";

let seqfile = ?"--seq" || stop("missing the input sequence data!");
let width = ?"--width" || NULL;
let maxitr = ?"--maxitr" || 1000;
let savefile = ?"--out" || file.path(dirname(seqfile), `${basename(seqfile)}.xml`);

seqfile
|> read.fasta
|> gibbs_scan(width = width, 
    maxitr = maxitr)
|> xml
|> writeLines(con = savefile)
;

message(`motif file save to file location: ${savefile}`);
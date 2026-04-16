require(GCModeller);

imports ["bioseq.patterns","bioseq.fasta"] from "seqtoolkit";

let seqfile = ?"--seq" || stop("missing the input sequence data!");
let width = ?"--width" || NULL;
let maxitr = ?"--maxitr" || 1000;
let savefile = ?"--out" || file.path(dirname(seqfile), `${basename(seqfile)}.xml`);
let Rplot = file.path( dirname(savefile), `${basename(savefile)}.png`);
let motif = seqfile
|> read.fasta
|> gibbs_scan(width = width, 
    maxitr = maxitr)
;

writeLines(xml(motif),con = savefile);

bitmap(file = Rplot) {
    plot(motif, title = basename(seqfile));
}

message(`motif file save to file location: ${savefile}`);
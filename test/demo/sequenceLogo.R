require(GCModeller);
require(jsonlite);

# Demo script for create sequence logo based on the MSA alignment analysis
# nt base frequency is created based on the MSA alignment operation.

imports "bioseq.patterns" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

# script cli usage
#
# R# sequenceLogo.R --seq input.fasta [--title <logo.title> --save output.png] 
#

# get input data from commandline arguments and
# fix for the optional arguments default value
# by apply or default syntax for non-logical values
let seq.fasta as string = ?"--seq"   || stop("No sequence data provided!");
let workdir = dirname(seq.fasta);
let logo.png as string  = ?"--save"  || file.path(workdir, `${basename(seq.fasta)}-seqlogo.png`);
let title as string     = ?"--title" || basename(seq.fasta);

# read sequence and then do MSA alignment
# finally count the nucleotide base frequency
# and then draw the sequence logo
# by invoke sequence logo drawer api
let msa = seq.fasta
|> read.fasta
|> MSA.of
;

print(msa);

bitmap( file = logo.png ){
    plot.seqLogo(msa, title);
}

writeLines(jsonlite::toJSON(msa), con = file.path( workdir, "LexA-MSA.json"));
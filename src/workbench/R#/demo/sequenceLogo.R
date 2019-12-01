# Demo script for create sequence logo based on the MSA alignment analysis
# nt base frequency is created based on the MSA alignment operation.

imports "bioseq.sequenceLogo" from "seqtoolkit.dll";
imports "bioseq.fasta" from "seqtoolkit.dll";

# script cli usage
#
# R# sequenceLogo.R --seq input.fasta [--save output.png] 
#

let seq.fasta as string = ?"--seq";
let logo.png as string  = ?"--save";
let title as string     = ?"--title";

if (is.empty(logo.png)) {
	logo.png <- `${seq.fasta}.logo.png`;
}
if (is.empty(title)) {
	title <- basename(seq.fasta);
}

seq.fasta
  :> read.fasta
  :> MSA.of
  :> plot.seqLogo(title)
  :> save.graphics( file = logo.png );
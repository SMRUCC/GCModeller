imports "TRN.builder" from "phenotype_kit.dll";
imports "bioseq.fasta" from "seqtoolkit.dll";

let rawSeq <- ["P:\XCC\Regprecise\Db.Xml"]
:> read.regprecise
:> motif.raw
;

let familyNames as string = names(rawSeq);

print(familyNames);

for(family in familyNames) {
	rawSeq[[family]] :> write.fasta(file = `K:\20200226\TRN\motifs/${family}.fasta`);
}
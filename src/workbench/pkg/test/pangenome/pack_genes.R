require(GCModeller);

imports "GenBank" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let src = ?"--genbank" || stop("no genbank source was provided!");
let result = ?"--out" || file.path(src, "result");
let gbff = load_genbanks( list.files(src,"*.gbff" ));
let get_proteins = function(gb) {
    protein_seqs(gb, title = "<locus_tag>.<gb_asm_id>");
}

let targets = open.fasta(file.path(result,"proteins.faa"), read=  FALSE);

for(let gb in gbff) {
    write.fasta(get_proteins(gb), file = targets, filter.empty = TRUE);
}

close(targets);
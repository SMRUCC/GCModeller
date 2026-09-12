require(GCModeller);

imports "GenBank" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let src = ?"--genbank" || stop("no genbank source was provided!");
let result = ?"--out" || file.path(src, "result");
let gbff = load_genbanks( list.files(src,"*.gbff" ));
let table = c();
let get_proteins = function(gb) {
    protein_seqs(gb, title = "<locus_tag>.<gb_asm_id>");
}

let targets = open.fasta(file.path(result,"proteins.faa"), read=  FALSE);

for(let gb in gbff |> take(500)) {
    print(accession_id(gb));
    
    table = c(table, as_tabular(gb));
    write.fasta(get_proteins(gb), file = targets, filter.empty = TRUE);
}

write.csv(table, file = file.path(result,"genes.csv"));
close(targets);
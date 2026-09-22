require(GCModeller);

imports "GenBank" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

# step 1: prepare annotation data set

# R# ./pack_genes.R --genbank ./data

let src     = ?"--genbank" || stop("no genbank source was provided!");
let result  = ?"--out"     || file.path(src, "result");
let takes_n = ?"--takes"   || 10000;
let gbff    = load_genbanks( list.files(src,"*.gbff" ), extract_genomics = TRUE);
let table   = c();
let targets = open.fasta(file.path(result,"proteins.faa"), read=  FALSE);

for(let gb in gbff |> take(takes_n)) {
    print(accession_id(gb));

    # write protein sequence into the file stream
    table = c(table, as_tabular(gb));
    write.fasta(protein_seqs(gb, title = "<locus_tag>.<gb_asm_id>"), 
        file = targets, 
        filter.empty = TRUE);
}

# export gene table and flush the protein sequence stream
write.csv(table, file = file.path(result,"genes.csv"));
close(targets);
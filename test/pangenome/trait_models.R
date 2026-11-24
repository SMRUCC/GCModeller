require(GCModeller);

imports "GenBank" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let source = "N:\pan-genome\screen-env";

for(let type in c("bacteria","fungi")) {
    let dirs = list.dirs(file.path(source, type), recursive = FALSE, full.names = TRUE);

    for(let dir in dirs) {
        let gbff = list.files(dir, pattern = "*.gbff", full.names = TRUE, recursive = TRUE) 
        |> GenBank::load_genbanks();
        let targets = open.fasta(file.path(source ,"proteins", `${basename(dir)}.faa`), read=  FALSE);

        print(dir);

        for(let gb in gbff) {
            print(accession_id(gb));

            write.fasta(protein_seqs(gb, title = "<locus_tag> <species_name>"), 
                file = targets, 
                filter.empty = TRUE);
        }

        close(targets);
    }
}






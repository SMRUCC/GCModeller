require(GCModeller);

imports "kmers" from "metagenomics_kit";
imports "bioseq.fasta" from "seqtoolkit";

let repo = "U:\metagenomics_LLMs\demo\meta_pipeline_package\meta_pipeline_package\database\host_database";
let human_genome = read.fasta(file.path(repo, "GCF_000001405.40_GRCh38.p14_genomic.fna") );
let bloom_db = file.path(repo, "human_blooms");

for(let nt in human_genome) {
    let name = .Internal::first(unlist(strsplit([nt]::Title," ")));
    let filepath = file.path(bloom_db, `9606_${name}.ksbloom`);

    nt 
    |> as.bloom_filter(ncbi_taxid = 9606, k = 50, fpr = 0.00001)
    |> writeBin(con = filepath);
}
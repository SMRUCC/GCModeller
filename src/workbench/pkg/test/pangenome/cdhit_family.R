require(GCModeller);
require(jsonlite);

imports "bioseq.fasta" from "seqtoolkit";
imports "kmers" from "seqtoolkit";
imports "pangenome" from "comparative_toolkit";

let dir as string = ?"--dir" || stop("no analysis data provided!");
let family_json = file.path(dir, "cdhit-family.json");
let family_result = {
    if (file.exists(family_json )) {
        # use cache
        jsonlite::fromJSON(family_json , what = "cdhit-family");
    } else {
        let proteins = read.fasta(file.path(dir, "proteins.faa"));
        let cache_data = cdhit_clusters(proteins);

        writeLines(jsonlite::toJSON(cache_data$clusters), con = family_json );
        cache_data$clusters;
    }
};
let orth = multiple_genome_alignment( family_groups(family_result));
let geneset = read_genetable(file.path(dir,"genes.csv"));
let context = build_context(geneset,uniqueByAcc=TRUE);
let result = pangenome::analysis(context, orth);

writeBin(result, con = file.path(dir, "result.zip"));
result = readBin(con =file.path(dir, "result.zip"), what = "pangenome");

write.csv(genetic_distance(result), file = file.path(dir, "genetic_distance.csv"));
write.csv(pav_matrix(result), file = file.path(dir, "pav_matrix.csv"));
write.csv(curve_data(result), file = file.path(dir, "pangenome_curve.csv"));
write.csv(sv_table(result ), file = file.path(dir, "sv_table.csv" ));
write.csv(pav_table(result), file = file.path(dir,"pav_table.csv"));
writeLines(report_html(result), con = file.path(dir,"result.html"));

require(GCModeller);
require(jsonlite);

# step 2: pan-genome analysis and report

# R# ./cdhit_family.R --dir ./data/result

imports "bioseq.fasta" from "seqtoolkit";
imports "kmers" from "seqtoolkit";
imports "pangenome" from "comparative_toolkit";

let dir as string = ?"--dir" || stop("no analysis data provided!");
let family_json   = file.path(dir, "cdhit-family.json");
let family_result = {
    if (file.exists(family_json )) {
        # use cache
        jsonlite::fromJSON(family_json , what = "cdhit-family");
    } else {
        # make protein family clustering
        let proteins = read.fasta(file.path(dir, "proteins.faa"));
        let cache_data = cdhit_clusters(proteins, identities = 0.6);
        
        # write the cache data
        writeLines(jsonlite::toJSON(cache_data$clusters), 
            con = family_json );
        
        cache_data$clusters;
    }
};
# load gene annotation and gene family groups
# then run pan-genome analysis
let orth    = multiple_genome_alignment( family_groups(family_result));
let geneset = read_genetable(file.path(dir,"genes.csv"));
let context = build_context(geneset,soft_core_threshold = 0.8,genome_size = c(2000,8000), uniqueByAcc=TRUE);
let result  = pangenome::analysis(context, orth);

# save the analysis result
writeBin(result, con = file.path(dir, "result.zip"));

# export the result data files
let scatter = pangenome::scatter_set(result);

# genome gene counts table
write.csv(as.data.frame(scatter$stats), file = file.path(dir, "genome_stats.csv"));
# pav entropy PCA result
write.csv(as.data.frame(scatter$pca), file = file.path(dir, "pav_pca.csv"));
# pav entropy data
write.csv(as.data.frame(scatter$entropy), file = file.path(dir, "pav_entropy.csv"));

write.csv(genetic_distance(result), file = file.path(dir, "genetic_distance.csv"));
write.csv(pav_matrix(result), file = file.path(dir, "pav_matrix.csv"));
write.csv(curve_data(result), file = file.path(dir, "pangenome_curve.csv"));
write.csv(sv_table(result ), file = file.path(dir, "sv_table.csv" ));
write.csv(pav_table(result), file = file.path(dir,"pav_table.csv"));

# SV structural variation matrices: rows are gene families, columns are genomes
write.csv(sv_copy_number_matrix(result), file = file.path(dir, "sv_copy_number_matrix.csv"));
write.csv(sv_median_matrix(result), file = file.path(dir, "sv_median_matrix.csv"));

# gene family distribution percent matrix, by two calibers:
# "gene" = copy number based, "family" = family count based
write.csv(category_percent_matrix(result, by = "gene"), file = file.path(dir, "category_percent_gene.csv"));
write.csv(category_percent_matrix(result, by = "family"), file = file.path(dir, "category_percent_family.csv"));

# SV information entropy scatter data with the kmeans clustering result
write.csv(as.data.frame(sv_entropy(result)), file = file.path(dir, "sv_entropy.csv"));
# make export html report result
writeLines(report_html(result), con = file.path(dir,"result.html"));


#' Make motif site search in parallel
#' 
#' @param db the file path to the motif PWM database
#' @param seqs the file path to the TSS upstream site for make motif site matches
#' 
#' @return a dataframe of the motif site matches result
#' 
const scan_motifs = function(db, seqs, 
                             identities_cutoff = 0.8,
                             minW = 0.85,
                             top = 3,
                             permutation = 2500,             
                             workdir = "./", 
                             n_threads = 8) {

    imports "TRN.builder" from "TRNtoolkit";
    imports "Parallel" from "snowFall";
    imports "bioseq.fasta" from "seqtoolkit";

    let motifdb = TRN.builder::open_motifdb(db);
    let familyList = [motifdb]::FamilyList;

    close(motifdb );

    # run in parallel for production
    Parallel::parallel(family = familyList, n_threads = n_threads, 
            ignoreError = FALSE, 
            debug = FALSE,
            log_tmp = `${workdir}/.local_debug/`,
            compress = FALSE) {

        require(GCModeller);

        imports "TRN.builder" from "TRNtoolkit";
        imports "bioseq.fasta" from "seqtoolkit";

        let family_name <- unlist(family);
        let upstream <- read.fasta(unlist(seqs));
        let outputdir <- file.path(unlist(workdir), "results");

        # view verbose debug echo 
        print("get motif family name for make search processing:");
        print(` -> ${family_name}`);                  

        TRN.builder::open_motifdb(db)
        |> motif_search(
            search_regions = upstream,
            family = family_name,
            identities_cutoff = identities_cutoff,
            minW = minW,
            top = top,
            permutation = permutation,
            tqdm_bar = FALSE
        ) 
        |> unlist()
        |> write.csv(file = file.path(outputdir, `${normalizeFileName(family_name)}.csv`));
    }

    let tmp_results = list.files(file.path(workdir, "results"), pattern = "*.csv");
    tmp_results = as.list(tmp_results, names = basename(tmp_results));
    tmp_results = lapply(tmp_results, path => read.csv(path, row.names = NULL, check.names = FALSE));
    tmp_results = bind_rows(tmp_results);

    return(tmp_results);
}
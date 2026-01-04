#' Create Local KEGG Orthology (KO) Sequence Database
#'
#' This is the main entry function for creating a local KO sequence database repository.
#' It downloads KEGG Orthology data and associated gene sequences from the KEGG database
#' via the KEGG REST API and caches them locally.
#'
#' @param db A local repository connection to the target KEGG genes database for saving
#'   and caching data downloaded from the KEGG website. This parameter can be a local
#'   directory path or a file connection to an archive file with internal filesystem
#'   tree management (e.g., a ZIP archive, HDF5 archive, or R# HDS pack file).
#'   Default is \code{"./"} (current working directory).
#'
#' @param species A character vector of KEGG organism species codes for taxonomy-based
#'   filtering of genes to download. Default is \code{NULL}, which means no filtering
#'   is applied. For example, setting \code{species = c("eco", "hsa", "mmu")} will
#'   download only KO gene sequences for \emph{Escherichia coli}, \emph{Homo sapiens},
#'   and \emph{Mus musculus}, ignoring all other organisms.
#'
#' @param seqtype The type of FASTA sequence data to download from the KEGG database.
#'   Must be one of:
#'   \itemize{
#'     \item \code{"ntseq"} - gene nucleotide sequence (default)
#'     \item \code{"aaseq"} - protein amino acid sequence
#'   }
#'
#' @return This function returns \code{NULL} invisibly. The primary output is the
#'   creation of a local database repository containing KO index files and
#'   sequence FASTA files.
#'
#' @export
#'
#' @family ko_db_functions
#'
#' @examples
#' \dontrun{
#' # Create KO database with all organisms in current directory
#' ko_db()
#'
#' # Create KO database for specific species only
#' ko_db(db = "./kegg_db", species = c("eco", "hsa", "mmu"))
#'
#' # Create KO database with protein sequences
#' ko_db(seqtype = "aaseq")
#' }
#'
const ko_db = function(db = "./", species = NULL, seqtype = c("ntseq","aaseq")) {
    # config local cache dir for REnv::getHtml function
    options(http.cache_dir = "$ko_db.http_cache");
    # set global environment for the repository model
    set(globalenv(),"$ko_db.http_cache", db);

    db |> ko_db_worker(species,seqtype= seqtype);
}

#' Internal Worker Function for KO Database Creation
#'
#' This is an internal helper function that implements the core logic for creating
#' the KO sequence database. It iterates through all KO entries in the KO index
#' and triggers the download of associated gene sequences.
#'
#' @param db A local repository connection to the target KEGG genes database for
#'   saving and caching downloaded data. See \code{\link{ko_db}} for details.
#'
#' @param species A character vector of KEGG organism species codes for filtering
#'   genes to download. Default is \code{NULL} (no filtering). See
#'   \code{\link{ko_db}} for details and examples.
#'
#' @param seqtype The type of FASTA sequence data to download. Must be either
#'   \code{"ntseq"} (gene nucleotide sequence) or \code{"aaseq"} (protein amino
#'   acid sequence). Default is \code{"ntseq"}.
#'
#' @return Returns \code{NULL} invisibly.
#'
#' @keywords internal
#'
#' @family ko_db_functions
#'
const ko_db_worker = function(db, species, seqtype = c("ntseq","aaseq")) {
    let ko_index = load_ko_index(db);

    print("load ko index table for make request data:");
    print(ko_index, max.print = 13);

    if (length(species) > 0) {
        print("KO gene sequence database will be downloaded with a specific genome range:");
        print(species);
    }

    for(ko in as.list(ko_index,byrow=TRUE)) {
        db |> fetch_ko_data(ko_id = ko$ko, 
                            species = species, 
                            seqtype = seqtype
        );
    }

    invisible(NULL);
}

#' Fetch and Download Data for a Specific KO Entry
#'
#' An internal function that downloads gene ID associations for a specific
#' KEGG Orthology (KO) identifier and optionally filters them by species.
#'
#' @param db A local repository connection to the target KEGG genes database.
#'
#' @param ko_id A character string specifying the KEGG Orthology identifier
#'   (e.g., \code{"K00001"}).
#'
#' @param species A character vector of KEGG organism species codes for filtering.
#'   Default is \code{NULL} (no filtering). Only genes from the specified organisms
#'   will be downloaded.
#'
#' @param seqtype The type of FASTA sequence data to download. Must be either
#'   \code{"ntseq"} (gene nucleotide sequence) or \code{"aaseq"} (protein amino
#'   acid sequence). Default is \code{"ntseq"}.
#'
#' @return Returns \code{NULL} invisibly.
#'
#' @details
#' This function performs the following steps:
#' \enumerate{
#'   \item Checks if a cached gene ID list exists for the KO entry
#'   \item If not cached, downloads the gene ID list from the KEGG REST API
#'   \item Parses the gene IDs into species code and gene locus tag components
#'   \item Filters genes by the specified species (if provided)
#'   \item Triggers the download of FASTA sequences for the filtered genes
#' }
#'
#' The KEGG API returns gene associations in the format \code{ko:K00001\teco:b0001},
#' where the first column is the KO identifier and the second column is the
#' gene identifier (formatted as \code{species:gene_id}).
#'
#' @keywords internal
#'
#' @family ko_db_functions
#'
const fetch_ko_data = function(db, ko_id, species, seqtype = c("ntseq","aaseq")) {
    let geneId_file = `/ko/${ko_id}.txt`;
    let ko_genes = NULL;

    if (!file.exists(geneId_file, fs=db)) {
        let ko_genes = REnv::getHtml(`https://rest.kegg.jp/link/genes/${ko_id}`, interval = 3, filetype = "txt");

        # split each text line with white space
        # ko:K00001	dme:Dmel_CG3481
        # ko:K00001	der:6540581
        # ko:K00001	dse:6611292
        # ko:K00001	dsi:Dsimw501_GD23968
        # ko:K00001	dya:Dyak_GE10683
        #
        # first column is the KO id
        # use the second column as gene id
        ko_genes = ko_genes |> textlines() |> strsplit("\s+");
        ko_genes = ko_genes@{2};

        writeLines(ko_genes, con = file.allocate(geneId_file, fs = db));
    }        

    # split the kegg gene id by use `:` symbol as delimiter
    # eco:b0001
    # first column is the kegg organism species code
    # second column is the gene locus tag
    ko_genes = readLines(file.allocate(geneId_file, fs = db));
    ko_genes = strsplit(ko_genes,"[:]");
    ko_genes = data.frame(species = ko_genes@{1}, gene_id = ko_genes@{2});

    print("genes that belongs to KO group:");
    print(ko_genes, max.print = 6);

    if (length(species) > 0) {
        let i = ko_genes$species in species;

        if (sum(i) > 0) {
            ko_genes = ko_genes[i,];
        } else {
            ko_genes = NULL;
        }            
    }

    if (nrow(ko_genes) > 0) {
        db |> download_koseqs(ko_id, ko_genes,seqtype =seqtype );
    }

    invisible(NULL);
}

#' Download FASTA Sequences from KEGG Database
#'
#' Downloads FASTA sequences for genes associated with a specific KEGG Orthology (KO)
#' identifier from the KEGG REST API and saves them to the local database repository.
#'
#' @param db A local repository connection to the target KEGG genes database where
#'   the FASTA sequences will be saved.
#'
#' @param ko_id A character string specifying the KEGG Orthology identifier
#'   (e.g., \code{"K00001"}).
#'
#' @param ko_genes A data frame containing gene IDs that belong to the current KO
#'   group. Must contain the following columns:
#'   \describe{
#'     \item{species}{KEGG organism species code (e.g., \code{"eco"}, \code{"hsa"})}
#'     \item{gene_id}{Gene locus tag within the organism (e.g., \code{"b0001"})}
#'   }
#'
#' @param seqtype The type of FASTA sequence data to download. Must be one of:
#'   \itemize{
#'     \item \code{"ntseq"} - gene nucleotide sequence (default)
#'     \item \code{"aaseq"} - protein amino acid sequence
#'   }
#'
#' @return Returns \code{NULL} invisibly.
#'
#' @details
#' This function downloads gene sequences in batches of 20 genes per request to
#' avoid overwhelming the KEGG REST API. It checks if a cached FASTA file already
#' exists for the KO entry and skips download if present.
#'
#' The FASTA sequences are saved to \code{/fasta/\{ko_id\}.txt} within the
#' repository structure.
#'
#' @keywords internal
#'
#' @family ko_db_functions
#'
const download_koseqs = function(db, ko_id, ko_genes, seqtype = c("ntseq","aaseq")) {
    let seqfile = `/fasta/${ko_id}.txt`;

    # skip of the existed fasta sequence file
    if (!file.exists(seqfile, fs=db)) {
        let fasta = NULL;

        # split the ko_genes id character vector into multiple parts
        # each part has 20 elements
        ko_genes = sprintf("%s:%s", ko_genes$species, ko_genes$gene_id);
        ko_genes = split(ko_genes, size = 20);

        print("download gene sequence with data task blocks:");
        print(length(ko_genes));
        str(ko_genes);

        for(block in ko_genes) {
            let url = `https://rest.kegg.jp/get/${paste(block, sep="+")}/${seqtype}`;
            let seqs = REnv::getHtml(url, interval = 3, filetype = "txt");
            
            fasta = c(fasta, seqs);
        }

        writeLines(fasta, con = file.allocate(seqfile, fs = db));
    }

    invisible(NULL);
}

#' Load KO Index Table
#'
#' Loads the KEGG Orthology (KO) index table, which contains metadata for all
#' KO entries including KO IDs, gene names, and functional descriptions.
#'
#' @param db A local repository connection to the target KEGG genes database.
#'   The KO index table is cached at \code{/ko.csv} within this repository.
#'
#' @return A data frame with three columns:
#'   \describe{
#'     \item{ko}{KEGG Orthology identifier (e.g., \code{"K00001"}); also used as row names}
#'     \item{gene_names}{Comma-separated list of gene names associated with the KO}
#'     \item{description}{Functional description of the KO, including EC numbers when applicable}
#'   }
#'
#' @details
#' If the cached index file (\code{/ko.csv}) does not exist in the repository,
#' this function downloads the KO list from the KEGG REST API at
#' \url{https://rest.kegg.jp/list/ko} , parses the response, and saves it locally
#' for future use.
#'
#' The KEGG API returns data in the format:
#' \preformatted{
#' K00001\tE1.1.1.1, adh; alcohol dehydrogenase [EC:1.1.1.1]
#' K00002\tAKR1A1, adh; alcohol dehydrogenase (NADP+) [EC:1.1.1.2]
#' }
#'
#' This format is parsed to extract:
#' \enumerate{
#'   \item The KO ID (first element)
#'   \item Gene names (second element; may be empty)
#'   \item Functional description (third element)
#' }
#'
#' The 3-second interval between requests helps avoid overwhelming the KEGG API.
#'
#' @export
#'
#' @family ko_db_functions
#'
#' @examples
#' \dontrun{
#' # Load KO index from existing database
#' ko_index <- load_ko_index(db = "./kegg_db")
#' head(ko_index)
#' }
#'
const load_ko_index = function(db) {
    if (!file.exists("/ko.csv", fs = db)) {
        # cache table if table file is missing
        let index = REnv::getHtml("https://rest.kegg.jp/list/ko", interval = 3, filetype = "txt");
        let ko_index = textlines(index) 
            |> strsplit("(\t+)|(;\s+)") 
            |> tqdm() 
            |> lapply(t -> {
                # ko_id \t+ gene_name    ;\s+ description
                # 1     2   3            4    5
                # K00001	E1.1.1.1, adh; alcohol dehydrogenase [EC:1.1.1.1]
                # K00002	AKR1A1, adh; alcohol dehydrogenase (NADP+) [EC:1.1.1.2]
                # K00003	hom; homoserine dehydrogenase [EC:1.1.1.3]

                if (length(t) == 3) {
                    # this KO has no gene name
                    # due to the gene name is missing, so that
                    # only has 3 elements, gene name is empty
                    c(t[1],"",t[3]);
                } else {
                    # pick the 1,3,5
                    # 1. KO id
                    # 3. gene name
                    # 5. gene function text
                    t[c(1,3,5)];
                }
            })
            ;

        ko_index = data.frame(
            row.names = ko_index@{1},
            ko = ko_index@{1},
            gene_names = ko_index@{2},
            description = ko_index@{3}
        );

        print("request ko index from KEGG web:");
        print(ko_index, max.print = 6);

        write.csv(ko_index, file = file.allocate("/ko.csv", fs = db));
    }

    read.csv(file.allocate("/ko.csv", fs = db), 
        row.names = 1, 
        check.names = FALSE);
}
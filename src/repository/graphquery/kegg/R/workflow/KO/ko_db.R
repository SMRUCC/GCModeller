#' create ko database
#' 
#' @param species a character vector of the kegg organism species code for make taxonomy range filter of the gene to downloads.
#'    default is NULL means no filter. example as set species parameter to c("eco","hsa","mmu") means this function will just download
#'    the ko gene sequence of taxonomy Escherichia coli, Homo sapiens, Mus musculus, ignores other orgniams
#' 
#' @param db a local repository connection to the target kegg genes database to save and cache the data which is download from
#'    the kegg website via the kegg rest api. this parameter value could be a local directory or file connection to the archive file 
#'    which has internal filesystem tree management, example as connection to a zip archive file, HDF5 archive file or the R# HDS pack file.
#' @param seqtype the fasta sequence data type for download from the KEGG database, default is gene nucleotide sequence.
#' 
#' @return this function returns nothing
#' 
#' @remarks the main entry function for the workflow of create a local KO sequence database repository.
#' 
const ko_db = function(db = "./", species = NULL, seqtype = c("ntseq","aaseq")) {
    # config local cache dir for REnv::getHtml function
    options(http.cache_dir = "$ko_db.http_cache");
    # set global environment for the repository model
    set(globalenv(),"$ko_db.http_cache", db);

    db |> ko_db_worker(species,seqtype= seqtype);
}

#' An internal function for implements create ko database
#' 
#' @param species a character vector of the kegg organism species code for make taxonomy range filter of the gene to downloads.
#'    default is NULL means no filter. example as set species parameter to c("eco","hsa","mmu") means this function will just download
#'    the ko gene sequence of taxonomy Escherichia coli, Homo sapiens, Mus musculus, ignores other orgniams
#' 
#' @param db a local repository connection to the target kegg genes database to save and cache the data which is download from
#'    the kegg website via the kegg rest api. this parameter value could be a local directory or file connection to the archive file 
#'    which has internal filesystem tree management, example as connection to a zip archive file, HDF5 archive file or the R# HDS pack file.
#' 
#' @return this function returns nothing
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

#' An internal function for download data for a specific KO ID
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

#' download fasta sequence from kegg database
#' 
#' @param ko_id ko id
#' @param ko_genes a dataframee table of the gene id collection that belongs to current ko_id KEGG Orthology
#'     should contains two data fields:
#'     species - the kegg organism specicies code
#'     gene_id - the gene locus tag
#' @param seqtype the fasta sequence type to download from the kegg database, value could be
#'     ntseq for gene nucleotide sequence and aaseq for protein amino acid sequence.
#' 
#' @return this function returns nothing
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

#' load KO index table
#' 
#' @return a dataframe table with data fields:
#'    ko - the KEGG Orthology id
#'    gene_names - the kegg gene names
#'    description - the kegg gene function description text
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
require(HDS);

#' Load internal kegg compound repository
#' 
const kegg_compounds = function(rawList = FALSE) {
    const reference_db = system.file("data/kegg/kegg.zip", package = "GCModeller");
    
    # get the database file stream
    using file as HDS::openStream(
            file = .readZipStream(zipfile = reference_db),
            readonly = TRUE) {

        if (rawList) {
            __hds_compound_files(kegg_db = file);
        } else {
            # wrap a index object
            repository::index(__hds_compound_files(kegg_db = file));
        }
    }
}

#' get all kegg compound files inside a HDS stream
#' 
#' @param kegg_db the HDS package of the kegg reference database
#' 
#' @return a set of the kegg compound clr object models
#' 
const __hds_compound_files = function(kegg_db) {
    __hds_compound_dir(kegg_db,           "/Metabolism")
    |> append(__hds_compound_dir(kegg_db, "/Genetic Information Processing"))
    |> append(__hds_compound_dir(kegg_db, "/Environmental Information Processing"))
    |> append(__hds_compound_dir(kegg_db, "/Cellular Processes"))
    |> append(__hds_compound_dir(kegg_db, "/Organismal Systems"))
    |> append(__hds_compound_dir(kegg_db, "/Human Diseases"))
    |> append(__hds_compound_dir(kegg_db, "/Drug Development"))
    ;
}

const __hds_compound_dir = function(kegg_db, dir) {
    let files = HDS::files(kegg_db, dir = `/${dir}/`, recursive = TRUE);

    files = as.data.frame(files);
    files = files[files$type == "dir", ];
    files = files[basename(files$path) == "compounds", ];

    # get all compounds
    files = lapply(files$path, function(dir) {
        as.data.frame(HDS::files(kegg_db, dir = `${dir}/`, recursive = TRUE))$path;
    }) |> unlist();

    files = data.frame(files, name = basename(files));
    files = files 
    |> groupBy("name") 
    |> sapply(function(d) {
        const list = (d$files);

        if (length(list) == 1) {
            list;
        } else {
            .Internal::first(list);
        }
    });

    lapply(files, function(path) {
        try({
            loadXml(HDS::getText(kegg_db, fileName = path), typeof = "kegg_compound");
        });
    });
}
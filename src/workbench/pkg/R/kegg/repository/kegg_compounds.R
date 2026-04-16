require(HDS);

#' Load internal kegg compound repository
#' 
#' @param reference_set load the compound list which is the kegg pathway 
#'   map associated. set this parameter value to FALSE for load full list 
#'   of the kegg compounds.
#' 
const kegg_compounds = function(rawList = FALSE, reference_set = TRUE) {
    if (reference_set) {
        let reference_db = system.file("data/kegg/kegg.zip", package = "GCModeller");
        
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
    } else {
        let reference_db = system.file("data/kegg/KEGG_compound.msgpack", package = "GCModeller");
        let pack = readBin(reference_db, what = "kegg_compound");

        pack;
    }
}

#' get all kegg compound files inside a HDS stream
#' 
#' @param kegg_db the HDS package of the kegg reference database
#' 
#' @return a set of the kegg compound clr object models
#' 
const __hds_compound_files = function(kegg_db, flag = "compounds", what = "kegg_compound") {
    __hds_compound_dir(kegg_db,           flag = flag, what = what, dir = "/Metabolism")
    |> append(__hds_compound_dir(kegg_db, flag = flag, what = what, dir = "/Genetic Information Processing"))
    |> append(__hds_compound_dir(kegg_db, flag = flag, what = what, dir = "/Environmental Information Processing"))
    |> append(__hds_compound_dir(kegg_db, flag = flag, what = what, dir = "/Cellular Processes"))
    |> append(__hds_compound_dir(kegg_db, flag = flag, what = what, dir = "/Organismal Systems"))
    |> append(__hds_compound_dir(kegg_db, flag = flag, what = what, dir = "/Human Diseases"))
    |> append(__hds_compound_dir(kegg_db, flag = flag, what = what, dir = "/Drug Development"))
    |> which(x -> !is.null(x))
    ;
}

const __hds_compound_dir = function(kegg_db, dir, flag = "compounds", what = "kegg_compound") {
    let files = HDS::files(kegg_db, dir = `/${dir}/`, recursive = TRUE);

    files = as.data.frame(files);
    files = files[files$type == "dir", ];
    files = files[basename(files$path) == flag, ];

    # get all compounds
    files = lapply(files$path, dir -> as.data.frame(HDS::files(kegg_db, dir = `${dir}/`, recursive = TRUE))$path);
    files = unlist(files);

    files = data.frame(files, name = basename(files));
    files 
    |> groupBy("name") 
    |> sapply(d -> .Internal::first(d$files))
    |> sapply(path -> try({
            loadXml(HDS::getText(kegg_db, fileName = path), typeof = what);
        })
    )
    ;
}
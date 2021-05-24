imports "brite" from "kegg_kit";

const pathway_category as function() {
    brite::parse("br08901");
}

const enumeratePath as function(brite, prefix, maxChars = 64) {
    const class    = brite[, "class"];
    const category = brite[, "category"]
        |> sapply(function(str) {
            if (nchar(str) > maxChars) {
                `${substr(str, 1, maxChars)}~`;
            } else {
                str;
            }
        })
        |> gsub(":", ",")
        ;
    const subcategory = brite[, "subcategory"];
    const order       = brite[, "order"];
    const id          = brite[, "entry"];
  
    if (all( is.null(subcategory))) {
        function(i) {
            `${class[i]}/${category[i]}/${prefix}${id[i]}`;
        }
    } else {
        stop("not yet implemented!");
    } 
}
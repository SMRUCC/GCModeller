imports "brite" from "kegg_kit";

#' get brite category data for KEGG pathway
#'
const pathway_category as function() {
  brite::parse("br08901");
}

#' Create resource file path
#' 
#' @param brite a dataframe object that created from the ``brite::parse``
#'    function.
#'
const enumeratePath as function(brite, prefix = "", maxChars = 64) {
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
    return(function(i) {
      `${class[i]}/${category[i]}/${prefix}${id[i]}`;
    });
  } else {
    return(function(i) {
      `${class[i]}/${category[i]}/${subcategory[i]}/${order[i]}/${prefix}${id[i]}`;
    });
  }
}

#' get reaction class category data 
#' 
const reactionclass_category as function() {
  brite::parse("br08204");
}
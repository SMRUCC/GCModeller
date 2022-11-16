imports "brite" from "kegg_kit";

#' get brite category data for KEGG pathway
#' 
#' @details the pathway category data is parse from the
#'    ``br08901`` category dataset.
#'
const pathway_category = function() {
  brite::parse("br08901");
}

#' brite list of kegg compounds
#' 
#' @details this function returns a list of 
#'    brite data of the kegg compounds
#' 
const compound_brites = function() {
  list(
    "Compounds with biological roles"          = brite::parse("br08001"),
    "Lipids"                                   = brite::parse("br08002"),
    "Phytochemical compounds"                  = brite::parse("br08003"),
    "Bioactive peptides"                       = brite::parse("br08005"),
    "Endocrine disrupting compounds"           = brite::parse("br08006"),
    "Pesticides"                               = brite::parse("br08007"),
    "Carcinogens"                              = brite::parse("br08008"),
    "Natural toxins"                           = brite::parse("br08009"),
    "Target-based classification of compounds" = brite::parse("br08010"),
    "Glycosides"                               = brite::parse("br08021")
  );
}

const placeNULL = function(v) {
  sapply(v, function(str) {
    if (is.null(str) || is.na(str)) {
      "";
    } else {
      if ((str == "N/A") ||(str == "n/a")) {
        "";
      } else {
        str;
      }
    }
  });
}

#' get reaction class category data 
#' 
const reactionclass_category = function() {
  brite::parse("br08204");
}

#' get reaction category data
#' 
const reaction_category = function() {
  brite::parse("br08201");
}
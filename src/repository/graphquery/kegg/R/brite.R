imports "brite" from "kegg_kit";

#' get brite category data for KEGG pathway
#'
const pathway_category as function() {
  brite::parse("br08901");
}

#' brite list of kegg compounds
#' 
#' @details this function returns a list of 
#'    brite data of the kegg compounds
#' 
const compound_brites as function() {
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

#' Create resource file path
#' 
#' @param brite a dataframe object that created from the ``brite::parse``
#'    function.
#'
const enumeratePath as function(brite, prefix = "", maxChars = 64) {
  const class        = brite[, "class"]       |> placeNULL();
  const category     = brite[, "category"]    |> placeNULL() |> trimLongName(maxChars);
  const subcategory  = brite[, "subcategory"] |> placeNULL() |> trimLongName(maxChars);
  const order        = brite[, "order"]       |> placeNULL();
  const id as string = brite[, "entry"];

  print("get all kegg class category maps:");
  print(dim(brite));
  print(head(brite));

  print("id prefix:");
  print(prefix);

  print("class data:");
  str(class);
  print("category data:");
  str(category);
  print("sub_category data:");
  str(subcategory);
  print("order data:");
  str(order);
  print("entry id data:");
  str(id);

  if (is.null(subcategory) || all(is.null(subcategory)) || subcategory == "") {
    print("path enumerate by class/category.");
    return(function(i) {
      `${class[i]}/${category[i]}/${prefix}${id[i]}`;
    });
  } 
  
  if (is.null(order) || all(is.null(order)) || order == "") {
    print("path enumerate by class/category/subcategory.");
    return(function(i) {
      `${class[i]}/${category[i]}/${subcategory[i]}/${prefix}${id[i]}`;
    });
  }

  print("path enumerate by class/category/subcategory/order.");
  return(function(i) {
    `${class[i]}/${category[i]}/${subcategory[i]}/${order[i]}/${prefix}${id[i]}`;
  });
}

#' try to make the long name shorter
#' 
#' @details the name in long length will caused the filesystem errors
#'     on windows system.
#' 
const trimLongName as function(longNames as string, maxChars = 64) {
  if (all(is.null(longNames))) {
    NULL;
  } else {
    longNames
    |> sapply(function(str) {
      if (nchar(str) > maxChars) {
        `${substr(str, 1, maxChars)}~`;
      } else {
        str;
      }
    })
    |> gsub(":", ",")
    ;
  }
}

#' get reaction class category data 
#' 
const reactionclass_category as function() {
  brite::parse("br08204");
}

#' get reaction category data
#' 
const reaction_category as function() {
  brite::parse("br08201");
}
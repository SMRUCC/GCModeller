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
    "" = brite::parse("br08901")
  );
}

#' Create resource file path
#' 
#' @param brite a dataframe object that created from the ``brite::parse``
#'    function.
#'
const enumeratePath as function(brite, prefix = "", maxChars = 64) {
  const class        = brite[, "class"];
  const category     = brite[, "category"]    |> trimLongName(maxChars);
  const subcategory  = brite[, "subcategory"] |> trimLongName(maxChars);
  const order        = brite[, "order"];
  const id as string = brite[, "entry"];

  print("get all kegg class category maps:");
  str(brite);

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

const trimLongName as function(longNames as string, maxChars = 64) {
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

#' get reaction class category data 
#' 
const reactionclass_category as function() {
  brite::parse("br08204");
}
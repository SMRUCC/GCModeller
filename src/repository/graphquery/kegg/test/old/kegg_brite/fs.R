
#' Create resource file path
#' 
#' @param brite a dataframe object that created from the ``brite::parse``
#'    function.
#'
const enumeratePath = function(brite, prefix = "", maxChars = 64) {
  const class        = brite[, "class"]       |> placeNULL();
  const category     = brite[, "category"]    |> placeNULL() |> trimLongName(maxChars);
  const subcategory  = brite[, "subcategory"] |> placeNULL() |> trimLongName(maxChars);
  const order        = brite[, "order"]       |> placeNULL() |> trimLongName(maxChars);;
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
    return(function(i) `${class[i]}/${category[i]}/${prefix}${id[i]}`);
  } 
  
  if (is.null(order) || all(is.null(order)) || order == "") {
    print("path enumerate by class/category/subcategory.");
    return(function(i) `${class[i]}/${category[i]}/${subcategory[i]}/${prefix}${id[i]}`);
  }

  print("path enumerate by class/category/subcategory/order.");
  return(function(i) `${class[i]}/${category[i]}/${subcategory[i]}/${order[i]}/${prefix}${id[i]}`);
}

#' try to make the long name shorter
#' 
#' @details the name in long length will caused the filesystem errors
#'     on windows system.
#' 
const trimLongName = function(longNames as string, maxChars = 32) {
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
    |> gsub(":", "-")
    |> gsub("\s*[/\\]\s*", "+", regexp = TRUE)
    ;
  }
}

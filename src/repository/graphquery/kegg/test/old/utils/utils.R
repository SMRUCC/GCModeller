imports "graphquery" from "webKit";

#' get graphquery from internal resource file
#'
#' @param ref the relative file name of the internal graphquery file
#'
const get_graph = function(ref) {
  const resource as string = system.file(ref, package = "kegg_graphquery");

  graphquery::parseQuery(
    readText(con = resource)
  );
}

#' create indexed list from a key-value pair collection
#'
const keyIndex = function(keyValues) {
  keyValues
  |> groupBy(r -> r$key)
  |> lapply(function(group) {
    if (group$key in ["Reference", "Authors", "Title", "Journal"]) {
      lapply(group, a -> a$content);
    } else {
      # [name => html]
      group[1]$content;
    }
  })
  ;
}

#' Does the given string is null or its string value represent null?
#'
#' @param str a string value to test
#'
const isNullString = function(str) {
  is.null(str) || (str in ["", "null", "NULL", "-"]);
}

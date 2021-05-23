imports "graphquery" from "webKit";

#' get graphquery from internal resource file
#'
const get_graph as function(ref) {
  const resource as string = system.file(ref, package = "kegg_graphquery");

  graphquery::parseQuery(
    readText(con = resource)
  );
}

#' create indexed list from a key-value pair collection
#'
const keyIndex as function(keyValues) {
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

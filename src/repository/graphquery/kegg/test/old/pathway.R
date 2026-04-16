imports "repository" from "kegg_kit";

#' Parse organism kegg code
#'
#' @return this function returns a list with ``code`` and ``name``.
#'
const parseKeggCode = function(name) {
  if (name == "") {
    list();
  } else {
    let kegg_code = $"\[.+:[a-z]{3,}\]"(name);

    kegg_code = substr(kegg_code, 2, nchar(kegg_code) - 1);
    kegg_code = strsplit(kegg_code, ":")[2];

    list(
      code = kegg_code,
      name = name
    );
  }
}

#' Create a literature dataframe
#'
const literature = function(reference, authors, title, journal) {
  const text_query = get_graph("graphquery/fields/text.graphquery");
  const getText = function(str) {
    graphquery::query(
      document   = Html::parse(str),
      graphquery = text_query
    )
    ;
  }

  data.frame(
    reference = sapply(reference, getText),
    authors   = sapply(authors,   getText),
    title     = sapply(title,     getText),
    journal   = sapply(journal,   getText)
  );
}

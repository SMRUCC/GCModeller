imports "repository" from "kegg_kit";

#' Query of kegg reaction data
#'
#' @param url the resource url on remote server or local file path for debug
#'
const kegg_reaction as function(url) {
  # parse the page text
  const keyValues = keyIndex(http_query(url, raw = FALSE));

  const id          = graphquery::query(document = Html::parse(keyValues$"Entry"),       graphquery = get_graph("graphquery/fields/simpleText.graphquery"));
  const commonNames = graphquery::query(document = Html::parse(keyValues$"Name"),        graphquery = get_graph("graphquery/fields/commonNames.graphquery"))
  |> strsplit("\r|\n")
  |> trim("; ")
  ;
  const info     = graphquery::query(document = Html::parse(keyValues$"Definition"),     graphquery = get_graph("graphquery/fields/text.graphquery"));
  const equation = graphquery::query(document = Html::parse(keyValues$"Equation"),       graphquery = get_graph("graphquery/fields/text.graphquery"));
  const comments = graphquery::query(document = Html::parse(keyValues$"Comment"),        graphquery = get_graph("graphquery/fields/text.graphquery"));
  const classes  = graphquery::query(document = Html::parse(keyValues$"Reaction class"), graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const enzyme   = graphquery::query(document = Html::parse(keyValues$"Enzyme"),         graphquery = get_graph("graphquery/fields/reactionLink.graphquery"));
  const pathways = graphquery::query(document = Html::parse(keyValues$"Pathway"),        graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const modules  = graphquery::query(document = Html::parse(keyValues$"Module"),         graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const KO       = graphquery::query(document = Html::parse(keyValues$"Orthology"),      graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const xref     = graphquery::query(document = Html::parse(keyValues$"Other DBs"),      graphquery = get_graph("graphquery/fields/dbLinks.graphquery"));

  print(DBLinks(xref));
  print(pathwayList(pathways));
  print(pathwayList(modules));
  print(pathwayList(KO));
  print(pathwayList(classes));

  if (isNullString(id)) {
    NULL;
  } else {
    repository::reaction(
      id             = id,
      name           = commonNames,
      definition     = info,
      equation       = equation,
      comment        = comments,
      reaction_class = pathwayList(classes),
      enzyme         = enzyme[enzyme  == $"\d[.].+"],
      pathways       = pathwayList(pathways),
      modules        = pathwayList(modules),
      KO             = pathwayList(KO),
      links          = DBLinks(xref)
    );
  }
}

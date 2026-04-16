imports "repository" from "kegg_kit";

#' Query of reaction class information
#'
#' @param url the resource url on remote server or local file path for debug
#'
const kegg_reactionclass as function(url) {
  # parse the page text
  const keyValues = keyIndex(http_query(url, raw = FALSE));

  const id         = graphquery::query(document = Html::parse(keyValues$"Entry"),         graphquery = get_graph("graphquery/fields/simpleText.graphquery"));
  const info       = graphquery::query(document = Html::parse(keyValues$"Definition"),    graphquery = get_graph("graphquery/fields/text.graphquery"));
  const reactionId = graphquery::query(document = Html::parse(keyValues$"Reaction"),      graphquery = get_graph("graphquery/fields/reactionLink.graphquery"));
  const EC_idlist  = graphquery::query(document = Html::parse(keyValues$"Enzyme"),        graphquery = get_graph("graphquery/fields/reactionLink.graphquery"));
  const pathways   = graphquery::query(document = Html::parse(keyValues$"Pathway"),       graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const KO         = graphquery::query(document = Html::parse(keyValues$"Orthology"),     graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const rmodules   = graphquery::query(document = Html::parse(keyValues$"RModule"),       graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const transforms = graphquery::query(document = Html::parse(keyValues$"Reactant pair"), graphquery = get_graph("graphquery/fields/reactionLink.graphquery"))
  |> which(r -> r == $"C\d+_C\d+")
  ;

  if (isNullString(id)) {
    NULL;
  } else {
    repository::reaction_class(
      id         = id,
      definition = info,
      reactions  = reactionId[reactionId == $"R\d+"],
      enzyme     = EC_idlist[EC_idlist  == $"\d[.].+"],
      pathways   = pathwayList(pathways),
      KO         = pathwayList(KO),
      transforms = compoundTransformPair(transforms),
      rmodules   = pathwayList(rmodules)
    );
  }
}

const compoundTransformPair as function(transforms) {
  const tuples = lapply(transforms, function(pair) {
    strsplit(pair, "_");
  });

  data.frame(
    from = sapply(tuples, t -> t[1]),
    to   = sapply(tuples, t -> t[2])
  );
}

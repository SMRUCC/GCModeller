imports "repository" from "kegg_kit";


const kegg_pathway as function(url) {
  # parse the page text
  const keyValues = keyIndex(http_query(url, raw = FALSE));
  str(keyValues);
  const id         = graphquery::query(document = Html::parse(keyValues$"Entry"),       graphquery = get_graph("graphquery/fields/simpleText.graphquery"));
  const commonName = graphquery::query(document = Html::parse(keyValues$"Name"),        graphquery = get_graph("graphquery/fields/text.graphquery"));
  const modules    = graphquery::query(document = Html::parse(keyValues$"Module"),      graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const info       = graphquery::query(document = Html::parse(keyValues$"Description"), graphquery = get_graph("graphquery/fields/text.graphquery"));

  print(pathwayList(modules));



  repository::pathway(
    id          = id,
    name        = commonName,
    modules     = pathwayList(modules),
    description = info
  );
}

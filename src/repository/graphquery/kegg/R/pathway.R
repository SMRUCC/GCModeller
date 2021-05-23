imports "repository" from "kegg_kit";


const kegg_pathway as function(url) {
  # parse the page text
  const keyValues = keyIndex(http_query(url, raw = FALSE));

const id          = graphquery::query(document = Html::parse(keyValues$"Entry"),      graphquery = get_graph("graphquery/fields/simpleText.graphquery"));

  str(keyValues);

  repository::pathway(
      id = id

  );
}
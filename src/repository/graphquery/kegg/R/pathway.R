imports "repository" from "kegg_kit";


const kegg_pathway as function(url) {
  # parse the page text
  const keyValues = keyIndex(http_query(url, raw = FALSE));

const id          = graphquery::query(document = Html::parse(keyValues$"Entry"),      graphquery = get_graph("graphquery/fields/simpleText.graphquery"));
  const commonName = graphquery::query(document = Html::parse(keyValues$"Name"),       graphquery = get_graph("graphquery/fields/text.graphquery"));
 const modules     = graphquery::query(document = Html::parse(keyValues$"Module"),     graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));


 print(pathwayList(modules));

  str(keyValues);

  repository::pathway(
      id = id,
name = commonName,
modules = pathwayList(modules)
  );
}
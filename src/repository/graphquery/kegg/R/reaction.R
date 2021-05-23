imports "repository" from "kegg_kit";

const kegg_reaction as function(url) {
  # parse the page text
  const keyValues = keyIndex(http_query(url, raw = FALSE));
}
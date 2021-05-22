#' query data to gcmodeller object model 
#'
const kegg_compound as function(url) {
	const keyValues = keyIndex(http_query(url, raw = FALSE));
	const xref      = graphquery::query(document = Html::parse(keyValues$"Other DBs"), graphquery = get_graph("graphquery/fields/dbLinks.graphquery"));
	
	print(xref);
}



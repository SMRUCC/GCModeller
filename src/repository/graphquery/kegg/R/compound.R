#' query data to gcmodeller object model 
#'
const kegg_compound as function(url) {
	const keyValues = keyIndex(http_query(url, raw = FALSE));
	const xref      = graphquery::query(document = Html::parse(keyValues$"Other DBs"), graphquery = get_graph("graphquery/fields/dbLinks.graphquery"));
	const KCF_text  = graphquery::query(document = Html::parse(keyValues$"KCF data"),  graphquery = get_graph("graphquery/fields/KCFtext.graphquery"));

	print(names(keyValues));

	print(KCF_text);

	# print(json_encode(xref));
}



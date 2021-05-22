#' query data to gcmodeller object model 
#'
const kegg_compound as function(url) {
	const keyValues   = keyIndex(http_query(url, raw = FALSE));
	const xref        = graphquery::query(document = Html::parse(keyValues$"Other DBs"),  graphquery = get_graph("graphquery/fields/dbLinks.graphquery"));
	const KCF_text    = graphquery::query(document = Html::parse(keyValues$"KCF data"),   graphquery = get_graph("graphquery/fields/KCFtext.graphquery"));
	const pathways    = graphquery::query(document = Html::parse(keyValues$"Pathway"),    graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
	const modules     = graphquery::query(document = Html::parse(keyValues$"Module"),     graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
	const reactionId  = graphquery::query(document = Html::parse(keyValues$"Reaction"),   graphquery = get_graph("graphquery/fields/reactionLink.graphquery"));
	const commonNames = graphquery::query(document = Html::parse(keyValues$"Name"),       graphquery = get_graph("graphquery/fields/commonNames.graphquery"));
	const id          = graphquery::query(document = Html::parse(keyValues$"Entry"),      graphquery = get_graph("graphquery/fields/simpleText.graphquery"));
	const formula     = graphquery::query(document = Html::parse(keyValues$"Formula"),    graphquery = get_graph("graphquery/fields/text.graphquery"));
	const exactMass   = graphquery::query(document = Html::parse(keyValues$"Exact mass"), graphquery = get_graph("graphquery/fields/text.graphquery"));
	const remarks     = graphquery::query(document = Html::parse(keyValues$"Remark"),     graphquery = get_graph("graphquery/fields/text.graphquery"));
	const EC_idlist   = graphquery::query(document = Html::parse(keyValues$"Enzyme"),     graphquery = get_graph("graphquery/fields/reactionLink.graphquery"));

	# reactionId = reactionId[reactionId == $"R\d+"];
	# EC_idlist  = EC_idlist[EC_idlist  == $"\d[.]+"];

	print(names(keyValues));
print(id);
print(commonNames);
print(formula);
print(exactMass);
print(remarks);
print(EC_idlist);

}



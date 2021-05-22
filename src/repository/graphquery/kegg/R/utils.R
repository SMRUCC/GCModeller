imports "graphquery" from "webKit";

#' get graphquery from internal resource file
#'
const get_graph as function(ref) {
    graphquery::parseQuery(
		readText(system.file(ref, package = "kegg_graphquery"))
	);
}

#' create indexed list from a key-value pair collection
#'
const keyIndex as function(keyValues) {
	const list  = lapply(keyValues, item -> item$content);
	names(list) = sapply(keyValues, item -> item$key);
	
	# [name => html]
	list;
}
imports ["Html", "http"] from "webKit";
imports "graphquery" from "webKit";

#' Run query with http cache
#'
#' @param url a data resource on remote server or local resource file
#' 
const http_query as function(url, raw = TRUE, graphquery = get_graph("graphquery/kegg_table.graphquery")) {
	({
		if (file.exists(url)) {
			readText(url)
		} else {
			getHtml(url)
		}
	})
	:> Html::parse
	:> graphquery::query(graphquery, raw = raw)
	;
}

#' Http get html or from cache
#'
const getHtml as function(url, interval = 5) {
	const http.cache_dir as string = getOption("http.cache_dir") || stop("You should set of the 'http.cache_dir' option at first!");
	
	const cacheKey as string   = md5(url);
	const prefix as string     = substr(cacheKey, 1, 2);
	const cache_file as string = `${http.cache_dir}/${prefix}/${cacheKey}.html`;
	
	if ((!file.exists(cache_file)) || (file.size(cache_file) <= 0)) {		
		writeLines(con = cache_file) {
			# request from remote server
			content(requests.get(url));
		}
		
		sleep(interval);
	}
	
	readText(cache_file);
}
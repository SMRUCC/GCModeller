imports ["Html", "http"] from "webKit";

#' Run query with http cache
#'
const http_query as function(url, graphquery) {
	getHtml(url)
	
}

const getHtml as function(url, interval = 5) {
	const http.cache_dir as string = getOption("http.cache_dir") || stop("You should set of the 'http.cache_dir' option at first!");
	
	const cacheKey as string   = md5(url);
	const prefix as string     = substr(cacheKey, 1, 2);
	const cache_file as string = `${cacheDir}/${prefix}/${cacheKey}.html`;
	
	if ((!file.exists(cache_file)) || (file.size(cache_file) <= 0)) {		
		writeLines(con = cache_file) {
			# request from remote server
			content(requests.get(url));
		}
		
		sleep(interval);
	}
	
	readText(cache_file);
}
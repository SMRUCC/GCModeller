imports ["Html", "http"] from "webKit";
imports "graphquery" from "webKit";

#' Run query with http cache
#'
#' @param url a data resource on remote server or local resource file
#' @param raw returns the JSON raw element data or ``R#`` list object?
#' @param graphquery the graphquery script for read the resource file 
#'                   from the remote server.
#'
const http_query as function(url, raw = TRUE, graphquery = get_graph("graphquery/kegg_table.graphquery")) {
  ({
    if (file.exists(url)) {
      # read from the local file
      readText(url)
    } else {
      # request from the remote server
      getHtml(url)
    }
  })
  |> Html::parse
  |> graphquery::query(graphquery, raw = raw)
  ;
}

#' Http get html or from cache
#'
#' @param url the url of the resource data on the remote server
#' @param interval the time interval in seconds for sleep after 
#'                 request data from the remote server.
#' 
const getHtml as function(url, interval = 3) {
  const http.cache_dir as string = getOption("http.cache_dir") || stop("You should set of the 'http.cache_dir' option at first!");

  const cacheKey as string   = md5(url);
  const prefix as string     = substr(cacheKey, 1, 2);
  const cache_file as string = `${http.cache_dir}/${prefix}/${cacheKey}.html`;

  if ((!file.exists(cache_file)) || (file.size(cache_file) <= 0)) {
    writeLines(con = cache_file) {
      # request from remote server
      # if the cache is not hit,
      # and then write it to the cache repository
      content(requests.get(url));
    }

    # sleep for a seconds after request resource data 
    # from the remote server
    sleep(interval);
  }

  # finally read data from cache
  readText(cache_file);
}

#' Batch query KEGG KO annotations for given KEGG gene IDs
#'
#' @description
#' `link_ko` takes a vector of KEGG gene identifiers (e.g. `"taes:803091"`)
#' and retrieves their associated KEGG Orthology (KO) identifiers by calling
#' the KEGG REST API `link/ko` endpoint. Results are cached locally in
#' text files to avoid repeated downloads.
#'
#' @param kegg_id Character vector of KEGG gene identifiers.
#'   Each element should be in the form `<org>:<gene>`, e.g. `"hsa:10458"`
#'   or `"taes:803091"`.
#'
#' @param cache Character. Path to the directory where cached results are
#'   stored. Text files in this directory are assumed to be tab‑separated
#'   output from previous KEGG `link/ko` queries. Default is `"./tmp"`.
#'
#' @param batch_size Integer. Number of KEGG IDs to include in each API
#'   request. Larger values reduce the number of requests but increase the
#'   risk of timeouts or server‑side limits. Default is `100`.
#'
#' @returns
#' This function is mainly called for its side effect: writing result files
#' into `cache`. It does not explicitly return a value, but the cached
#' files can be read back as tab‑separated tables with columns:
#' \describe{
#'   \item{V1}{KEGG gene identifier (input).}
#'   \item{V2}{KEG Orthology identifier in the form `ko:KXXXXX`.}
#' }
#' Rows correspond to gene–KO mappings returned by the KEGG API.
#' If a gene has no KO annotation, it may not appear in the cached files.
#'
#' @details
#' The function first scans `cache` for existing result files, reads them,
#' and extracts already queried KEGG IDs. Those IDs are excluded from the
#' current API requests to avoid redundant downloads.
#'
#' For each batch of remaining KEGG IDs, a URL of the form
#' \preformatted{https://rest.kegg.jp/link/ko/<id1>+<id2>+...+<idn>}
#' is constructed. The KEGG API returns a tab‑delimited text file, which is
#' written to a file named `<md5(url)>.txt` under `cache`. A 1‑second pause
#' is inserted between requests to reduce load on the KEGG server.
#'
#' This implementation assumes that the HTTP request returns plain text
#' content; the exact syntax of `requests.get()`, `plain_text`, and `sleep()`
#' depends on your runtime environment and is shown here as pseudo‑code.
#'
#' @examples
#' \dontrun{
#' # Basic usage with default cache directory
#' link_ko(c("hsa:10458", "mmu:12345"))
#'
#' # Custom cache directory and batch size
#' link_ko(
#'   kegg_id = c("taes:803091", "taes:803092"),
#'   cache = "./kegg_cache",
#'   batch_size = 50
#' )
#'
#' # Read back the cached results
#' cache_dir <- "./kegg_cache"
#' files <- list.files(cache_dir, pattern = "\\.txt$", full.names = TRUE)
#' ko_table <- do.call(rbind, lapply(files, read.table,
#'   header = FALSE, row.names = NULL, sep = "\t"
#' ))
#' head(ko_table)
#' }
#'
#' @seealso
#' \code{\link{read.table}} for reading the cached result files;
#' KEGG REST API documentation for \code{/link/ko} operation.
#'
#' @export
const link_ko = function(kegg_id, cache = "./tmp", batch_size = 100) {
    imports "http" from "webKit";

    let cached = list.files(cache, pattern = "*.txt");
    cached = lapply(cached, file => read.table(file, header = FALSE, row.names = NULL, sep = "\t"));
    cached = lapply(cached, t => t[,1]);
    cached = unlist(cached) |> unique();

    let hits = kegg_id in cached; 

    kegg_id = kegg_id[!hits];

    for(batch in tqdm(kegg_id |> split( size = batch_size))) {
        let url = `https://rest.kegg.jp/link/ko/${paste(batch, sep = "+")}`;
        let key = md5(url);

        url 
        |> requests.get() 
        |> plain_text 
        |> writeLines(con = file.path(cache, `${key}.txt`))
        ;

        sleep(1);
    }
}
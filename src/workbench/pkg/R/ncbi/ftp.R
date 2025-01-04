#' Do ftp download of the ncbi genbank assembly file.
#' 
#' @param overrides overrides the local existed file.
#' 
const ncbi_assembly_ftp = function(ref, download_dir = "./", overrides = TRUE) {
    imports "ftp" from "webKit";

    let ncbi = new ftp(server = "ftp.ncbi.nih.gov");
    let url  = gsub([ref]::ftp_path, "https://ftp.ncbi.nlm.nih.gov", "");
    let name = basename(url,TRUE);
    let ftp_url = `${url}/${name}_genomic.gbff.gz`;
    let local_path = `${download_dir}/${basename(ftp_url,TRUE)}`;

    print(`ftp get: ${ftp_url}`);

    if (overrides || (file.size(local_path) <= 0)) {
        ncbi |> ftp.get(ftp_url, download_dir & "/");
    }
}
const ncbi_assembly_ftp = function(ref, download_dir = "./") {
    imports "ftp" from "webKit";

    let ncbi = new ftp(server = "ftp.ncbi.nih.gov");
    let url  = gsub([ref]::ftp_path, "https://ftp.ncbi.nlm.nih.gov", "");
    let name = basename(url,TRUE);
    let ftp_url = `${url}/${name}_genomic.gbff.gz`;

    print(`ftp get: ${ftp_url}`);

    ncbi |> ftp.get(ftp_url, download_dir & "/");
}
const ncbi_assembly_ftp = function(id) {
    imports "ftp" from "webKit";

    let ncbi = new ftp(server = "ftp://ftp.ncbi.nih.gov");
    let str = gsub(id,"_","");
    let split_length = 3;
    let start_positions = seq(1, nchar(str), by = split_length);
    let split_str = sapply(start_positions, start -> substr(str, start, start + split_length - 1));

    str(split_str);
}
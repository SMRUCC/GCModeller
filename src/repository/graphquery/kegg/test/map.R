require(kegg_api);

options(http.cache_dir = `${@dir}/.cache/`);

for(id in ["map00020" "map00010"]) {
    kegg_map(
        id
    )
    |> xml
    |> writeLines(con = `${@dir}/demo_maps/${id}.Xml`)
    ;
}


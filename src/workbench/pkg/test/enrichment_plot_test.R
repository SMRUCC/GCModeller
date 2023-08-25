require(GCModeller);

let data = read.csv(file = "C:\Users\Administrator\Desktop\通路富集结果.csv", row.names = NULL, check.names = FALSE);

print(data);

GCModeller::KEGG_MapRender(data, map_id = "pathway",
        pathway_links = "links",
        outputdir = "C:\Users\Administrator\Desktop");
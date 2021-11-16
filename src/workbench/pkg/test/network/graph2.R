require(GCModeller);
require(igraph);

imports "igraph.layouts" from "igraph";

const data = read.csv(file = `${@dir}/all_enriched.csv`);

print(head(data));

const name = data[, "."];
const [Raw.p, Impact, compounds, pathway] = data;
const logP = -log10(Raw.p);
const IF = logP * Impact;

str(quantile(IF));

let g = empty.network();
const links = list();

for(i in 1:length(IF)) {
    const cidList = strsplit(compounds[i], "[+,;]|\s+");
    const pid = pathway[i];

    if (!(pid in names(links))) {
        links[[pid]] = list(name = name[i]);
    }

    const plinks = links[[pid]];

    for(cid in cidList) {
        if (!(cid in names(plinks))) {
            plinks[[cid]] = 0;
        }

        plinks[[cid]] = plinks[[cid]] + IF[i];
    }
}

str(links);

for(pid in names(links)) {
    const plink = links[[pid]];
    
    g |> add.node(pid, label = plink$name, group = "pathway");

    plink$name = NULL;

    for(cid in names(plink)) {
        if (is.null(g |> getElementByID(cid))) {
            g |> add.node(cid, group = "compound");
        }
        
        g |> add.edge(cid, pid, plink[[cid]]);
    }
}

g = g |> layout.force_directed;

print(g);

save.network(g, file = @dir);

# for(i in 1:length(IF)) {
#     const cidList = strsplit(compounds[i], "[+,;]|\s+");
#     const pid = pathway[i];

#     if (is.null(g |> getElementByID(pid))) {
#         g |> add.node(pid, label = name[i]);
#     }

#     for(cid in cidList) {
#         if (is.null(g |> getElementByID(cid))) {
#             g |> add.node(cid, label = cid);
#         }

#         if (!(g |> has.edge(pid, cid))) {

#         }
#     }
# }

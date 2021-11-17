require(GCModeller);
require(igraph);
require(ColorBrewer);

imports "igraph.layouts" from "igraph";
imports "igraph.render" from "igraph";
imports "styler" from "igraph";

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

bitmap(file = `${@dir}/enriched.png`) {
    g = g 
    |> compute.network 
    |> layout.force_directed(
        size       = [3000, 2100], 
        iterations = 200
    )
    ;
    
    size(V(g)) = lapply(degree(g), x -> x * 2.5);

    color(V(g)[~group == "pathway"])  = "red";
    color(V(g)[~group == "compound"]) = "green";

    color(E(g)) = "lightgray";

    const w = unlist(weight(E(g))) |> ColorBrewer::TrIQ();

    width(E(g)) = lapply(weight(E(g)), x -> ifelse(x > w, w, x));   

    str(weight(E(g)));
    print(g);

    save.network(g, file = @dir);
    render(g, canvasSize = [3000, 2100], labelerIterations = -1);
}

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

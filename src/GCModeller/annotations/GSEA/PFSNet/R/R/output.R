as.tabular = function(pfsnet, class1 = "A", class2 = "B") {
    result = as.tabular.classSet(pfsnet$class1, className = class1);
    result = rbind(result, as.tabular.classSet(pfsnet$class2, className = class2));

    result;
}

as.tabular.classSet = function(classSet, className = "A") {
    subnets = classSet$subnets;
    table   = NULL;

    if (length(subnets) == 0) {
        NULL;
    } else {
        for(pathway in names(subnets)) {
            table = rbind(table, as.tabular.subnetwork(subnets[[pathway]], pathway));
        }

        subnetwork = unlist(table[, "subnetwork"]);
        statistics = unlist( table[, "statistics"]);
        pvalue     = unlist(  table[, "pvalue"]);
        nodes      = unlist(  table[, "nodes"]);
        edges      = unlist( table[, "edges"]);
        weight1    = unlist( table[, "weight1"]);
        weight2    = unlist( table[, "weight2"]);
        genes      = unlist(  table[, "genes"]);

        data.frame(
            subnetwork = subnetwork,
            phenotype  = className,
            statistics = statistics,
            pvalue     = pvalue,
            nodes	   = nodes,
            edges      = edges,
            weight1    = weight1,
            weight2    = weight2,
            genes      = genes
        );
    }
}

as.tabular.subnetwork = function(subnet, pathway) {
    result     = subnet[[9]];
    statistics = result[[2]]$statistics;
    pvalue     = result[[2]]$p.value;
    genes      = result[[3]]$name;
    weight1    = result[[3]]$weight;
    weight2    = result[[3]]$weight2;

    list(subnetwork = pathway,
         statistics = statistics,
         pvalue     = pvalue,
         nodes      = length(genes),
         edges      = 0,
         weight1    = mean(weight1),
         weight2    = mean(weight2),
         genes      = paste(genes, collapse = "; "));
}

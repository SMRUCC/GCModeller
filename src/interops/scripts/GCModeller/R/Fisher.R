
#' Run fisher enrichment analysis
#'
#' @param geneSet a gene id list.
#'
Fisher = function(geneSet, background) {
    if (is.character(background)) {
        background = ReadFisherCluster(background);
    }

    N = sizeof(background);

    if (N < length(geneSet)) {
        N = length(geneSet);
        warning("the size of your background is less than the input geneSet!");
    }

    enrich = lapply(background, function(cluster) {
        .enrich(cluster, geneSet, N);
    });
    enrich = data.frame(
        term_id = sapply(enrich, function(i) i$term_id),
        cluster_size = sapply(enrich, function(i) i$cluster_size),
        hits = sapply(enrich, function(i) paste(i$hits, collapse = "; ")),
        size = sapply(enrich, function(i) i$size),
        p.value = sapply(enrich, function(i) i$p.value)
    );
    enrich = enrich[enrich[, "size"] > 0, ];

    # reorder via p-value in asc order
    enrich[order(enrich[, "p.value"]), ];
}

#' Fisher enrichment test
#'
#' @param N the background size, number of unique gene id in
#'    the background model.
#'
.enrich = function(cluster, geneSet, N) {
    n = length(geneSet);
    M = length(cluster$genes);
    klist = .intersect(cluster, geneSet);
    k = length(klist);

    # Crosstable for Fisher test
    Crosstab <- data.frame(
        gene.not.interest = c(M-k, ifelse(N-M-n+k < 0, 0, N-M-n+k)),
        gene.in.interest  = c(k, n-k)
    );
    row.names(Crosstab) <- c("In_category", "not_in_category");

    F = fisher.test(Crosstab);

    list(
        term_id = cluster$ID,
        cluster_size = cluster$size,
        hits = klist,
        size = k,
        p.value = F$p.value
    );
}

.intersect = function(cluster, geneSet) {
    i = sapply(cluster$genes, function(gene) {
        if (gene$geneId %in% geneSet) {
            TRUE;
        } else if (gene$locus_tag %in% geneSet) {
            TRUE;
        } else if (any(gene$term_id %in% geneSet)) {
            TRUE;
        } else {
            FALSE;
        }
    });

    cluster$genes[unlist(i)] %>%
        sapply(., function(gene) gene$locus_tag) %>%
        unlist();
}

#' Get background size
#'
#' @param background a model object which is created from
#'    the \code{\link{ReadFisherCluster}} function.
#'
sizeof = function(background) {
    background %>%
        lapply(., function(cl) sapply(cl$genes, function(gene) gene$geneId)) %>%
        unlist() %>%
        unlist() %>%
        unique() %>%
        length();
}

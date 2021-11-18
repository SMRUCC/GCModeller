ReadFisherCluster = function(xmlfile) {
    xmlTree    = XML::xmlTreeParse(file = xmlfile);
    background = xmlTree$doc$children$background;
    background = background[4:length(background)];
    background = lapply(background, function(cl) {
        .read_cluster(xmlcl = cl);
    });

    names(background) = sapply(background, function(cl) cl$ID);

    background;
}

.read_cluster = function(xmlcl) {
    info = XML::xmlAttrs(xmlcl);
    size = as.numeric(info[["size"]]);
    ID   = info[["ID"]];
    members = XML::xmlToList(xmlcl)$members;

    if (is.null(members)) {
        warning(sprintf("No members in cluster [%s]!", ID));
        members = list();
    } else {
        members = lapply(1:ncol(members), function(i) {
            .read_gene(data = members[, i]);
        });
    }

    if (size != length(members)) {
        warning(sprintf("[%s] cluster member is inconsist!", ID));
    }

    list(
        ID    = ID,
        size  = size,
        genes = members
    );
}

.read_gene = function(data) {
    ls        = names(data);
    alias     = as.vector(unlist(data[ls == "alias"]));
    term_id   = as.vector(unlist(data[ls == "term_id"]));
    locus_tag = data$locus_tag$.attrs[["name"]];
    desc      = data$locus_tag$text;
    geneId    = data$.attrs[["accessionID"]];
    geneName  = data$.attrs[["name"]];

    list(
        geneId    = geneId,
        geneName  = geneName,
        desc      = desc,
        locus_tag = locus_tag,
        term_id   = term_id,
        alias     = alias
    );
}
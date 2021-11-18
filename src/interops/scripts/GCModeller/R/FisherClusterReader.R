ReadFisherCluster = function(xmlfile) {
    xmlTree    = XML::xmlTreeParse(file = xmlfile);
    background = xmlTree$doc$children$background;
    background = background[4:length(background)];
    background = lapply(background, function(cl) {
        ReadCluster(xmlcl = cl);
    });

    names(background) = sapply(background, function(cl) cl$ID);

    background;
}

ReadCluster = function(xmlcl) {
    info = xmlAttrs(xmlcl);
    size = as.numeric(info[["size"]]);
    ID   = info[["ID"]];
    members = xmlToList(xmlcl)$members;
    members = lapply(1:size, function(i) {
        data      = members[, i];
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
    });

    list(
        ID    = ID,
        size  = size,
        genes = members
    );
}

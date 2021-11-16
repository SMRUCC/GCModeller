imports ["bioseq.patterns", "bioseq.fasta", "bioseq.blast"] from "GCModeller::seqtoolkit";

setwd(@dir);

const raw     = read.csv("QLZ092-20210412-P_ModificationSites.txt", tsv = TRUE, check.names = FALSE);
const target  = ["RcsF", "RcsC", "RcsD", "RcsA", "RcsB"];
const uniprot = read.fasta("uniprot-taxonomy__Escherichia+coli+(strain+K12)+[83333]_.fasta")
|> which(function(fsa) {
    as.object(fsa)$Headers
    |> grep(target, fixed = TRUE)
    |> any
    ;
})
;
const query = as.list(raw, byrow = TRUE)
|> lapply(function(i) {
    fasta(i[["Peptide Sequence"]], [i[["Protein Accession"]], i[["Protein Description"]]]);
});

str(query);
# str(raw);
# print(length(uniprot));

lapply(uniprot, function(seq) {
    let blosum = blosum();
    let align  = NULL; 
    
    for(q in query) {
        align.smith_waterman(q, seq, blosum) 
        |> HSP(0.8, 0.9 * size(q))
        |> (function(data) {
            data[, "queryName"]   = rep(as.object(q)$Headers[1], nrow(data));
            data[, "subjectName"] = rep(toString(seq), nrow(data));
			data[, "sequence"]    = rep(as.object(seq)$SequenceData, nrow(data));

            align = rbind(align, data);
        });       
    }

    print(head(align));

    align;
})
|> (function(list) {
    let data = NULL;

    for(part in list) {
        data = rbind(data, part);
    }

    data;
})
|> write.csv(file = "search.csv", row.names = FALSE)
;
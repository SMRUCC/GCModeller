#' create ko database
const ko_db = function(db = "./", species = NULL) {
    options(http.cache_dir = "$ko_db.http_cache");
    set(globalenv(),"$ko_db.http_cache", db);

    let ko_index = load_ko_index(db);

    print("load ko index table for make request data:");
    print(ko_index, max.print = 13);

    if (length(species) > 0) {
        print("KO gene sequence database will be downloaded with a specific genome range:");
        print(species);
    }

    for(ko in tqdm(as.list(ko_index,byrow=TRUE))) {
        let ko_id = ko$ko;
        let geneId_file = `/ko/${ko_id}.txt`;
        let ko_genes = NULL;

        if (!file.exists(geneId_file, fs=db)) {
            let ko_genes = REnv::getHtml(`https://rest.kegg.jp/link/genes/${ko_id}`, interval = 3, filetype = "txt");

            ko_genes = ko_genes |> textlines() |> strsplit("\s+");
            ko_genes = ko_genes@{2};

            writeLines(ko_genes, con = file.allocate(geneId_file, fs = db));
        }        

        ko_genes = readLines(file.allocate(geneId_file, fs = db));
        ko_genes = strsplit(ko_genes,"[:]");
        ko_genes = data.frame(species = ko_genes@{1}, gene_id = ko_genes@{2});

        print("genes that belongs to KO group:");
        print(ko_genes, max.print = 6);

        if (length(species) > 0) {
            let i = ko_genes$species in species;

            if (sum(i) > 0) {
                ko_genes = ko_genes[i,];
            } else {
                ko_genes = NULL;
            }            
        }

        if (nrow(ko_genes) > 0) {
            let seqfile = `/fasta/${ko_id}.txt`;

            if (!file.exists(seqfile, fs=db)) {
                let fasta = NULL;

                ko_genes = sprintf("%s:%s", ko_genes$species, ko_genes$gene_id);
                ko_genes = split(ko_genes, size = 20);

                print("download gene sequence with data task blocks:");
                print(length(ko_genes));
                str(ko_genes);

                for(block in ko_genes) {
                    let url = `https://rest.kegg.jp/get/${paste(block, sep="+")}/ntseq`;
                    let seqs = REnv::getHtml(url, interval = 3, filetype = "txt");
                    
                    fasta = c(fasta, seqs);
                }

                writeLines(fasta, con = file.allocate(seqfile, fs = db));
            }
        }
    }
}

const load_ko_index = function(db) {
    if (!file.exists("/ko.csv", fs = db)) {
        let index = REnv::getHtml("https://rest.kegg.jp/list/ko", interval = 3, filetype = "txt");
        let ko_index = textlines(index) 
            |> strsplit("(\t+)|(;\s+)") 
            |> tqdm() 
            |> lapply(t -> {
                if (length(t) == 3) {
                    c(t[1],"",t[3]);
                } else {
                    t[c(1,3,5)];
                }
            })
            ;

        ko_index = data.frame(
            row.names = ko_index@{1},
            ko = ko_index@{1},
            gene_names = ko_index@{2},
            description = ko_index@{3}
        );

        print("request ko index from KEGG web:");
        print(ko_index, max.print = 6);

        write.csv(ko_index, file = file.allocate("/ko.csv", fs = db));
    }

    read.csv(file.allocate("/ko.csv", fs = db), 
        row.names = 1, 
        check.names = FALSE);
}
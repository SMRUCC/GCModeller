imports ["annotation.genomics", "annotation.genbank_kit", "bioseq.fasta"] from "seqtoolkit.dll";

let genome.nt = read.seq("K:\20200226\IGV_data\genome.fasta") :> as.object;
let sRNA = $"sRNA\d+";

["K:\20200226\IGV_data\genome.gtf"]
:> read.gtf
:> which(gene -> !(as.object(gene)$Synonym like sRNA))
:> as.tabular(title = genome.nt$Title, size = genome.nt$Length)
:> as.genbank
:> add.origin.fasta(genome.nt)
:> add.protein.fasta(read.fasta("K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\X101SC19112292-Z01-J001-B1-16.fasta"))
:> write.genbank(file = "K:\20200226\IGV_data\assembly.gb")
;
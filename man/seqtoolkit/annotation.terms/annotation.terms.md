# annotation.terms

tools for make ontology term annotation based on the proteins sequence data

+ [removes_proteinIDSuffix](annotation.terms/removes_proteinIDSuffix.1) Removes the numeric suffix (usually representing an exon index) from protein identifiers.
+ [read_rankterms](annotation.terms/read_rankterms.1) read the given table file as rank term object
+ [rank_term](annotation.terms/rank_term.1) create the rank term annotation objects from the given parallel data 
+ [geneNames](annotation.terms/geneNames.1) try parse gene names from the product description strings
+ [assign_ko](annotation.terms/assign_ko.1) do KO number assign based on the bbh alignment result.
+ [assign.COG](annotation.terms/assign.COG.1) assign the COG functional category for each query gene by keeping the 
+ [assign_terms](annotation.terms/assign_terms.1) assign the top term by score ranking
+ [m8_metabolic_terms](annotation.terms/m8_metabolic_terms.1) extract the metabolic function term annotation from the diamond m8 
+ [term_table](annotation.terms/term_table.1) make the term annotation table of each gene from a given set of the 
+ [assign.Pfam](annotation.terms/assign.Pfam.1) assign the pfam protein domain annotation for the query protein 
+ [assign.GO](annotation.terms/assign.GO.1) assign the gene ontology(GO) term annotation for the query gene
+ [write.id_maps](annotation.terms/write.id_maps.1) save the id mapping solver data into a text file
+ [read.MyvaCOG](annotation.terms/read.MyvaCOG.1) read the myva COG hits annotation data from a csv table file
+ [read.id_maps](annotation.terms/read.id_maps.1) read the id mappings text data as the secondary id mapping solver 
+ [synonym](annotation.terms/synonym.1) query the id synonyms of the given id list from the id mapping solver 
+ [read_vfdb_seqs](annotation.terms/read_vfdb_seqs.1) read VFDB fasta sequence database
+ [write_simple_vfdb](annotation.terms/write_simple_vfdb.1) save the VFDB virulence factor sequence data as a simple fasta format 
+ [make_vectors](annotation.terms/make_vectors.1) make the genomics metabolic vector data from the given term annotation 
+ [write_genomes_jsonl](annotation.terms/write_genomes_jsonl.1) write the genomics metabolic vector data into a jsonl text file
+ [tfidf_vectorizer](annotation.terms/tfidf_vectorizer.1) make embedding of the genomics metabolic model

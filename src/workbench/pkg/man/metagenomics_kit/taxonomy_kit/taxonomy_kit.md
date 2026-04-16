# taxonomy_kit

toolkit for process ncbi taxonomy tree data
> The Taxonomy Database is a curated classification and nomenclature for all of the 
>  organisms in the public sequence databases. This currently represents about 10% 
>  of the described species of life on the planet.

+ [biom.string](taxonomy_kit/biom.string.1) cast taxonomy object to biom style taxonomy string
+ [biom_string.parse](taxonomy_kit/biom_string.parse.1) parse the taxonomy string in BIOM style
+ [taxonomy_name](taxonomy_kit/taxonomy_name.1) get taxonomy name
+ [unique_taxonomy](taxonomy_kit/unique_taxonomy.1) make taxonomy object unique
+ [Ncbi.taxonomy_tree](taxonomy_kit/Ncbi.taxonomy_tree.1) load ncbi taxonomy tree model from the given data files
+ [ranks](taxonomy_kit/ranks.1) cast the ncbi taxonomy tree model to taxonomy ranks data
+ [taxonomy_ranks](taxonomy_kit/taxonomy_ranks.1) get all taxonomy tree nodes of the specific taxonomy ranks
+ [taxonomy.filter](taxonomy_kit/taxonomy.filter.1) 
+ [lineage](taxonomy_kit/lineage.1) get taxonomy lineage model from the ncbi taxonomy tree by given taxonomy id
+ [as.taxonomy.tree](taxonomy_kit/as.taxonomy.tree.1) build taxonomy tree based on a given collection of taxonomy object.
+ [consensus](taxonomy_kit/consensus.1) 
+ [read.mothurTree](taxonomy_kit/read.mothurTree.1) Parse the result output from mothur command ``summary.tax``.
+ [taxonomy_range](taxonomy_kit/taxonomy_range.1) 
+ [LCA](taxonomy_kit/LCA.1) 
+ [accession2Taxid](taxonomy_kit/accession2Taxid.1) Create a stream read to the ncbi accession id mapping to ncbi taxonomy id

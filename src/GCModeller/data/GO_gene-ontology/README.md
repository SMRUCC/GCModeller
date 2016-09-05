# GO_gene-ontology
Gene Ontology Parser for GCModeller

The Gene Ontology (GO) project is a collaborative effort to address the need for consistent descriptions of gene products across databases. Founded in 1998, the project began as a collaboration between three model organism databases, FlyBase (Drosophila), the Saccharomyces Genome Database (SGD) and the Mouse Genome Database (MGD). The GO Consortium (GOC) has since grown to incorporate many databases, including several of the world's major repositories for plant, animal, and microbial genomes. The GO Contributors page lists all member organizations.

The GO project has developed three structured ontologies that describe gene products in terms of their associated biological processes, cellular components and molecular functions in a species-independent manner. There are three separate aspects to this effort: first, the development and maintenance of the ontologies themselves; second, the annotation of gene products, which entails making associations between the ontologies and the genes and gene products in the collaborating databases; and third, the development of tools that facilitate the creation, maintenance and use of ontologies.

The use of GO terms by collaborating databases facilitates uniform queries across all of them. Controlled vocabularies are structured so they can be queried at different levels; for example, users may query GO to find all gene products in the mouse genome that are involved in signal transduction, or zoom in on all receptor tyrosine kinases that have been annotated. This structure also allows annotators to assign properties to genes or gene products at different levels, depending on the depth of knowledge about that entity.

Shared vocabularies are an important step towards unifying biological databases, but additional work is still necessary as knowledge changes, updates lag behind, and individual curators evaluate data differently. The GO aims to serve as a platform where curators can agree on stating how and why a specific term is used, and how to consistently apply it, for example, to establish relationships between gene products.

#### The Scope of GO
The following areas are outside the scope of GO, and terms in these domains will not appear in the ontologies:

+ Gene products: e.g. cytochrome c is not in the ontologies, but attributes of cytochrome c, such as oxidoreductase activity, are.
+ Processes, functions or components that are unique to mutants or diseases: e.g. oncogenesis is not a valid GO term, as "causing cancer" is the result of reprogrammed, not normal cells and thus it is not the normal function of a gene.
+ Attributes of sequence such as "intron" or "exon" parameters belong in a separate sequence ontology; see the Open Biological and Biomedical Ontologies website for more information.
+ Protein domains or structural features.
+ Protein-protein interactions.
+ Environment, evolution and expression.
+ Anatomical or histological features above the level of cellular components, including cell types.
# Species-Specific Terms

## Handling Species Specificity
One of the biggest problems for a controlled vocabulary is choosing term names and definitions that will unambiguously identify a component, function or process. If a word or phrase refers to different entities or processes depending upon the organism, subclasses are created based on differentiating characteristics, such structure, physical composition or order of subprocesses, rather than by identifying the taxonomic group in which the component or process occurs.
A classic example of this is cell wall, which refers to the rigid or semi-rigid structure surrounding the cell membrane in plants, fungi and some prokaryotes. Its composition differs between these three sets of organisms, though, so to allow greater specificity of annotation, the ontology describes three subclasses of cell wall differentiated by their physical characteristics:

+ cell wall
  + [i] fungal-type cell wall
  + [i] peptidoglycan-based cell wall
  + [i] plant-type cell wall

The definitions for these terms are as follows:

###### cell wall
The rigid or semi-rigid envelope lying outside the cell membrane of plant, fungal, and most prokaryotic cells, maintaining their shape and protecting them from osmotic lysis. In plants it is made of cellulose and, often, lignin; in fungi it is composed largely of polysaccharides; in bacteria it is composed of peptidoglycan.

###### plant-type cell wall
A more or less rigid structure lying outside the cell membrane of a cell and composed of cellulose and pectin and other organic and inorganic substances.

###### peptidoglycan-based cell wall
A protective structure outside the cytoplasmic membrane composed of peptidoglycan (also known as murein), a molecule made up of a glycan (sugar) backbone of repetitively alternating N-acetylglucosamine and N-acetylmuramic acid with short, attached, cross-linked peptide chains containing unusual amino acids. An example of this component is found in Escherichia coli. .

###### fungal-type cell wall
A rigid yet dynamic structure surrounding the plasma membrane that affords protection from stresses and contributes to cell morphogenesis, consisting of extensively cross-linked glycoproteins and carbohydrates. The glycoproteins may be modified with N- or O-linked carbohydrates, or glycosylphosphatidylinositol (GPI) anchors; the polysaccharides are primarily branched glucans, including beta-linked and alpha-linked glucans, and may also include chitin and other carbohydrate polymers, but not cellulose or pectin. Enzymes involved in cell wall biosynthesis are also found in the cell wall. Note that some forms of fungi develop a capsule outside of the cell wall under certain circumstances; this is considered a separate structure.

Note that the definitions use physical and structural characteristics to differentiate between cell wall types.

### Terms with Taxon Restrictions
The Gene Ontology also provides an OBO format file containing species-specific terms and the taxa for which they are or are not appropriate. The file can be viewed online , downloaded by FTP , or found in the GO SVN repository at go/quality_control/annotation_checks/taxon_checks/taxon_go_triggers.obo.

### Sensu Terms
The use of sensu, ‘in the sense of’, to designate a certain interpretation of a word or phrase has been deprecated.
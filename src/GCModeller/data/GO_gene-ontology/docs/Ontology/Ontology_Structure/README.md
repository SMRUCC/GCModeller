# Ontology Structure

## What is an Ontology?
An ontology is a formal representation of a body of knowledge, within a given domain. Ontologies usually consist of a set of classes or terms with relations that operate between them. The domains that GO represents are biological processes, functions and cellular components.

## GO as a Graph
The structure of GO can be described in terms of a graph, where each GO term is a node, and the relationships between the terms are edges between the nodes. GO is loosely hierarchical, with 'child' terms being more specialized than their 'parent' terms, but unlike a strict hierarchy, a term may have more than one parent term (note that the parent/child model does not hold true for all types of relation, see the relations documentation). For example, the biological process term hexose biosynthetic process has two parents, hexose metabolic process and monosaccharide biosynthetic process. This is because biosynthetic process is a subtype of metabolic process and a hexose is a subtype of monosaccharide.
The following diagram is a screenshot from the ontology editing software OBO-Edit, showing a small set of terms from the ontology.

![](diag-ontology-graph.gif)
A set of terms under the biological process node pigmentation.

In the diagram, relations between the terms are represented by the colored arrows; the letter in the box midway along each arrow is the relationship type. Note that the terms get more specialized going down the graph, with the most general terms—the root nodes, cellular component, biological process and molecular function—at the top of the graph. Terms may have more than one parent, and they may be connected to parent terms via different relations. The GO relations documentation describes these relations in greater detail.

## One Ontology... or Three?
As the diagram above suggests, the three GO domains (cellular component, biological process, and molecular function) are each represented by a root ontology term. All terms in a domain can trace their parentage to the root term, although there may be numerous different paths via varying numbers of intermediary terms to the ontology root. The three root nodes are unrelated and do not have a common parent node, and hence GO is three ontologies. Some graph-based software may require a single root node; in these cases, a "fake" term can be added as a parent of the three existing root nodes.
The three GO ontologies are is_a disjoint, meaning that no is_a relations operate between terms from the different ontologies. However, other relationships such as part_of and regulates do operate between the GO ontologies. For example the molecular function term 'cyclin-dependent protein kinase activity' is part_of the biological process 'cell cycle'.

## Obsolete Terms
Occasionally, a term is added to GO that is out of scope, misleadingly named or defined, or describes a concept that would be better represented in another way and needs to be removed from the published ontology. In these cases, the term and ID still persist in the ontology, but the term is tagged as obsolete, and all relationships to other terms are removed. A comment is added to the term, detailing the reason for the obsoletion and tags are also added that specify replacement terms. See the OBO file format specification for more information.
## Term Structure
### Essential Elements

###### Unique identifier and term name
Every term has a term name—e.g. mitochondrion, glucose transport, amino acid binding—and a unique zero-padded seven digit identifier (often called the term accession or term accession number) prefixed by GO:, e.g. GO:0005125 or GO:0060092. The numerical portion of the ID has no inherent meaning or relation to the position of the term in the ontologies. Ranges of GO IDs are assigned to individual ontology editors or editing groups, and can thus be used to trace who added the term.

###### Namespace
Denotes which of the three sub-ontologies—cellular component, biological process or molecular function—the term belongs to.

###### Definition
A textual description of what the term represents, plus reference(s) to the source of the information. All new terms added to the ontology must have a definition; there remains a very small set of terms from the original ontology that lack definitions, but the vast majority of terms are defined.

###### Relationships to other terms
One or more links that capture how the term relates to other terms in the ontology. All terms (other than the root terms representing each namespace, above) have an is a sub-class relationship to another term; for example, GO:0015758 : glucose transport is a GO:0015749 : monosaccharide transport. The Gene Ontology employs a number of other relations, including part of (e.g. GO:0031966 : mitochondrial membrane part of GO:0005740 : mitochondrial envelope) and regulates (e.g. GO:0006916 : anti-apoptosis regulates GO:0012501 : programmed cell death). The relations documentation has more information on the relations used in the ontology.

### Optional Extras

###### Secondary IDs
Alternate IDs that refer to a term. Secondary IDs come about when two or more terms are identical in meaning, and are merged into a single term. All terms IDs are preserved so that no information (for example, annotations to the merged IDs) is lost.

###### Synonyms
Alternative words or phrases closely related in meaning to the term name, with indication of the relationship between the name and synonym given by the synonym scope. The scopes for GO synonyms are:

+ exact
  an exact equivalent; interchangeable with the term name
  e.g. ornithine cycle is an exact synonym of urea cycle
+ broad
  the synonym is broader than the term name
  e.g. cell division is a broad synonym of cytokinesis
+ narrow
  the synonym is narrower or more precise than the term name
  e.g. pyrimidine-dimer repair by photolyase is a narrow synonym of photoreactive repair
+ related
  the terms are related in some way not covered above
  e.g. cytochrome bc1 complex is a related synonym of ubiquinol-cytochrome-c reductase activity virulence is a related synonym of pathogenesis

Custom synonym types are also used in the ontology. For example, a number of synonyms are designated as systematic synonyms; synonyms of this type are exact synonyms of the term name.

###### Database cross-references
Database cross-references, or dbxrefs, refer to identical or very similar objects in other databases. For instance, the molecular function term retinal isomerase activity is cross-referenced with the Enzyme Commission entry EC:5.2.1.3; the biological process term sulfate assimilation has the cross-reference MetaCyc:PWY-781.

###### Comment
Any extra information about the term and its usage.

###### Subset
Indicates that the term belongs to a designated subset of terms, e.g. one of the GO slims.

###### Obsolete tag
Indicates that the term has been deprecated and should not be used.

### Sample GO Term
The following is a GO term taken from the OBO format file.
```
id: GO:0016049
name: cell growth
namespace: biological_process
def: "The process in which a cell irreversibly increases in size over time by accretion and biosynthetic production of matter similar to that already present." [GOC:ai]
subset: goslim_generic
subset: goslim_plant
subset: gosubset_prok
synonym: "cell expansion" RELATED []
synonym: "cellular growth" EXACT []
synonym: "growth of cell" EXACT []
is_a: GO:0009987 ! cellular process
is_a: GO:0040007 ! growth
relationship: part_of GO:0008361 ! regulation of cell size
```

## Cross-Products and Logical Definitions
To be maximally useful, the Gene Ontology should be accessible to computers as well as to human users, enabling tools to access the data and perform tasks and analyses that would be time-consuming and work intensive for humans. One aspect that can aid automated access to the ontology is creating computable logical definitions to complement the existing text definitions. These logical definitions are in the genus-differentia form: the definition consists of the genus, the broader class to which the term belongs, and the differentia, the properties that distinguish the term from other members of the class. For example:

mitochondrial DNA replication is DNA replication that occurs in a mitochondrion

DNA replication is the genus and the differentia is occurs in a mitochondrion.

lysosomal membrane is the membrane that surrounds a lysosome

membrane is the genus and the differentia is surrounds a lysosome.

If we use ontology terms in the genus and the differentia, we can see that these logical definitions take the general form
```
term = term that relation term
```
For example:

mitochondrial DNA replication is DNA replication that occurs in a mitochondrion

lysosomal membrane is a membrane that surrounds a lysosome

These definitions of terms created by combining other terms with relations are called cross-products in GO parlance. In the OBO 1.2 format file, the human-readable text definition is held in the def line, and the cross-product definition in the intersection_of lines of a stanza. The cross-products above would be represented as follows:
```
[Term]
id: GO:0006264
name: mitochondrial DNA replication
def: "The process whereby new strands of DNA are synthesized in the mitochondrion." [source: GOC:ai]
intersection_of: GO:0006260 ! DNA replication
intersection_of: OBO_REL:occurs_in GO:0005739 ! mitochondrion
[Term]
id: GO:0005765
name: lysosomal membrane
def: "The lipid bilayer surrounding the lysosome and separating its contents from the cell cytoplasm." [source: GOC:ai]
intersection_of: GO:0016020 ! membrane
intersection_of: part_of GO:0005764 ! lysosome
```

## Cross-Products with external ontologies
Cross-products need not be restricted to terms within GO; cross-products can also be created by combining GO terms with those from other ontologies. For example, by using the Cell Ontology, we can easily extract cell type information from GO terms. For example:
megasporocyte nucleus (GO:0043076) is a nucleus (GO:0005634) that is part of a megasporocyte (CL:0000320)
```
[Term]
id: GO:0043076
name: megasporocyte nucleus
def: "The nucleus of a megasporocyte, a diploid cell that undergoes meiosis to produce four megaspores, and its descendents."
[source: GOC:jl, ISBN:0618254153]
intersection_of: GO:0005634 ! nucleus
intersection_of: part_of CL:0000320 ! megasporocyte
```
osteoblast development (GO:0002076) is cell development (GO:0048468) that results in the complete development of an osteoblast (CL:0000062)
```
[Term]
id: GO:0002076
name: osteoblast development
def: "The process whose specific outcome is the progression of an osteoblast over time, from its formation to the mature structure. Osteoblast development does not include the steps involved in committing a cranial neural crest cell or an osteoprogenitor cell to an osteoblast fate. An osteoblast is a cell that gives rise to bone." [source: GOC:dph]
intersection_of: GO:0048468 ! cell development
intersection_of: OBO_REL:results_in_complete_development_of CL:0000062 ! osteoblast
```
Cross-products are currently being retrofitted to existing ontology terms and added to new terms. Eventually, the hope is that cross-products could be dynamically generated, rather than having be added manually each time a new term is required. This would obviate the need for some of highly specific terms in GO—for example, many of the terms referring to organism anatomy or chemical entities—and simplify ontology searches and browsing.
More information on the ongoing work on cross-products can be found in the cross-products category on the GO wiki.
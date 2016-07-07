---
title: FeatureQualifiers
---

# FeatureQualifiers
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES](N-SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.html)_

Qualifiers provide additional information about features. They take
 the form of a slash (/) followed by a qualifier name and, if
 applicable, an equal sign (=) and a qualifier value. Feature
 qualifiers begin at column 22

> 请注意，由于是直接使用ToString方法进行查询键值的获取的，所以请不要修改这些枚举对象的大小写



### Properties

#### anticodon
Location of the anticodon of tRNA and the amino acid for which it codes
#### bound_moiety
Moiety bound
#### citation
Reference to a citation providing the claim of or evidence for a feature
#### codon
Specifies a codon that is different from any found in the reference genetic code
#### codon_start
Indicates the first base of the first complete codon in a CDS (as 1 or 2 or 3)
#### cons_splice
Identifies intron splice sites that do not conform to the 5'-GT... AG-3' splice site consensus
#### db_xref
A database cross-reference; pointer to related information in another database. A description of all cross-references can be found at: http://www.ncbi.nlm.nih.gov/collab/db_xref.html
#### direction
Direction of DNA replication
#### EC_number
Enzyme Commission number for the enzyme product of thesequence
#### evidence
Value indicating the nature of supporting evidence
#### frequency
Frequency of the occurrence of a feature
#### function
Function attributed to a sequence
#### gene
Symbol of the gene corresponding to a sequence region (usable with all features)
#### label
A label used to permanently identify a feature
#### map
Map position of the feature in free-format text
#### mod_base
Abbreviation for a modified nucleotide base
#### note
Any comment or additional information
#### number
A number indicating the order of genetic elements (e.g., exons or introns) in the 5 to 3 direction
#### organism
Name of the organism that is the source of thesequence data in the record.
#### partial
Differentiates between complete regions and partial ones
#### phenotype
Phenotype conferred by the feature
#### product
Name of a product encoded by a coding region (CDS) feature
#### pseudo
Indicates that this feature is a non-functional version of the element named by the feature key
#### rpt_family
Type of repeated sequence; Alu or Kpn, for example
#### rpt_type
Organization of repeated sequence
#### rpt_unit
Identity of repeat unit that constitutes a repeat_region
#### standard_name
Accepted standard name for this feature
#### transl_except
Translational exception: single codon, the translation of which does not conform to the reference genetic code
#### translation
Amino acid translation of a coding region
#### type
Name of a strain if different from that in the SOURCE field
#### usedin
Indicates that feature is used in a compound feature in another entry

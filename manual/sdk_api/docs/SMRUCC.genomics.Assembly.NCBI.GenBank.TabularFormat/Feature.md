﻿# Feature
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat](./index.md)_

A feature is here an interval (i.e., a range of positions) on a chromosome or a union of such intervals.
 (Feature是基因组序列片段之上的一个具备有生物学功能意义的区域，故而这个对象继承自核酸位点对象)
 
 In the case of RNA-Seq, the features are typically genes, where each gene is considered here as the union of all its exons. 
 One may also consider each exon as a feature, e.g., in order to check for alternative splicing. 
 
 For comparative ChIP-Seq, the features might be binding region from a pre-determined list.




### Properties

#### attributes
From version 2 onwards, the attribute field must have an tag value structure following the syntax used within objects in 
 a .ace file, flattened onto one line by semicolon separators. Tags must be standard identifiers ([A-Za-z][A-Za-z0-9_]*). 
 Free text values must be quoted with double quotes. Note: all non-printing characters in such free text value strings 
 (e.g. newlines, tabs, control characters, etc) must be explicitly represented by their C (UNIX) style backslash-escaped 
 representation (e.g. newlines as '\n', tabs as '\t'). As in ACEDB, multiple values can follow a specific tag. The aim is 
 to establish consistent use of particular tags, corresponding to an underlying implied ACEDB model if you want to think 
 that way (but acedb is not required). Examples of these would be:
 
 seq1 BLASTX similarity 101 235 87.1 + 0 Target "HBA_HUMAN" 11 55 ; E_value 0.0003
 dJ102G20 GD_mRNA coding_exon 7105 7201 . - 2 Sequence "dJ102G20.C1.1"
 
 The semantics Of tags In attribute field tag-values pairs has intentionally Not been formalized. Two useful guidelines are 
 To use DDBJ/EMBL/GenBank feature 'qualifiers' (see DDBJ/EMBL/GenBank feature table documentation), or the features that 
 ACEDB generates when it dumps GFF. Version 1 note In version 1 the attribute field was called the group field, with the 
 following specification: An optional string-valued field that can be used as a name to group together a set of records. 
 Typical uses might be to group the introns and exons in one gene prediction (or experimentally verified gene structure), 
 or to group multiple regions of match to another sequence, such as an EST or a protein.
 (请注意，所有的key都已经被转换为小写的形式了)
#### comments
Comments are allowed, starting with "#" as in Perl, awk etc. Everything following # until the end of the line is ignored. 
 Effectively this can be used in two ways. Either it must be at the beginning of the line (after any whitespace), to make 
 the whole line a comment, or the comment could come after all the required fields on the line.
#### Ends
Integers. <start> must be less than or equal to <end>. Sequence numbering starts at 1, so these numbers 
 should be between 1 and the length of the relevant sequence, inclusive. 
 
 (Version 2 change: version 2 condones values of <start> and <end> that extend outside the reference sequence. 
 This is often more natural when dumping from acedb, rather than clipping. It means that some software using the 
 files may need to clip for itself.)
#### Feature
The feature type name. We hope to suggest a standard set of features, to facilitate import/export, comparison etc.. 
 Of course, people are free to define new ones as needed. For example, Genie splice detectors account for a region 
 of DNA, and multiple detectors may be available for the same site, as shown above. We would like to enforce a 
 standard nomenclature for common GFF features. This does not forbid the use of other features, rather, just that 
 if the feature is obviously described in the standard list, that the standard label should be used. For this standard 
 table we propose to fall back on the international public standards for genomic database feature annotation, 
 specifically, the DDBJ/EMBL/GenBank feature table documentation).
#### frame
One of '0', '1', '2' or '.'. '0' indicates that the specified region is in frame, i.e. that its first base corresponds to 
 the first base of a codon. '1' indicates that there is one extra base, i.e. that the second base of the region corresponds 
 to the first base of a codon, and '2' means that the third base of the region is the first base of a codon. 
 
 If the strand is '-', then the first base of the region is value of <end>, because the corresponding coding region will run 
 from <end> to <start> on the reverse strand. As with <strand>, if the frame is not relevant then set <frame> to '.'. 
 It has been pointed out that "phase" might be a better descriptor than "frame" for this field. 
 
 Version 2 change: This field is left empty '.' for RNA and protein features.
#### ID
请注意，这个属性不是基因号
#### ProteinId

#### score
A floating point value. When there is no score (i.e. for a sensor that just records the possible presence of a signal, 
 as for the EMBL features above) you should use '.'. 
 
 (Version 2 change: in version 1 of GFF you had to write 0 in such circumstances.)
#### seqname
The name of the sequence. Having an explicit sequence name allows a feature file to be prepared for a data set 
 of multiple sequences. Normally the seqname will be the identifier of the sequence in an accompanying fasta 
 format file. An alternative is that <seqname> is the identifier for a sequence in a public database, such as 
 an EMBL/Genbank/DDBJ accession number. Which is the case, and which file or database to use, should be explained 
 in accompanying information.
#### source
The source of this feature. This field will normally be used to indicate the program making the prediction, 
 or if it comes from public database annotation, or is experimentally verified, etc.
#### start
Integers. <start> must be less than or equal to <end>. Sequence numbering starts at 1, so these numbers 
 should be between 1 and the length of the relevant sequence, inclusive. 
 
 (Version 2 change: version 2 condones values of <start> and <end> that extend outside the reference sequence. 
 This is often more natural when dumping from acedb, rather than clipping. It means that some software using the 
 files may need to clip for itself.)
#### Strand
One of '+', '-' or '.'. '.' should be used when strand is not relevant, e.g. for dinucleotide repeats. 
 
 Version 2 change: This field is left empty '.' for RNA and protein features.

---
title: RegulatoryElement
---

# RegulatoryElement
_namespace: [SMRUCC.genomics.Data.Regtransbase.StructureObjects](N-SMRUCC.genomics.Data.Regtransbase.StructureObjects.html)_

Regulatory elements 
 For each article, annotator input information about regulatory elements and set up links between them.
 Regulatory elements are “players” in experiments described in the article. There are 10 types of 
 regulatory elements (corresponding to 10 objects): Inductor (Effector), Regulator, Site, Gene, 
 Transcript, Operon, Locus, Regulon, Helix, и Secondary Structure.




### Properties

#### ArticleGuid
art_guid - determine article (link to articles), id of the article, which contain the 
 regulatory element
#### Descript
descript - some description in free form, description of the regulatory element
#### GenomeGuid
genome_guid - determine genome (link to dict_genomes)
#### Guid
[regelem]_guid - unique identifier
#### IsRealName
fl_real_name - boolean, determine whether name is "real" (i.e. does it belong to some systematic 
 nomenclature or was introduced by article authors)
#### Name
name - regulatory element name
#### PackageGuid
pkg_guid - determine package (link to packages)

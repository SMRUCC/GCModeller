---
title: Sites
---

# Sites
_namespace: [SMRUCC.genomics.Data.Regtransbase.StructureObjects](N-SMRUCC.genomics.Data.Regtransbase.StructureObjects.html)_



> 
>  CREATE TABLE `sites` (
>    `site_guid` int(11) NOT NULL DEFAULT '0',
>    `pkg_guid` int(11) NOT NULL DEFAULT '0',
>    `art_guid` int(11) NOT NULL DEFAULT '0',
>    `name` varchar(50) DEFAULT NULL,
>    `fl_real_name` int(1) DEFAULT NULL,
>    `genome_guid` int(11) DEFAULT NULL,
>    `func_site_type_guid` int(11) DEFAULT NULL,
>    `struct_site_type_guid` int(11) DEFAULT NULL,
>    `regulator_guid` int(11) DEFAULT '0',
>    `fl_dna_rna` int(1) DEFAULT NULL,
>    `pos_from` int(11) DEFAULT NULL,
>    `pos_from_guid` int(11) DEFAULT NULL,
>    `pfo_type_id` int(11) DEFAULT NULL,
>    `pfo_side_guid` int(11) DEFAULT NULL,
>    `pos_to` int(11) DEFAULT NULL,
>    `pos_to_guid` int(11) DEFAULT NULL,
>    `pto_type_id` int(11) DEFAULT NULL,
>    `pto_side_guid` int(11) DEFAULT NULL,
>    `site_len` int(11) DEFAULT NULL,
>    `sequence` text,
>    `signature` varchar(255) DEFAULT NULL,
>    `descript` blob,
>    `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
>    PRIMARY KEY (`site_guid`),
>    KEY `FK_sites-pkg_guid` (`pkg_guid`),
>    KEY `FK_sites-art_guid` (`art_guid`),
>    KEY `FK_sites-genome_guid` (`genome_guid`),
>    KEY `FK_sites-func_site_type_guid` (`func_site_type_guid`),
>    KEY `FK_sites-struct_site_type_guid` (`struct_site_type_guid`),
>    KEY `FK_sites-regulator_guid` (`regulator_guid`),
>    CONSTRAINT `FK_sites-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
>    CONSTRAINT `FK_sites-func_site_type_guid` FOREIGN KEY (`func_site_type_guid`) REFERENCES `dict_func_site_types` (`func_site_type_guid`),
>    CONSTRAINT `FK_sites-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
>    CONSTRAINT `FK_sites-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
>    CONSTRAINT `FK_sites-struct_site_type_guid` FOREIGN KEY (`struct_site_type_guid`) REFERENCES `dict_struct_site_types` (`struct_site_type_guid`),
>    CONSTRAINT `sites_ibfk_1` FOREIGN KEY (`regulator_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
>  ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
>  


### Methods

#### FixSequenceError
```csharp
SMRUCC.genomics.Data.Regtransbase.StructureObjects.Sites.FixSequenceError(System.String@)
```
针对DNA序列进行修复

|Parameter Name|Remarks|
|--------------|-------|
|Sequence|-|



### Properties

#### fl_dna_rna
fl_dna_rna: DNA/RNA flag
#### FuncSiteTypeGuid
functional_site_type_guid: id of the functional type of the site (from FunctionalSiteType dictionary (see below, part 5)).
#### PfoSideGuid
pfo_side_guid, pto_side_guid: id of the relation between reference regulatory element and current regulatory element, 
 which positions are indicated in pos_from, pos_to fields (from ObjSideType dictionary (for instance, transcription 
 start, translation start, transcription end, translation end; see part 5)).
#### PfoTypeId
pfo_type_id, pto_type_id: type of the reference regulatory element, which used as point of origin for positions indicated in pos_from, pos_to fields
#### PositionFrom
pos_from, pos_to: first and last positions of the regulatory element
#### PositionFromGuid
pos_from_guid, pos_to_guid: id of the reference regulatory element, which used as point of origin for positions indicated in pos_from, pos_to fields
#### PositionTo
pos_from, pos_to: first and last positions of the regulatory element
#### PositionToGuid
pfo_side_guid, pto_side_guid: id of the relation between reference regulatory element and current regulatory element, 
 which positions are indicated in pos_from, pos_to fields (from ObjSideType dictionary (for instance, transcription 
 start, translation start, transcription end, translation end; see part 5)).
#### PtoSideGuid
pfo_side_guid, pto_side_guid: id of the relation between reference regulatory element and current regulatory element, 
 which positions are indicated in pos_from, pos_to fields (from ObjSideType dictionary (for instance, transcription 
 start, translation start, transcription end, translation end; see part 5)).
#### PtoTypeId
pfo_type_id, pto_type_id: type of the reference regulatory element, which used as point of origin for positions 
 indicated in pos_from, pos_to fields
#### Sequence
sequence: site sequence
#### Signature
signature: site signature (if site sequence is too short for certain localization in the genome, 
 annotator had to input longer sequence fragment in “signature” field). Signature must be at least 
 30 nt. signature - site signature (part of sequence long enough to find site in genome reliably 
 if site sequence is too short) (a number can be placed within this field to represent a particular 
 length of unknown sequence (N's)
#### SiteLen
site_len: site length
#### StructureSiteTypeGuid
structural_site_type_guid: id of the functional type of the site (from StructuralSiteType dictionary (see below, part 5)).

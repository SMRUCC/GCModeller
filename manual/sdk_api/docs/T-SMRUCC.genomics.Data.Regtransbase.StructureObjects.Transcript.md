---
title: Transcript
---

# Transcript
_namespace: [SMRUCC.genomics.Data.Regtransbase.StructureObjects](N-SMRUCC.genomics.Data.Regtransbase.StructureObjects.html)_



> 
>  CREATE TABLE `transcripts` (
>    `transcript_guid` int(11) NOT NULL DEFAULT '0',
>    `pkg_guid` int(11) NOT NULL DEFAULT '0',
>    `art_guid` int(11) NOT NULL DEFAULT '0',
>    `name` varchar(50) DEFAULT NULL,
>    `fl_real_name` int(1) DEFAULT NULL,
>    `genome_guid` int(11) DEFAULT NULL,
>    `pos_from` int(11) DEFAULT NULL,
>    `pos_from_guid` int(11) DEFAULT NULL,
>    `pfo_type_id` int(11) DEFAULT NULL,
>    `pfo_side_guid` int(11) DEFAULT NULL,
>    `pos_to` int(11) DEFAULT NULL,
>    `pos_to_guid` int(11) DEFAULT NULL,
>    `pto_type_id` int(11) DEFAULT NULL,
>    `pto_side_guid` int(11) DEFAULT NULL,
>    `tr_len` int(11) DEFAULT NULL,
>    `descript` blob,
>    `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
>    PRIMARY KEY (`transcript_guid`),
>    KEY `FK_transcripts-pkg_guid` (`pkg_guid`),
>    KEY `FK_transcripts-art_guid` (`art_guid`),
>    KEY `FK_transcripts-genome_guid` (`genome_guid`),
>    CONSTRAINT `FK_transcripts-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
>    CONSTRAINT `FK_transcripts-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
>    CONSTRAINT `FK_transcripts-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
>  ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
>  



### Properties

#### Guid
transcript_guid
#### pfo_side_guid
pfo_side_guid - location in this regulatory element to which start is binded (e.g. start, 
 end, transcription start, translation start etc.; link to dict_obj_side_types)
#### pfo_type_id
pfo_type_id - type of this regulatory element (link to obj_types)
#### pos_from
pos_from - start position (relative to some regulatory element)
#### pos_from_guid
pos_from_guid - regulatory element to which start of the secondary structure is binded 
 (link to some table with regulatory elements)
#### pos_to
pos_to - end position (relative to some regulatory element)
#### pos_to_guid
pos_to_guid - regulatory element dto which end position is binded (link to some table with
 regulatory elements)
#### pto_side_guid
pto_side_guid - location in this regulatory element to which end is binded (e.g. start, end, 
 transcription start, translation start etc.; link to dict_obj_side_types)
#### pto_type_id
pto_type_id - type of this regulatory element (link to obj_types)
#### tr_len
transcript length

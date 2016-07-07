---
title: sec_structures
---

# sec_structures
_namespace: [SMRUCC.genomics.Data.Regtransbase.StructureObjects](N-SMRUCC.genomics.Data.Regtransbase.StructureObjects.html)_

Secondary structure

> 
>  CREATE TABLE `sec_structures` (
>    `sec_struct_guid` int(11) NOT NULL DEFAULT '0',
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
>    `sequence` varchar(255) DEFAULT NULL,
>    `descript` blob,
>    PRIMARY KEY (`sec_struct_guid`),
>    KEY `FK_sec_structures-pkg_guid` (`pkg_guid`),
>    KEY `FK_sec_structures-art_guid` (`art_guid`),
>    KEY `FK_sec_structures-genome_guid` (`genome_guid`),
>    CONSTRAINT `FK_sec_structures-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
>    CONSTRAINT `FK_sec_structures-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
>    CONSTRAINT `FK_sec_structures-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
>  ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
>  



### Properties

#### sequence
sequence: sequence of the RNA fragment

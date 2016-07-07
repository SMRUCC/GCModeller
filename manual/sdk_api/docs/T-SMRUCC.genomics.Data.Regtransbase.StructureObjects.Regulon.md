---
title: Regulon
---

# Regulon
_namespace: [SMRUCC.genomics.Data.Regtransbase.StructureObjects](N-SMRUCC.genomics.Data.Regtransbase.StructureObjects.html)_



> 
>  CREATE TABLE `regulons` (
>    `regulon_guid` int(11) NOT NULL DEFAULT '0',
>    `pkg_guid` int(11) NOT NULL DEFAULT '0',
>    `art_guid` int(11) NOT NULL DEFAULT '0',
>    `name` varchar(50) DEFAULT NULL,
>    `fl_real_name` int(1) DEFAULT NULL,
>    `genome_guid` int(11) DEFAULT NULL,
>    `regulator_guid` int(11) DEFAULT NULL,
>    `descript` blob,
>    `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
>    PRIMARY KEY (`regulon_guid`),
>    KEY `FK_regulons-pkg_guid` (`pkg_guid`),
>    KEY `FK_regulons-art_guid` (`art_guid`),
>    KEY `FK_regulons-genome_guid` (`genome_guid`),
>    KEY `FK_regulons-regulator_guid` (`regulator_guid`),
>    CONSTRAINT `FK_regulons-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
>    CONSTRAINT `FK_regulons-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
>    CONSTRAINT `FK_regulons-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
>    CONSTRAINT `regulons_ibfk_1` FOREIGN KEY (`regulator_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
>  ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
>  SELECT * FROM dbregulation_update.sites;
>  



### Properties

#### Guid
regulon_guid: id of the regulator for the regulon

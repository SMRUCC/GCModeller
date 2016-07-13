---
title: Regulator
---

# Regulator
_namespace: [SMRUCC.genomics.Data.Regtransbase.StructureObjects](N-SMRUCC.genomics.Data.Regtransbase.StructureObjects.html)_



> 
>  CREATE TABLE `regulators` (
>    `regulator_guid` int(11) NOT NULL DEFAULT '0',
>    `pkg_guid` int(11) NOT NULL DEFAULT '0',
>    `art_guid` int(11) NOT NULL DEFAULT '0',
>    `name` varchar(50) DEFAULT NULL,
>    `fl_real_name` int(1) DEFAULT NULL,
>    `genome_guid` int(11) DEFAULT NULL,
>    `fl_prot_rna` int(1) DEFAULT NULL,
>    `regulator_type_guid` int(11) DEFAULT '0',
>    `gene_guid` int(11) DEFAULT NULL,
>    `ref_bank1` varchar(255) DEFAULT NULL,
>    `ref_bank2` varchar(255) DEFAULT NULL,
>    `ref_bank3` varchar(255) DEFAULT NULL,
>    `ref_bank4` varchar(255) DEFAULT NULL,
>    `consensus` varchar(50) DEFAULT NULL,
>    `family` varchar(20) DEFAULT NULL,
>    `descript` blob,
>    `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
>    PRIMARY KEY (`regulator_guid`),
>    KEY `FK_regulators-pkg_guid` (`pkg_guid`),
>    KEY `FK_regulators-art_guid` (`art_guid`),
>    KEY `FK_regulators-genome_guid` (`genome_guid`),
>    KEY `FK_regulators-regulator_type_guid` (`regulator_type_guid`),
>    KEY `FK_regulators-gene_guid` (`gene_guid`),
>    CONSTRAINT `FK_regulators-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
>    CONSTRAINT `FK_regulators-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
>    CONSTRAINT `FK_regulators-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
>    CONSTRAINT `FK_regulators-regulator_type_guid` FOREIGN KEY (`regulator_type_guid`) REFERENCES `dict_regulator_types` (`regulator_type_guid`),
>    CONSTRAINT `regulators_ibfk_1` FOREIGN KEY (`gene_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
>  ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
>  



### Properties

#### Consensus
consensus: binding site consensus for the regulator
#### Family
family: regulator family
#### flag_prot_rna
flag_prot_rna: Protein/RNA flag
#### GeneGuid
gene_guid: id of the gene encoded the regulator
#### Name
name - regulatory element name
#### RefBank1
ref_bank1 – ref_bank: id of the protein in external databases, such as NCBI

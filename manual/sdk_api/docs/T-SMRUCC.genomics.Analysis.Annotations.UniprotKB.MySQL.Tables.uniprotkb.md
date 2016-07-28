---
title: uniprotkb
---

# uniprotkb
_namespace: [SMRUCC.genomics.Analysis.Annotations.UniprotKB.MySQL.Tables](N-SMRUCC.genomics.Analysis.Annotations.UniprotKB.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `uniprotkb`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `uniprotkb` (
 `uniprot_id` varchar(45) NOT NULL COMMENT 'UniqueIdentifier Is the primary accession number of the UniProtKB entry.',
 `entry_name` varchar(45) DEFAULT NULL COMMENT 'EntryName Is the entry name of the UniProtKB entry.',
 `orgnsm_sp_name` tinytext COMMENT 'OrganismName Is the scientific name of the organism of the UniProtKB entry.',
 `gn` varchar(45) DEFAULT NULL COMMENT 'GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.',
 `pe` varchar(45) DEFAULT NULL COMMENT 'ProteinExistence Is the numerical value describing the evidence for the existence of the protein.',
 `sv` varchar(45) DEFAULT NULL COMMENT 'SequenceVersion Is the version number of the sequence.',
 `prot_name` tinytext COMMENT 'ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.',
 `length` int(11) DEFAULT NULL COMMENT 'length of the protein sequence',
 `sequence` text COMMENT 'protein sequence',
 PRIMARY KEY (`uniprot_id`),
 UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-12-03 20:59:22




### Properties

#### entry_name
EntryName Is the entry name of the UniProtKB entry.
#### gn
GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.
#### length
length of the protein sequence
#### orgnsm_sp_name
OrganismName Is the scientific name of the organism of the UniProtKB entry.
#### pe
ProteinExistence Is the numerical value describing the evidence for the existence of the protein.
#### prot_name
ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.
#### sequence
protein sequence
#### sv
SequenceVersion Is the version number of the sequence.
#### uniprot_id
UniqueIdentifier Is the primary accession number of the UniProtKB entry.

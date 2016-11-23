# gene
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `gene`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene` (
 `locus_tag` char(45) NOT NULL,
 `gene_name` mediumtext,
 `definition` mediumtext,
 `aa_seq` longtext,
 `nt_seq` longtext,
 `ec` tinytext,
 `modules` mediumtext,
 `diseases` mediumtext,
 `organism` varchar(45) DEFAULT NULL,
 `pathways` varchar(45) DEFAULT NULL,
 `uniprot` varchar(45) DEFAULT NULL COMMENT 'uniprot entry for this protein',
 `ncbi_entry` varchar(45) DEFAULT NULL,
 `kegg_sp` varchar(45) DEFAULT NULL COMMENT 'kegg species organism brief code',
 PRIMARY KEY (`locus_tag`),
 UNIQUE KEY `entry_UNIQUE` (`locus_tag`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




### Properties

#### kegg_sp
kegg species organism brief code
#### uniprot
uniprot entry for this protein

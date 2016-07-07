---
title: obj_name_genomes
---

# obj_name_genomes
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `obj_name_genomes`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `obj_name_genomes` (
 `obj_guid` int(11) NOT NULL DEFAULT '0',
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `art_guid` int(11) NOT NULL DEFAULT '0',
 `name` varchar(50) DEFAULT NULL,
 `genome_guid` int(11) DEFAULT NULL,
 `obj_type_id` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`obj_guid`),
 KEY `FK_obj_name_genomes-pkg_guid` (`pkg_guid`),
 KEY `FK_obj_name_genomes-art_guid` (`art_guid`),
 KEY `FK_obj_name_genomes-genome_guid` (`genome_guid`),
 CONSTRAINT `FK_obj_name_genomes-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
 CONSTRAINT `FK_obj_name_genomes-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
 CONSTRAINT `FK_obj_name_genomes-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





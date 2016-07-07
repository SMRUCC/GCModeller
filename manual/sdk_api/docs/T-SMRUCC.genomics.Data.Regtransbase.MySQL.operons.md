---
title: operons
---

# operons
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `operons`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `operons` (
 `operon_guid` int(11) NOT NULL DEFAULT '0',
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `art_guid` int(11) NOT NULL DEFAULT '0',
 `name` varchar(50) DEFAULT NULL,
 `fl_real_name` int(1) DEFAULT NULL,
 `genome_guid` int(11) DEFAULT NULL,
 `descript` blob,
 `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`operon_guid`),
 KEY `FK_operons-pkg_guid` (`pkg_guid`),
 KEY `FK_operons-art_guid` (`art_guid`),
 KEY `FK_operons-genome_guid` (`genome_guid`),
 CONSTRAINT `FK_operons-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
 CONSTRAINT `FK_operons-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
 CONSTRAINT `FK_operons-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





---
title: sites
---

# sites
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `sites`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `sites` (
 `site_guid` int(11) NOT NULL DEFAULT '0',
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `art_guid` int(11) NOT NULL DEFAULT '0',
 `name` varchar(50) DEFAULT NULL,
 `fl_real_name` int(1) DEFAULT NULL,
 `genome_guid` int(11) DEFAULT NULL,
 `func_site_type_guid` int(11) DEFAULT NULL,
 `struct_site_type_guid` int(11) DEFAULT NULL,
 `regulator_guid` int(11) DEFAULT '0',
 `fl_dna_rna` int(1) DEFAULT NULL,
 `pos_from` int(11) DEFAULT NULL,
 `pos_from_guid` int(11) DEFAULT NULL,
 `pfo_type_id` int(11) DEFAULT NULL,
 `pfo_side_guid` int(11) DEFAULT NULL,
 `pos_to` int(11) DEFAULT NULL,
 `pos_to_guid` int(11) DEFAULT NULL,
 `pto_type_id` int(11) DEFAULT NULL,
 `pto_side_guid` int(11) DEFAULT NULL,
 `site_len` int(11) DEFAULT NULL,
 `sequence` text,
 `signature` varchar(255) DEFAULT NULL,
 `descript` blob,
 `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`site_guid`),
 KEY `FK_sites-pkg_guid` (`pkg_guid`),
 KEY `FK_sites-art_guid` (`art_guid`),
 KEY `FK_sites-genome_guid` (`genome_guid`),
 KEY `FK_sites-func_site_type_guid` (`func_site_type_guid`),
 KEY `FK_sites-struct_site_type_guid` (`struct_site_type_guid`),
 KEY `FK_sites-regulator_guid` (`regulator_guid`),
 CONSTRAINT `FK_sites-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
 CONSTRAINT `FK_sites-func_site_type_guid` FOREIGN KEY (`func_site_type_guid`) REFERENCES `dict_func_site_types` (`func_site_type_guid`),
 CONSTRAINT `FK_sites-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
 CONSTRAINT `FK_sites-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
 CONSTRAINT `FK_sites-struct_site_type_guid` FOREIGN KEY (`struct_site_type_guid`) REFERENCES `dict_struct_site_types` (`struct_site_type_guid`),
 CONSTRAINT `sites_ibfk_1` FOREIGN KEY (`regulator_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





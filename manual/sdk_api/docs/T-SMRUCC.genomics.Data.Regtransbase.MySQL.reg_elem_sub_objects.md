---
title: reg_elem_sub_objects
---

# reg_elem_sub_objects
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `reg_elem_sub_objects`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reg_elem_sub_objects` (
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `art_guid` int(11) NOT NULL DEFAULT '0',
 `parent_guid` int(11) NOT NULL DEFAULT '0',
 `parent_type_id` int(11) DEFAULT NULL,
 `child_guid` int(11) NOT NULL DEFAULT '0',
 `child_type_id` int(11) DEFAULT NULL,
 `child_n` int(11) DEFAULT NULL,
 `strand` int(1) DEFAULT NULL,
 PRIMARY KEY (`parent_guid`,`child_guid`),
 KEY `FK_reg_elem_sub_objects-pkg_guid` (`pkg_guid`),
 KEY `FK_reg_elem_sub_objects-art_guid` (`art_guid`),
 KEY `child_guid` (`child_guid`),
 CONSTRAINT `FK_reg_elem_sub_objects-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
 CONSTRAINT `FK_reg_elem_sub_objects-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
 CONSTRAINT `reg_elem_sub_objects_ibfk_1` FOREIGN KEY (`child_guid`) REFERENCES `obj_name_genomes` (`obj_guid`),
 CONSTRAINT `reg_elem_sub_objects_ibfk_2` FOREIGN KEY (`parent_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





---
title: exp_sub_objects
---

# exp_sub_objects
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `exp_sub_objects`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `exp_sub_objects` (
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `art_guid` int(11) NOT NULL DEFAULT '0',
 `exp_guid` int(11) NOT NULL DEFAULT '0',
 `obj_guid` int(11) NOT NULL DEFAULT '0',
 `obj_type_id` int(11) DEFAULT NULL,
 `order_num` int(11) DEFAULT NULL,
 `strand` int(1) DEFAULT NULL,
 PRIMARY KEY (`exp_guid`,`obj_guid`),
 KEY `FK_exp_sub_objects-pkg_guid` (`pkg_guid`),
 KEY `FK_exp_sub_objects-art_guid` (`art_guid`),
 KEY `obj_guid` (`obj_guid`),
 CONSTRAINT `exp_sub_objects_ibfk_1` FOREIGN KEY (`obj_guid`) REFERENCES `obj_name_genomes` (`obj_guid`),
 CONSTRAINT `FK_exp_sub_objects-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
 CONSTRAINT `FK_exp_sub_objects-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
 CONSTRAINT `FK_exp_sub_objects-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





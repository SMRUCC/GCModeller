---
title: exp2result_types
---

# exp2result_types
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `exp2result_types`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `exp2result_types` (
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `art_guid` int(11) NOT NULL DEFAULT '0',
 `exp_guid` int(11) NOT NULL DEFAULT '0',
 `exp_result_type_guid` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`exp_guid`,`exp_result_type_guid`),
 KEY `FK_exp2result_types-pkg_guid` (`pkg_guid`),
 KEY `FK_exp2result_types-art_guid` (`art_guid`),
 KEY `FK_exp2result_types-exp_result_type_guid` (`exp_result_type_guid`),
 CONSTRAINT `FK_exp2result_types-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
 CONSTRAINT `FK_exp2result_types-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
 CONSTRAINT `FK_exp2result_types-exp_result_type_guid` FOREIGN KEY (`exp_result_type_guid`) REFERENCES `dict_exp_result_types` (`exp_result_type_guid`),
 CONSTRAINT `FK_exp2result_types-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





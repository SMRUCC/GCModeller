---
title: exp2effectors
---

# exp2effectors
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `exp2effectors`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `exp2effectors` (
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `art_guid` int(11) NOT NULL DEFAULT '0',
 `exp_guid` int(11) NOT NULL DEFAULT '0',
 `effector_guid` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`exp_guid`,`effector_guid`),
 KEY `FK_exp2effectors-pkg_guid` (`pkg_guid`),
 KEY `FK_exp2effectors-art_guid` (`art_guid`),
 KEY `FK_exp2effectors-effector_guid` (`effector_guid`),
 CONSTRAINT `FK_exp2effectors-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
 CONSTRAINT `FK_exp2effectors-effector_guid` FOREIGN KEY (`effector_guid`) REFERENCES `dict_effectors` (`effector_guid`),
 CONSTRAINT `FK_exp2effectors-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
 CONSTRAINT `FK_exp2effectors-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





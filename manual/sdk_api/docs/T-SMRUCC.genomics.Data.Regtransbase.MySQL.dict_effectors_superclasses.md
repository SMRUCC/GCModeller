---
title: dict_effectors_superclasses
---

# dict_effectors_superclasses
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `dict_effectors_superclasses`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dict_effectors_superclasses` (
 `effector_superclass_guid` int(11) NOT NULL DEFAULT '0',
 `name` varchar(100) NOT NULL DEFAULT '',
 `parent_guid` int(11) DEFAULT NULL,
 `idx` int(11) NOT NULL DEFAULT '0',
 `left_idx` int(11) NOT NULL DEFAULT '0',
 `right_idx` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`effector_superclass_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





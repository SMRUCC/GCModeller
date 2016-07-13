---
title: dict_effectors
---

# dict_effectors
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `dict_effectors`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dict_effectors` (
 `effector_guid` int(11) NOT NULL DEFAULT '0',
 `name` text,
 `description` mediumtext NOT NULL,
 `effector_superclass_guid` int(11) DEFAULT NULL,
 PRIMARY KEY (`effector_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





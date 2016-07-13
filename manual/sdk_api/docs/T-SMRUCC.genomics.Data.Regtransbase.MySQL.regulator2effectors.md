---
title: regulator2effectors
---

# regulator2effectors
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `regulator2effectors`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulator2effectors` (
 `regulator_guid` int(10) unsigned NOT NULL DEFAULT '0',
 `effector_guid` int(10) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`regulator_guid`,`effector_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





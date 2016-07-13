---
title: dict_regulator_types
---

# dict_regulator_types
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `dict_regulator_types`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dict_regulator_types` (
 `regulator_type_guid` int(11) NOT NULL DEFAULT '0',
 `name` varchar(100) NOT NULL DEFAULT '',
 PRIMARY KEY (`regulator_type_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





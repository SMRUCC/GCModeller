---
title: dict_func_site_types
---

# dict_func_site_types
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `dict_func_site_types`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dict_func_site_types` (
 `func_site_type_guid` int(11) NOT NULL DEFAULT '0',
 `name` varchar(100) DEFAULT NULL,
 PRIMARY KEY (`func_site_type_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





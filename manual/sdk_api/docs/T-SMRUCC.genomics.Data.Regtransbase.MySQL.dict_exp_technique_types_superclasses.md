---
title: dict_exp_technique_types_superclasses
---

# dict_exp_technique_types_superclasses
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `dict_exp_technique_types_superclasses`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dict_exp_technique_types_superclasses` (
 `exp_technique_type_superclass_guid` int(10) unsigned NOT NULL DEFAULT '0',
 `name` varchar(255) NOT NULL DEFAULT '',
 PRIMARY KEY (`exp_technique_type_superclass_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





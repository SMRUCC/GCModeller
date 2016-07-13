---
title: obj_sub_types
---

# obj_sub_types
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `obj_sub_types`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `obj_sub_types` (
 `parent_obj_type_id` int(11) NOT NULL DEFAULT '0',
 `child_obj_type_id` int(11) NOT NULL DEFAULT '0'
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





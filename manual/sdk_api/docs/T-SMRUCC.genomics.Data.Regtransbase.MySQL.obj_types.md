---
title: obj_types
---

# obj_types
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `obj_types`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `obj_types` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `obj_type_name` varchar(50) DEFAULT NULL,
 `obj_tbname` varchar(50) DEFAULT NULL,
 `cp_order` int(11) DEFAULT NULL,
 PRIMARY KEY (`id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





﻿# h_dict_struct_site_types
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `h_dict_struct_site_types`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `h_dict_struct_site_types` (
 `pkg_name` varchar(100) NOT NULL DEFAULT '',
 `db_name` varchar(100) DEFAULT NULL,
 PRIMARY KEY (`pkg_name`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





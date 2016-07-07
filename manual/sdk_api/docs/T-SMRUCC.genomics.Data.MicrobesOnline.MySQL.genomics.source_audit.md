---
title: source_audit
---

# source_audit
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `source_audit`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `source_audit` (
 `source_id` varchar(255) DEFAULT NULL,
 `source_fullpath` varchar(255) DEFAULT NULL,
 `source_path` varchar(255) DEFAULT NULL,
 `source_type` varchar(255) DEFAULT NULL,
 `source_md5` varchar(32) DEFAULT NULL,
 `source_parsetime` int(11) DEFAULT NULL,
 `source_mtime` int(11) DEFAULT NULL,
 KEY `fa1` (`source_path`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





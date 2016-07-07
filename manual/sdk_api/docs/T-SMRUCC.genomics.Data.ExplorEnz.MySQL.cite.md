---
title: cite
---

# cite
_namespace: [SMRUCC.genomics.Data.ExplorEnz.MySQL](N-SMRUCC.genomics.Data.ExplorEnz.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `cite`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `cite` (
 `cite_key` varchar(48) NOT NULL DEFAULT '',
 `ec_num` varchar(12) NOT NULL DEFAULT '',
 `ref_num` int(11) DEFAULT NULL,
 `acc_no` int(11) NOT NULL AUTO_INCREMENT,
 `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`acc_no`)
 ) ENGINE=MyISAM AUTO_INCREMENT=47359 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





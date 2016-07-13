---
title: dbxref
---

# dbxref
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dbxref` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `xref_dbname` varchar(55) NOT NULL DEFAULT '',
 `xref_key` varchar(255) NOT NULL DEFAULT '',
 `xref_keytype` varchar(32) DEFAULT NULL,
 `xref_desc` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `xref_key` (`xref_key`,`xref_dbname`),
 UNIQUE KEY `dx0` (`id`),
 KEY `dx1` (`xref_dbname`),
 KEY `dx2` (`xref_key`),
 KEY `dx3` (`id`,`xref_dbname`),
 KEY `dx4` (`id`,`xref_key`,`xref_dbname`),
 KEY `dx5` (`id`,`xref_key`)
 ) ENGINE=MyISAM AUTO_INCREMENT=39457 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





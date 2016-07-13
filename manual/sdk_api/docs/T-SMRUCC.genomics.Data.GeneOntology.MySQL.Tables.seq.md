---
title: seq
---

# seq
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `seq`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `seq` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `display_id` varchar(64) DEFAULT NULL,
 `description` varchar(255) DEFAULT NULL,
 `seq` mediumtext,
 `seq_len` int(11) DEFAULT NULL,
 `md5checksum` varchar(32) DEFAULT NULL,
 `moltype` varchar(25) DEFAULT NULL,
 `timestamp` int(11) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `seq0` (`id`),
 UNIQUE KEY `display_id` (`display_id`,`md5checksum`),
 KEY `seq1` (`display_id`),
 KEY `seq2` (`md5checksum`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





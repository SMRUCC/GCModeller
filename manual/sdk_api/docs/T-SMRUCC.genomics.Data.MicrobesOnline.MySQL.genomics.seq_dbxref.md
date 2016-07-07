---
title: seq_dbxref
---

# seq_dbxref
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `seq_dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `seq_dbxref` (
 `seq_id` int(11) NOT NULL DEFAULT '0',
 `dbxref_id` int(11) NOT NULL DEFAULT '0',
 UNIQUE KEY `seq_id` (`seq_id`,`dbxref_id`),
 KEY `seqx0` (`seq_id`),
 KEY `seqx1` (`dbxref_id`),
 KEY `seqx2` (`seq_id`,`dbxref_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





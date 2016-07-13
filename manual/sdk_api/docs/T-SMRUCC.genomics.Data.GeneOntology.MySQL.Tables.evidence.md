---
title: evidence
---

# evidence
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `evidence`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `evidence` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `code` varchar(8) NOT NULL,
 `association_id` int(11) NOT NULL,
 `dbxref_id` int(11) NOT NULL,
 `seq_acc` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `association_id` (`association_id`,`dbxref_id`,`code`),
 UNIQUE KEY `ev0` (`id`),
 UNIQUE KEY `ev5` (`id`,`association_id`),
 UNIQUE KEY `ev6` (`id`,`code`,`association_id`),
 KEY `ev1` (`association_id`),
 KEY `ev2` (`code`),
 KEY `ev3` (`dbxref_id`),
 KEY `ev4` (`association_id`,`code`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





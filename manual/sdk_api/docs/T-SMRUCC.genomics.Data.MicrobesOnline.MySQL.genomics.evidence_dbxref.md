---
title: evidence_dbxref
---

# evidence_dbxref
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `evidence_dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `evidence_dbxref` (
 `evidence_id` int(11) NOT NULL DEFAULT '0',
 `dbxref_id` int(11) NOT NULL DEFAULT '0',
 KEY `evx1` (`evidence_id`),
 KEY `evx2` (`dbxref_id`),
 KEY `evx3` (`evidence_id`,`dbxref_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





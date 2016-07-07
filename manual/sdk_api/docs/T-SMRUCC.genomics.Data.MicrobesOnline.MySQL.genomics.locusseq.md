---
title: locusseq
---

# locusseq
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locusseq`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locusseq` (
 `locusId` int(10) unsigned DEFAULT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `sequence` longblob,
 UNIQUE KEY `orfId_version` (`locusId`,`version`),
 KEY `Index_Orf` (`locusId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1 MAX_ROWS=1000000000 AVG_ROW_LENGTH=1000;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





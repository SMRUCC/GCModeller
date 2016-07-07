---
title: transcriptseq
---

# transcriptseq
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `transcriptseq`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `transcriptseq` (
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `sequence` longblob,
 PRIMARY KEY (`locusId`,`version`),
 KEY `Index_Orf` (`locusId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





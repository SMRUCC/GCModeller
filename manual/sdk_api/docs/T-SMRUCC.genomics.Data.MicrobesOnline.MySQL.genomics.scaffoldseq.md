---
title: scaffoldseq
---

# scaffoldseq
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `scaffoldseq`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `scaffoldseq` (
 `scaffoldId` int(10) unsigned NOT NULL DEFAULT '0',
 `sequence` longblob,
 PRIMARY KEY (`scaffoldId`),
 KEY `Index_Scaffold` (`scaffoldId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1 MAX_ROWS=1000000000 AVG_ROW_LENGTH=100000;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





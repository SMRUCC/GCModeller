---
title: annotationdetail
---

# annotationdetail
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `annotationdetail`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `annotationdetail` (
 `annotationId` int(10) unsigned NOT NULL DEFAULT '0',
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `type` enum('name','synonym','description','ecNum','go','comment') NOT NULL DEFAULT 'name',
 `action` enum('append','replace','delete') NOT NULL DEFAULT 'append',
 `annotation` text NOT NULL,
 KEY `annotationId` (`annotationId`),
 KEY `orfId` (`locusId`),
 KEY `type` (`type`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





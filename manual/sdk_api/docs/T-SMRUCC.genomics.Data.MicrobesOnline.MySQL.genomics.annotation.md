---
title: annotation
---

# annotation
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `annotation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `annotation` (
 `annotationId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `source` varchar(64) NOT NULL DEFAULT '',
 `date` int(10) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`annotationId`),
 KEY `source` (`source`)
 ) ENGINE=MyISAM AUTO_INCREMENT=21884003 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





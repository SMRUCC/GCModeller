---
title: crisprfamily
---

# crisprfamily
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `crisprfamily`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `crisprfamily` (
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `familyId` int(10) unsigned NOT NULL DEFAULT '0',
 `type` smallint(5) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`locusId`,`familyId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





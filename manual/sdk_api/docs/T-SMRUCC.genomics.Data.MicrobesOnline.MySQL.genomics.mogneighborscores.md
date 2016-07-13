---
title: mogneighborscores
---

# mogneighborscores
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `mogneighborscores`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `mogneighborscores` (
 `mog1` int(10) unsigned NOT NULL DEFAULT '0',
 `mog2` int(10) unsigned NOT NULL DEFAULT '0',
 `score` float NOT NULL DEFAULT '0',
 `nTaxGroupsBoth` int(10) unsigned NOT NULL DEFAULT '0',
 `nTaxGroups1` int(10) unsigned NOT NULL DEFAULT '0',
 `nTaxGroups2` int(10) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`mog1`,`mog2`),
 KEY `mog2` (`mog2`,`mog1`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





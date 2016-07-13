---
title: locus2rtb
---

# locus2rtb
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2rtb`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2rtb` (
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `rtb_seqfeature_id` int(10) unsigned NOT NULL DEFAULT '0',
 `rtb_characterized` int(1) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`locusId`),
 KEY `rtb_seqfeature_id` (`rtb_seqfeature_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





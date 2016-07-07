---
title: taxparentchild
---

# taxparentchild
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `taxparentchild`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `taxparentchild` (
 `parentId` int(10) NOT NULL DEFAULT '0',
 `childId` int(10) NOT NULL DEFAULT '0',
 `nDistance` int(10) unsigned NOT NULL DEFAULT '0',
 UNIQUE KEY `combined` (`parentId`,`childId`),
 KEY `parentId` (`parentId`),
 KEY `childId` (`childId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





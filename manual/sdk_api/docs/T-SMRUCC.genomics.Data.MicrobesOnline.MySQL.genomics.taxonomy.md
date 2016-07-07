---
title: taxonomy
---

# taxonomy
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `taxonomy`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `taxonomy` (
 `taxonomyId` int(10) NOT NULL DEFAULT '0',
 `name` varchar(255) DEFAULT NULL,
 `placement` int(10) DEFAULT NULL,
 `shortName` varchar(100) DEFAULT NULL,
 `taxDispGroupId` int(10) DEFAULT NULL,
 `created` date DEFAULT NULL,
 `PMID` varchar(50) DEFAULT NULL,
 `Publication` varchar(255) DEFAULT NULL,
 `Uniprot` varchar(10) DEFAULT NULL,
 `ncbiProjectId` int(10) DEFAULT NULL,
 PRIMARY KEY (`taxonomyId`),
 KEY `taxDispGroupId` (`taxDispGroupId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





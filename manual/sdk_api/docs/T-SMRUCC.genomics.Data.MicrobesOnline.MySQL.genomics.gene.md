---
title: gene
---

# gene
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `gene`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene` (
 `taxId` int(20) unsigned NOT NULL,
 `geneId` varchar(100) NOT NULL,
 `name` varchar(20) DEFAULT NULL,
 `evidence` varchar(100) DEFAULT NULL,
 `paralog` int(10) unsigned DEFAULT NULL,
 `product` varchar(100) DEFAULT NULL,
 `productInfo` varchar(255) DEFAULT NULL,
 `productType` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`geneId`),
 KEY `taxId` (`taxId`),
 KEY `paralog` (`paralog`),
 KEY `product` (`product`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





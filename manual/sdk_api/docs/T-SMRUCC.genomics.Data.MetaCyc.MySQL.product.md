---
title: product
---

# product
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `product`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `product` (
 `ReactionWID` bigint(20) NOT NULL,
 `OtherWID` bigint(20) NOT NULL,
 `Coefficient` smallint(6) NOT NULL,
 KEY `FK_Product` (`ReactionWID`),
 CONSTRAINT `FK_Product` FOREIGN KEY (`ReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





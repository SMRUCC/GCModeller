---
title: featuredimensionwidfeaturewid
---

# featuredimensionwidfeaturewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `featuredimensionwidfeaturewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `featuredimensionwidfeaturewid` (
 `FeatureDimensionWID` bigint(20) NOT NULL,
 `FeatureWID` bigint(20) NOT NULL,
 KEY `FK_FeatureDimensionWIDFeatur1` (`FeatureDimensionWID`),
 KEY `FK_FeatureDimensionWIDFeatur2` (`FeatureWID`),
 CONSTRAINT `FK_FeatureDimensionWIDFeatur1` FOREIGN KEY (`FeatureDimensionWID`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FeatureDimensionWIDFeatur2` FOREIGN KEY (`FeatureWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





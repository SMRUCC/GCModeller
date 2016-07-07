---
title: featurewidfeaturewid
---

# featurewidfeaturewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `featurewidfeaturewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `featurewidfeaturewid` (
 `FeatureWID1` bigint(20) NOT NULL,
 `FeatureWID2` bigint(20) NOT NULL,
 KEY `FK_FeatureWIDFeatureWID1` (`FeatureWID1`),
 KEY `FK_FeatureWIDFeatureWID2` (`FeatureWID2`),
 CONSTRAINT `FK_FeatureWIDFeatureWID1` FOREIGN KEY (`FeatureWID1`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FeatureWIDFeatureWID2` FOREIGN KEY (`FeatureWID2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





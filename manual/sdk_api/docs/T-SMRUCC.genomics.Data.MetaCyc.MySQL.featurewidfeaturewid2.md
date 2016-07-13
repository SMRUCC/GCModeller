---
title: featurewidfeaturewid2
---

# featurewidfeaturewid2
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `featurewidfeaturewid2`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `featurewidfeaturewid2` (
 `FeatureWID1` bigint(20) NOT NULL,
 `FeatureWID2` bigint(20) NOT NULL,
 KEY `FK_FeatureWIDFeatureWID21` (`FeatureWID1`),
 KEY `FK_FeatureWIDFeatureWID22` (`FeatureWID2`),
 CONSTRAINT `FK_FeatureWIDFeatureWID21` FOREIGN KEY (`FeatureWID1`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FeatureWIDFeatureWID22` FOREIGN KEY (`FeatureWID2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





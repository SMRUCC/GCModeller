---
title: arraydesignwidcompositegrpwid
---

# arraydesignwidcompositegrpwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `arraydesignwidcompositegrpwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `arraydesignwidcompositegrpwid` (
 `ArrayDesignWID` bigint(20) NOT NULL,
 `CompositeGroupWID` bigint(20) NOT NULL,
 KEY `FK_ArrayDesignWIDCompositeGr1` (`ArrayDesignWID`),
 KEY `FK_ArrayDesignWIDCompositeGr2` (`CompositeGroupWID`),
 CONSTRAINT `FK_ArrayDesignWIDCompositeGr1` FOREIGN KEY (`ArrayDesignWID`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ArrayDesignWIDCompositeGr2` FOREIGN KEY (`CompositeGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





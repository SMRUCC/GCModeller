---
title: arraydesignwidcontactwid
---

# arraydesignwidcontactwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `arraydesignwidcontactwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `arraydesignwidcontactwid` (
 `ArrayDesignWID` bigint(20) NOT NULL,
 `ContactWID` bigint(20) NOT NULL,
 KEY `FK_ArrayDesignWIDContactWID1` (`ArrayDesignWID`),
 KEY `FK_ArrayDesignWIDContactWID2` (`ContactWID`),
 CONSTRAINT `FK_ArrayDesignWIDContactWID1` FOREIGN KEY (`ArrayDesignWID`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ArrayDesignWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





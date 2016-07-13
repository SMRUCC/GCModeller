---
title: kegg2taxonomy
---

# kegg2taxonomy
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `kegg2taxonomy`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `kegg2taxonomy` (
 `keggOrgId` varchar(5) NOT NULL DEFAULT '',
 `taxonomyId` int(10) NOT NULL DEFAULT '0',
 PRIMARY KEY (`keggOrgId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





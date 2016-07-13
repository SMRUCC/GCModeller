---
title: sigmanetworkset_tmp
---

# sigmanetworkset_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `sigmanetworkset_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `sigmanetworkset_tmp` (
 `sigma_promoter` varchar(80) DEFAULT NULL,
 `snws_promoter_name` varchar(100) DEFAULT NULL,
 `gene_coding` varchar(100) DEFAULT NULL,
 `gene_regulated` varchar(100) DEFAULT NULL,
 `bnumber` varchar(10) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





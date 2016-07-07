---
title: comments
---

# comments
_namespace: [SMRUCC.genomics.Data.ChEBI.Tables](N-SMRUCC.genomics.Data.ChEBI.Tables.html)_

--
 
 DROP TABLE IF EXISTS `comments`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `comments` (
 `id` int(11) NOT NULL,
 `compound_id` int(11) NOT NULL,
 `text` text NOT NULL,
 `created_on` datetime NOT NULL,
 `datatype` varchar(80) DEFAULT NULL,
 `datatype_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `compound_id` (`compound_id`),
 CONSTRAINT `FK_COMMENTS_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





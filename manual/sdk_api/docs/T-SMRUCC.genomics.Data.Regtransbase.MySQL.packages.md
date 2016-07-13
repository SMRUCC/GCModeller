---
title: packages
---

# packages
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `packages`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `packages` (
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `title` char(50) DEFAULT NULL,
 `annotator_id` int(11) DEFAULT NULL,
 `article_num` int(11) DEFAULT NULL,
 `pkg_state` int(11) NOT NULL DEFAULT '0',
 `pkg_state_date` char(10) DEFAULT NULL,
 `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`pkg_guid`),
 KEY `annotator_id` (`annotator_id`),
 CONSTRAINT `packages_ibfk_1` FOREIGN KEY (`annotator_id`) REFERENCES `db_users` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





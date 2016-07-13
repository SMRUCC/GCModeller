---
title: articles
---

# articles
_namespace: [SMRUCC.genomics.Data.Regtransbase.MySQL](N-SMRUCC.genomics.Data.Regtransbase.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `articles`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `articles` (
 `art_guid` int(11) NOT NULL DEFAULT '0',
 `pkg_guid` int(11) NOT NULL DEFAULT '0',
 `title` varchar(255) DEFAULT NULL,
 `author` varchar(255) DEFAULT NULL,
 `pmid` varchar(20) DEFAULT NULL,
 `art_journal` varchar(50) DEFAULT NULL,
 `art_year` varchar(10) DEFAULT NULL,
 `art_month` varchar(10) DEFAULT NULL,
 `art_volume` varchar(10) DEFAULT NULL,
 `art_issue` varchar(10) DEFAULT NULL,
 `art_pages` varchar(20) DEFAULT NULL,
 `art_abstruct` blob,
 `exp_num` int(11) DEFAULT NULL,
 `art_state` int(11) DEFAULT '0',
 `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`art_guid`),
 UNIQUE KEY `pmid_unique` (`pmid`),
 KEY `FK_articles-pkg_guid` (`pkg_guid`),
 CONSTRAINT `FK_articles-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





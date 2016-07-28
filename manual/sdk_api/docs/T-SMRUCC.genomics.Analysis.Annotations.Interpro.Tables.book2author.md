---
title: book2author
---

# book2author
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `book2author`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `book2author` (
 `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `author_id` int(9) NOT NULL,
 `order_in` int(3) NOT NULL,
 PRIMARY KEY (`isbn`,`order_in`,`author_id`),
 UNIQUE KEY `uq_book2author$1` (`isbn`,`order_in`),
 KEY `i_book2author$fk_author_id` (`author_id`),
 CONSTRAINT `fk_book2author$author_id` FOREIGN KEY (`author_id`) REFERENCES `author` (`author_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
 CONSTRAINT `fk_book2author$isbn` FOREIGN KEY (`isbn`) REFERENCES `book` (`isbn`) ON DELETE NO ACTION ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --





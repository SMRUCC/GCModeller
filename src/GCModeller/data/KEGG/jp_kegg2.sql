CREATE DATABASE  IF NOT EXISTS `jp_kegg2` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `jp_kegg2`;
-- MySQL dump 10.13  Distrib 5.7.9, for Win64 (x86_64)
--
-- Host: localhost    Database: jp_kegg2
-- ------------------------------------------------------
-- Server version	5.7.12-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `class_ko00001_orthology`
--

DROP TABLE IF EXISTS `class_ko00001_orthology`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `class_ko00001_orthology` (
  `Orthology` int(11) NOT NULL COMMENT '``data_orthology``基因同源数据表之中的唯一数字编号',
  `KEGG` varchar(45) DEFAULT NULL COMMENT '当前的这个基因同源的KO编号',
  `name` varchar(45) DEFAULT NULL COMMENT '基因名',
  `function` varchar(45) DEFAULT NULL COMMENT '功能描述',
  `level_A` varchar(45) DEFAULT NULL COMMENT '代谢途径大分类',
  `level_B` varchar(45) DEFAULT NULL COMMENT '代谢途径小分类',
  `level_C` varchar(45) DEFAULT NULL COMMENT 'KEGG pathway.当前的这个参考基因同源所处的代谢途径',
  PRIMARY KEY (`Orthology`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG的基因同源分类';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `class_ko00001_pathway`
--

DROP TABLE IF EXISTS `class_ko00001_pathway`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `class_ko00001_pathway` (
  `pathway` int(11) NOT NULL,
  `KEGG` varchar(45) DEFAULT NULL,
  `level_A` varchar(45) DEFAULT NULL,
  `level_B` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pathway`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `class_orthology_genes`
--

DROP TABLE IF EXISTS `class_orthology_genes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `class_orthology_genes` (
  `uid` int(11) NOT NULL,
  `orthology` int(11) NOT NULL COMMENT '直系同源表的数字编号',
  `locus_tag` varchar(45) NOT NULL COMMENT '基因号',
  `geneName` varchar(45) DEFAULT NULL COMMENT '基因名，因为有些基因还是没有名称的，所以在这里可以为空',
  `organism` varchar(45) NOT NULL COMMENT 'KEGG物种简写编号',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个数据表描述了uniprot之中的基因蛋白数据之间的基因同源关系';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `data_compounds`
--

DROP TABLE IF EXISTS `data_compounds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `data_compounds` (
  `uid` int(11) NOT NULL,
  `KEGG` varchar(45) NOT NULL COMMENT 'KEGG代谢物编号',
  `names` varchar(45) DEFAULT NULL,
  `formula` varchar(45) DEFAULT NULL COMMENT '分子式',
  `mass` varchar(45) DEFAULT NULL COMMENT '物质质量',
  `mol_weight` varchar(45) DEFAULT NULL COMMENT '分子质量',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `data_enzyme`
--

DROP TABLE IF EXISTS `data_enzyme`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `data_enzyme` (
  `uid` int(11) NOT NULL,
  `EC` varchar(45) NOT NULL COMMENT 'EC编号',
  `name` varchar(45) DEFAULT NULL COMMENT '酶名称',
  `sysname` varchar(45) DEFAULT NULL COMMENT '生物酶的系统名称',
  `Reaction(KEGG)_uid` varchar(45) DEFAULT NULL COMMENT '``data_reactions``表之中的数字编号',
  `Reaction(KEGG)` varchar(45) DEFAULT NULL COMMENT 'KEGG之中所能够被催化的生物过程的编号',
  `Reaction(IUBMB)` varchar(45) DEFAULT NULL,
  `Substrate` varchar(45) DEFAULT NULL,
  `Product` varchar(45) DEFAULT NULL,
  `Comment` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='酶';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `data_modules`
--

DROP TABLE IF EXISTS `data_modules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `data_modules` (
  `uid` int(11) NOT NULL,
  `KEGG` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `definition` varchar(45) DEFAULT NULL,
  `map` varchar(45) DEFAULT NULL COMMENT 'image -> gzip -> base64 string',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `data_organisms`
--

DROP TABLE IF EXISTS `data_organisms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `data_organisms` (
  `uid` int(11) NOT NULL,
  `KEGG_sp` varchar(8) NOT NULL,
  `scientific name` varchar(45) DEFAULT NULL,
  `domain` varchar(45) DEFAULT NULL,
  `kingdom` varchar(45) DEFAULT NULL COMMENT '界',
  `phylum` varchar(45) DEFAULT NULL COMMENT '门',
  `class` varchar(45) DEFAULT NULL COMMENT '纲',
  `order` varchar(45) DEFAULT NULL COMMENT '目',
  `family` varchar(45) DEFAULT NULL COMMENT '科',
  `genus` varchar(45) DEFAULT NULL COMMENT '属',
  `species` varchar(45) DEFAULT NULL COMMENT '种',
  PRIMARY KEY (`KEGG_sp`,`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`),
  UNIQUE KEY `KEGG_sp_UNIQUE` (`KEGG_sp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='taxonomy.(物种分类数据)\n生物主要分类等级是门（phylum）、纲（class）、目（order）、科（family）、属（genus）、种（species）。种以下还有亚种（subspecies，缩写成subsp.），植物还有变种（variety，缩写成var.）。有时还有一些辅助等级，实在主要分类等级术语前加前缀超（super-）、亚（sub-）.在亚纲、亚目之下有时还分别设置次纲（infraclass）和次目（infraorder）等。';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `data_orthology`
--

DROP TABLE IF EXISTS `data_orthology`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `data_orthology` (
  `uid` int(11) NOT NULL,
  `KEGG` varchar(45) DEFAULT NULL COMMENT 'KO编号',
  `name` varchar(45) DEFAULT NULL,
  `definition` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG基因直系同源数据';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `data_pathway`
--

DROP TABLE IF EXISTS `data_pathway`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `data_pathway` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `KO` varchar(45) NOT NULL,
  `description` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `map` varchar(45) DEFAULT NULL COMMENT 'image -> gzip -> base64 string',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`),
  UNIQUE KEY `KO_UNIQUE` (`KO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='参考代谢途径的定义';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `data_reactions`
--

DROP TABLE IF EXISTS `data_reactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `data_reactions` (
  `uid` int(11) NOT NULL,
  `KEGG` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `definition` varchar(45) DEFAULT NULL,
  `comment` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `data_references`
--

DROP TABLE IF EXISTS `data_references`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `data_references` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `pmid` int(11) NOT NULL,
  `journal` varchar(45) DEFAULT NULL,
  `title` varchar(45) NOT NULL,
  `authors` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `link_enzymes`
--

DROP TABLE IF EXISTS `link_enzymes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `link_enzymes` (
  `enzyme` int(11) NOT NULL,
  `EC` varchar(45) DEFAULT NULL,
  `database` varchar(45) DEFAULT NULL,
  `ID` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`enzyme`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `xref_module_reactions`
--

DROP TABLE IF EXISTS `xref_module_reactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `xref_module_reactions` (
  `module` int(11) NOT NULL,
  `reaction` int(11) NOT NULL,
  `KEGG` varchar(45) NOT NULL COMMENT '代谢反应的KEGG编号',
  PRIMARY KEY (`module`,`reaction`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代谢反应和生物模块之间的关系';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `xref_pathway_compounds`
--

DROP TABLE IF EXISTS `xref_pathway_compounds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `xref_pathway_compounds` (
  `pathway` int(11) NOT NULL,
  `compound` int(11) NOT NULL COMMENT '``data_compounds``表之中的唯一数字编号',
  `KEGG` varchar(45) NOT NULL COMMENT 'KEGG compound id.(KEGG代谢物的编号)',
  `name` varchar(45) DEFAULT NULL COMMENT '代谢物的名称',
  PRIMARY KEY (`pathway`,`compound`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代谢途径之中所包含有的代谢物的列表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `xref_pathway_genes`
--

DROP TABLE IF EXISTS `xref_pathway_genes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `xref_pathway_genes` (
  `pathway` int(11) NOT NULL,
  `gene` int(11) NOT NULL,
  `gene_KO` varchar(45) DEFAULT NULL COMMENT '目标基因的KO分类编号',
  `locus_tag` varchar(45) DEFAULT NULL COMMENT '基因号',
  `gene_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pathway`,`gene`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代谢途径和所属于该代谢途径对象的基因之间的关系表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `xref_pathway_modules`
--

DROP TABLE IF EXISTS `xref_pathway_modules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `xref_pathway_modules` (
  `pathway` int(11) NOT NULL,
  `module` int(11) NOT NULL,
  `KO` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pathway`,`module`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `xref_pathway_references`
--

DROP TABLE IF EXISTS `xref_pathway_references`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `xref_pathway_references` (
  `pathway` int(11) NOT NULL,
  `reference` int(11) NOT NULL,
  `title` varchar(45) DEFAULT NULL COMMENT '文献的标题',
  PRIMARY KEY (`pathway`,`reference`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代谢途径的参考文献';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'jp_kegg2'
--

--
-- Dumping routines for database 'jp_kegg2'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-04-30 12:56:55

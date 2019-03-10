CREATE DATABASE  IF NOT EXISTS `warehouse` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `warehouse`;
-- MySQL dump 10.13  Distrib 5.6.13, for Win32 (x86)
--
-- Host: localhost    Database: warehouse
-- ------------------------------------------------------
-- Server version	5.6.17

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
-- Table structure for table `archive`
--

DROP TABLE IF EXISTS `archive`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `archive` (
  `WID` bigint(20) NOT NULL,
  `OtherWID` bigint(20) NOT NULL,
  `Format` varchar(10) NOT NULL,
  `Contents` longblob,
  `URL` text,
  `ToolName` varchar(50) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `array_`
--

DROP TABLE IF EXISTS `array_`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `array_` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `ArrayIdentifier` varchar(255) DEFAULT NULL,
  `ArrayXOrigin` float DEFAULT NULL,
  `ArrayYOrigin` float DEFAULT NULL,
  `OriginRelativeTo` varchar(255) DEFAULT NULL,
  `ArrayDesign` bigint(20) DEFAULT NULL,
  `Information` bigint(20) DEFAULT NULL,
  `ArrayGroup` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Array_1` (`DataSetWID`),
  KEY `FK_Array_3` (`ArrayDesign`),
  KEY `FK_Array_4` (`Information`),
  KEY `FK_Array_5` (`ArrayGroup`),
  CONSTRAINT `FK_Array_1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Array_3` FOREIGN KEY (`ArrayDesign`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Array_4` FOREIGN KEY (`Information`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Array_5` FOREIGN KEY (`ArrayGroup`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `arraydesign`
--

DROP TABLE IF EXISTS `arraydesign`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arraydesign` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Version` varchar(255) DEFAULT NULL,
  `NumberOfFeatures` smallint(6) DEFAULT NULL,
  `SurfaceType` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ArrayDesign1` (`DataSetWID`),
  KEY `FK_ArrayDesign3` (`SurfaceType`),
  CONSTRAINT `FK_ArrayDesign1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArrayDesign3` FOREIGN KEY (`SurfaceType`) REFERENCES `term` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `arraydesignwidcompositegrpwid`
--

DROP TABLE IF EXISTS `arraydesignwidcompositegrpwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
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
-- Table structure for table `arraydesignwidcontactwid`
--

DROP TABLE IF EXISTS `arraydesignwidcontactwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
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
-- Table structure for table `arraydesignwidreportergroupwid`
--

DROP TABLE IF EXISTS `arraydesignwidreportergroupwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arraydesignwidreportergroupwid` (
  `ArrayDesignWID` bigint(20) NOT NULL,
  `ReporterGroupWID` bigint(20) NOT NULL,
  KEY `FK_ArrayDesignWIDReporterGro1` (`ArrayDesignWID`),
  KEY `FK_ArrayDesignWIDReporterGro2` (`ReporterGroupWID`),
  CONSTRAINT `FK_ArrayDesignWIDReporterGro1` FOREIGN KEY (`ArrayDesignWID`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArrayDesignWIDReporterGro2` FOREIGN KEY (`ReporterGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `arraygroup`
--

DROP TABLE IF EXISTS `arraygroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arraygroup` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Barcode` varchar(255) DEFAULT NULL,
  `ArraySpacingX` float DEFAULT NULL,
  `ArraySpacingY` float DEFAULT NULL,
  `NumArrays` smallint(6) DEFAULT NULL,
  `OrientationMark` varchar(255) DEFAULT NULL,
  `OrientationMarkPosition` varchar(25) DEFAULT NULL,
  `Width` float DEFAULT NULL,
  `Length` float DEFAULT NULL,
  `ArrayGroup_SubstrateType` bigint(20) DEFAULT NULL,
  `ArrayGroup_DistanceUnit` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ArrayGroup1` (`DataSetWID`),
  KEY `FK_ArrayGroup3` (`ArrayGroup_SubstrateType`),
  KEY `FK_ArrayGroup4` (`ArrayGroup_DistanceUnit`),
  CONSTRAINT `FK_ArrayGroup1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArrayGroup3` FOREIGN KEY (`ArrayGroup_SubstrateType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArrayGroup4` FOREIGN KEY (`ArrayGroup_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `arraygroupwidarraywid`
--

DROP TABLE IF EXISTS `arraygroupwidarraywid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arraygroupwidarraywid` (
  `ArrayGroupWID` bigint(20) NOT NULL,
  `ArrayWID` bigint(20) NOT NULL,
  KEY `FK_ArrayGroupWIDArrayWID1` (`ArrayGroupWID`),
  KEY `FK_ArrayGroupWIDArrayWID2` (`ArrayWID`),
  CONSTRAINT `FK_ArrayGroupWIDArrayWID1` FOREIGN KEY (`ArrayGroupWID`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArrayGroupWIDArrayWID2` FOREIGN KEY (`ArrayWID`) REFERENCES `array_` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `arraymanufacture`
--

DROP TABLE IF EXISTS `arraymanufacture`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arraymanufacture` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `ManufacturingDate` varchar(255) DEFAULT NULL,
  `Tolerance` float DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ArrayManufacture1` (`DataSetWID`),
  CONSTRAINT `FK_ArrayManufacture1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `arraymanufacturedeviation`
--

DROP TABLE IF EXISTS `arraymanufacturedeviation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arraymanufacturedeviation` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Array_` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ArrayManufactureDeviation1` (`DataSetWID`),
  KEY `FK_ArrayManufactureDeviation2` (`Array_`),
  CONSTRAINT `FK_ArrayManufactureDeviation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArrayManufactureDeviation2` FOREIGN KEY (`Array_`) REFERENCES `array_` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `arraymanufacturewidarraywid`
--

DROP TABLE IF EXISTS `arraymanufacturewidarraywid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arraymanufacturewidarraywid` (
  `ArrayManufactureWID` bigint(20) NOT NULL,
  `ArrayWID` bigint(20) NOT NULL,
  KEY `FK_ArrayManufactureWIDArrayW1` (`ArrayManufactureWID`),
  KEY `FK_ArrayManufactureWIDArrayW2` (`ArrayWID`),
  CONSTRAINT `FK_ArrayManufactureWIDArrayW1` FOREIGN KEY (`ArrayManufactureWID`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArrayManufactureWIDArrayW2` FOREIGN KEY (`ArrayWID`) REFERENCES `array_` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `arraymanufacturewidcontactwid`
--

DROP TABLE IF EXISTS `arraymanufacturewidcontactwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arraymanufacturewidcontactwid` (
  `ArrayManufactureWID` bigint(20) NOT NULL,
  `ContactWID` bigint(20) NOT NULL,
  KEY `FK_ArrayManufactureWIDContac1` (`ArrayManufactureWID`),
  KEY `FK_ArrayManufactureWIDContac2` (`ContactWID`),
  CONSTRAINT `FK_ArrayManufactureWIDContac1` FOREIGN KEY (`ArrayManufactureWID`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArrayManufactureWIDContac2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bassaymappingwidbassaymapwid`
--

DROP TABLE IF EXISTS `bassaymappingwidbassaymapwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bassaymappingwidbassaymapwid` (
  `BioAssayMappingWID` bigint(20) NOT NULL,
  `BioAssayMapWID` bigint(20) NOT NULL,
  KEY `FK_BAssayMappingWIDBAssayMap1` (`BioAssayMappingWID`),
  KEY `FK_BAssayMappingWIDBAssayMap2` (`BioAssayMapWID`),
  CONSTRAINT `FK_BAssayMappingWIDBAssayMap1` FOREIGN KEY (`BioAssayMappingWID`) REFERENCES `bioassaymapping` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BAssayMappingWIDBAssayMap2` FOREIGN KEY (`BioAssayMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioassay`
--

DROP TABLE IF EXISTS `bioassay`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioassay` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `DerivedBioAssay_Type` bigint(20) DEFAULT NULL,
  `FeatureExtraction` bigint(20) DEFAULT NULL,
  `BioAssayCreation` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioAssay1` (`DataSetWID`),
  KEY `FK_BioAssay3` (`DerivedBioAssay_Type`),
  KEY `FK_BioAssay4` (`FeatureExtraction`),
  KEY `FK_BioAssay5` (`BioAssayCreation`),
  CONSTRAINT `FK_BioAssay1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssay3` FOREIGN KEY (`DerivedBioAssay_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssay4` FOREIGN KEY (`FeatureExtraction`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssay5` FOREIGN KEY (`BioAssayCreation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioassaydata`
--

DROP TABLE IF EXISTS `bioassaydata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioassaydata` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `BioAssayDimension` bigint(20) DEFAULT NULL,
  `DesignElementDimension` bigint(20) DEFAULT NULL,
  `QuantitationTypeDimension` bigint(20) DEFAULT NULL,
  `BioAssayData_BioDataValues` bigint(20) DEFAULT NULL,
  `ProducerTransformation` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioAssayData1` (`DataSetWID`),
  KEY `FK_BioAssayData3` (`BioAssayDimension`),
  KEY `FK_BioAssayData4` (`DesignElementDimension`),
  KEY `FK_BioAssayData5` (`QuantitationTypeDimension`),
  KEY `FK_BioAssayData6` (`BioAssayData_BioDataValues`),
  KEY `FK_BioAssayData7` (`ProducerTransformation`),
  CONSTRAINT `FK_BioAssayData1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayData3` FOREIGN KEY (`BioAssayDimension`) REFERENCES `bioassaydimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayData4` FOREIGN KEY (`DesignElementDimension`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayData5` FOREIGN KEY (`QuantitationTypeDimension`) REFERENCES `quantitationtypedimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayData6` FOREIGN KEY (`BioAssayData_BioDataValues`) REFERENCES `biodatavalues` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayData7` FOREIGN KEY (`ProducerTransformation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioassaydatacluster`
--

DROP TABLE IF EXISTS `bioassaydatacluster`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioassaydatacluster` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `ClusterBioAssayData` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioAssayDataCluster1` (`DataSetWID`),
  KEY `FK_BioAssayDataCluster3` (`ClusterBioAssayData`),
  CONSTRAINT `FK_BioAssayDataCluster1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayDataCluster3` FOREIGN KEY (`ClusterBioAssayData`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioassaydimension`
--

DROP TABLE IF EXISTS `bioassaydimension`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioassaydimension` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioAssayDimension1` (`DataSetWID`),
  CONSTRAINT `FK_BioAssayDimension1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioassaydimensiowidbioassaywid`
--

DROP TABLE IF EXISTS `bioassaydimensiowidbioassaywid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioassaydimensiowidbioassaywid` (
  `BioAssayDimensionWID` bigint(20) NOT NULL,
  `BioAssayWID` bigint(20) NOT NULL,
  KEY `FK_BioAssayDimensioWIDBioAss1` (`BioAssayDimensionWID`),
  KEY `FK_BioAssayDimensioWIDBioAss2` (`BioAssayWID`),
  CONSTRAINT `FK_BioAssayDimensioWIDBioAss1` FOREIGN KEY (`BioAssayDimensionWID`) REFERENCES `bioassaydimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayDimensioWIDBioAss2` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioassaymapping`
--

DROP TABLE IF EXISTS `bioassaymapping`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioassaymapping` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioAssayMapping1` (`DataSetWID`),
  CONSTRAINT `FK_BioAssayMapping1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioassaymapwidbioassaywid`
--

DROP TABLE IF EXISTS `bioassaymapwidbioassaywid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioassaymapwidbioassaywid` (
  `BioAssayMapWID` bigint(20) NOT NULL,
  `BioAssayWID` bigint(20) NOT NULL,
  KEY `FK_BioAssayMapWIDBioAssayWID1` (`BioAssayMapWID`),
  KEY `FK_BioAssayMapWIDBioAssayWID2` (`BioAssayWID`),
  CONSTRAINT `FK_BioAssayMapWIDBioAssayWID1` FOREIGN KEY (`BioAssayMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayMapWIDBioAssayWID2` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioassaytuple`
--

DROP TABLE IF EXISTS `bioassaytuple`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioassaytuple` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `BioAssay` bigint(20) DEFAULT NULL,
  `BioDataTuples_BioAssayTuples` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioAssayTuple1` (`DataSetWID`),
  KEY `FK_BioAssayTuple2` (`BioAssay`),
  KEY `FK_BioAssayTuple3` (`BioDataTuples_BioAssayTuples`),
  CONSTRAINT `FK_BioAssayTuple1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayTuple2` FOREIGN KEY (`BioAssay`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayTuple3` FOREIGN KEY (`BioDataTuples_BioAssayTuples`) REFERENCES `biodatavalues` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioassaywidchannelwid`
--

DROP TABLE IF EXISTS `bioassaywidchannelwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioassaywidchannelwid` (
  `BioAssayWID` bigint(20) NOT NULL,
  `ChannelWID` bigint(20) NOT NULL,
  KEY `FK_BioAssayWIDChannelWID1` (`BioAssayWID`),
  KEY `FK_BioAssayWIDChannelWID2` (`ChannelWID`),
  CONSTRAINT `FK_BioAssayWIDChannelWID1` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayWIDChannelWID2` FOREIGN KEY (`ChannelWID`) REFERENCES `channel` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioassaywidfactorvaluewid`
--

DROP TABLE IF EXISTS `bioassaywidfactorvaluewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioassaywidfactorvaluewid` (
  `BioAssayWID` bigint(20) NOT NULL,
  `FactorValueWID` bigint(20) NOT NULL,
  KEY `FK_BioAssayWIDFactorValueWID1` (`BioAssayWID`),
  KEY `FK_BioAssayWIDFactorValueWID2` (`FactorValueWID`),
  CONSTRAINT `FK_BioAssayWIDFactorValueWID1` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayWIDFactorValueWID2` FOREIGN KEY (`FactorValueWID`) REFERENCES `factorvalue` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `biodatavalues`
--

DROP TABLE IF EXISTS `biodatavalues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `biodatavalues` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Order_` varchar(25) DEFAULT NULL,
  `BioDataCube_DataInternal` bigint(20) DEFAULT NULL,
  `BioDataCube_DataExternal` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioDataValues1` (`DataSetWID`),
  KEY `FK_BioDataValues2` (`BioDataCube_DataInternal`),
  KEY `FK_BioDataValues3` (`BioDataCube_DataExternal`),
  CONSTRAINT `FK_BioDataValues1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioDataValues2` FOREIGN KEY (`BioDataCube_DataInternal`) REFERENCES `datainternal` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioDataValues3` FOREIGN KEY (`BioDataCube_DataExternal`) REFERENCES `dataexternal` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bioevent`
--

DROP TABLE IF EXISTS `bioevent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bioevent` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `CompositeSequence` bigint(20) DEFAULT NULL,
  `Reporter` bigint(20) DEFAULT NULL,
  `CompositeSequence2` bigint(20) DEFAULT NULL,
  `BioAssayMapTarget` bigint(20) DEFAULT NULL,
  `TargetQuantitationType` bigint(20) DEFAULT NULL,
  `DerivedBioAssayDataTarget` bigint(20) DEFAULT NULL,
  `QuantitationTypeMapping` bigint(20) DEFAULT NULL,
  `DesignElementMapping` bigint(20) DEFAULT NULL,
  `Transformation_BioAssayMapping` bigint(20) DEFAULT NULL,
  `BioMaterial_Treatments` bigint(20) DEFAULT NULL,
  `Order_` smallint(6) DEFAULT NULL,
  `Treatment_Action` bigint(20) DEFAULT NULL,
  `Treatment_ActionMeasurement` bigint(20) DEFAULT NULL,
  `Array_` bigint(20) DEFAULT NULL,
  `PhysicalBioAssayTarget` bigint(20) DEFAULT NULL,
  `PhysicalBioAssay` bigint(20) DEFAULT NULL,
  `Target` bigint(20) DEFAULT NULL,
  `PhysicalBioAssaySource` bigint(20) DEFAULT NULL,
  `MeasuredBioAssayTarget` bigint(20) DEFAULT NULL,
  `PhysicalBioAssay2` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioEvent1` (`DataSetWID`),
  KEY `FK_BioEvent3` (`CompositeSequence`),
  KEY `FK_BioEvent4` (`Reporter`),
  KEY `FK_BioEvent5` (`CompositeSequence2`),
  KEY `FK_BioEvent6` (`BioAssayMapTarget`),
  KEY `FK_BioEvent7` (`TargetQuantitationType`),
  KEY `FK_BioEvent8` (`DerivedBioAssayDataTarget`),
  KEY `FK_BioEvent9` (`QuantitationTypeMapping`),
  KEY `FK_BioEvent10` (`DesignElementMapping`),
  KEY `FK_BioEvent11` (`Transformation_BioAssayMapping`),
  KEY `FK_BioEvent12` (`BioMaterial_Treatments`),
  KEY `FK_BioEvent13` (`Treatment_Action`),
  KEY `FK_BioEvent14` (`Treatment_ActionMeasurement`),
  KEY `FK_BioEvent15` (`Array_`),
  KEY `FK_BioEvent16` (`PhysicalBioAssayTarget`),
  KEY `FK_BioEvent17` (`PhysicalBioAssay`),
  KEY `FK_BioEvent18` (`Target`),
  KEY `FK_BioEvent19` (`PhysicalBioAssaySource`),
  KEY `FK_BioEvent20` (`MeasuredBioAssayTarget`),
  KEY `FK_BioEvent21` (`PhysicalBioAssay2`),
  CONSTRAINT `FK_BioEvent1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent10` FOREIGN KEY (`DesignElementMapping`) REFERENCES `designelementmapping` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent11` FOREIGN KEY (`Transformation_BioAssayMapping`) REFERENCES `bioassaymapping` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent12` FOREIGN KEY (`BioMaterial_Treatments`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent13` FOREIGN KEY (`Treatment_Action`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent14` FOREIGN KEY (`Treatment_ActionMeasurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent15` FOREIGN KEY (`Array_`) REFERENCES `array_` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent16` FOREIGN KEY (`PhysicalBioAssayTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent17` FOREIGN KEY (`PhysicalBioAssay`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent18` FOREIGN KEY (`Target`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent19` FOREIGN KEY (`PhysicalBioAssaySource`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent20` FOREIGN KEY (`MeasuredBioAssayTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent21` FOREIGN KEY (`PhysicalBioAssay2`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent3` FOREIGN KEY (`CompositeSequence`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent4` FOREIGN KEY (`Reporter`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent5` FOREIGN KEY (`CompositeSequence2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent6` FOREIGN KEY (`BioAssayMapTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent7` FOREIGN KEY (`TargetQuantitationType`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent8` FOREIGN KEY (`DerivedBioAssayDataTarget`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent9` FOREIGN KEY (`QuantitationTypeMapping`) REFERENCES `quantitationtypemapping` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `biomaterialmeasurement`
--

DROP TABLE IF EXISTS `biomaterialmeasurement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `biomaterialmeasurement` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `BioMaterial` bigint(20) DEFAULT NULL,
  `Measurement` bigint(20) DEFAULT NULL,
  `Treatment` bigint(20) DEFAULT NULL,
  `BioAssayCreation` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioMaterialMeasurement1` (`DataSetWID`),
  KEY `FK_BioMaterialMeasurement2` (`BioMaterial`),
  KEY `FK_BioMaterialMeasurement3` (`Measurement`),
  KEY `FK_BioMaterialMeasurement4` (`Treatment`),
  KEY `FK_BioMaterialMeasurement5` (`BioAssayCreation`),
  CONSTRAINT `FK_BioMaterialMeasurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioMaterialMeasurement2` FOREIGN KEY (`BioMaterial`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioMaterialMeasurement3` FOREIGN KEY (`Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioMaterialMeasurement4` FOREIGN KEY (`Treatment`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioMaterialMeasurement5` FOREIGN KEY (`BioAssayCreation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `biosource`
--

DROP TABLE IF EXISTS `biosource`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `biosource` (
  `WID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) DEFAULT NULL,
  `TaxonWID` bigint(20) DEFAULT NULL,
  `Name` varchar(200) DEFAULT NULL,
  `Strain` varchar(220) DEFAULT NULL,
  `Organ` varchar(50) DEFAULT NULL,
  `Organelle` varchar(50) DEFAULT NULL,
  `Tissue` varchar(100) DEFAULT NULL,
  `CellType` varchar(50) DEFAULT NULL,
  `CellLine` varchar(50) DEFAULT NULL,
  `ATCCId` varchar(50) DEFAULT NULL,
  `Diseased` char(1) DEFAULT NULL,
  `Disease` varchar(250) DEFAULT NULL,
  `DevelopmentStage` varchar(50) DEFAULT NULL,
  `Sex` varchar(15) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioSource1` (`TaxonWID`),
  KEY `FK_BioSource2` (`DataSetWID`),
  CONSTRAINT `FK_BioSource1` FOREIGN KEY (`TaxonWID`) REFERENCES `taxon` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioSource2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `biosourcewidbiosubtypewid`
--

DROP TABLE IF EXISTS `biosourcewidbiosubtypewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `biosourcewidbiosubtypewid` (
  `BioSourceWID` bigint(20) NOT NULL,
  `BioSubtypeWID` bigint(20) NOT NULL,
  KEY `FK_BioSourceWIDBioSubtypeWID1` (`BioSourceWID`),
  KEY `FK_BioSourceWIDBioSubtypeWID2` (`BioSubtypeWID`),
  CONSTRAINT `FK_BioSourceWIDBioSubtypeWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioSourceWIDBioSubtypeWID2` FOREIGN KEY (`BioSubtypeWID`) REFERENCES `biosubtype` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `biosourcewidcontactwid`
--

DROP TABLE IF EXISTS `biosourcewidcontactwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `biosourcewidcontactwid` (
  `BioSourceWID` bigint(20) NOT NULL,
  `ContactWID` bigint(20) NOT NULL,
  KEY `FK_BioSourceWIDContactWID1` (`BioSourceWID`),
  KEY `FK_BioSourceWIDContactWID2` (`ContactWID`),
  CONSTRAINT `FK_BioSourceWIDContactWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioSourceWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `biosourcewidgenewid`
--

DROP TABLE IF EXISTS `biosourcewidgenewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `biosourcewidgenewid` (
  `BioSourceWID` bigint(20) NOT NULL,
  `GeneWID` bigint(20) NOT NULL,
  KEY `FK_BioSourceWIDGeneWID1` (`BioSourceWID`),
  KEY `FK_BioSourceWIDGeneWID2` (`GeneWID`),
  CONSTRAINT `FK_BioSourceWIDGeneWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioSourceWIDGeneWID2` FOREIGN KEY (`GeneWID`) REFERENCES `gene` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `biosourcewidproteinwid`
--

DROP TABLE IF EXISTS `biosourcewidproteinwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `biosourcewidproteinwid` (
  `BioSourceWID` bigint(20) NOT NULL,
  `ProteinWID` bigint(20) NOT NULL,
  KEY `FK_BioSourceWIDProteinWID1` (`BioSourceWID`),
  KEY `FK_BioSourceWIDProteinWID2` (`ProteinWID`),
  CONSTRAINT `FK_BioSourceWIDProteinWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioSourceWIDProteinWID2` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `biosubtype`
--

DROP TABLE IF EXISTS `biosubtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `biosubtype` (
  `WID` bigint(20) NOT NULL,
  `Type` varchar(25) DEFAULT NULL,
  `Value` varchar(50) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioSubtype2` (`DataSetWID`),
  CONSTRAINT `FK_BioSubtype2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `channel`
--

DROP TABLE IF EXISTS `channel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `channel` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Channel1` (`DataSetWID`),
  CONSTRAINT `FK_Channel1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `channelwidcompoundwid`
--

DROP TABLE IF EXISTS `channelwidcompoundwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `channelwidcompoundwid` (
  `ChannelWID` bigint(20) NOT NULL,
  `CompoundWID` bigint(20) NOT NULL,
  KEY `FK_ChannelWIDCompoundWID1` (`ChannelWID`),
  KEY `FK_ChannelWIDCompoundWID2` (`CompoundWID`),
  CONSTRAINT `FK_ChannelWIDCompoundWID1` FOREIGN KEY (`ChannelWID`) REFERENCES `channel` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChannelWIDCompoundWID2` FOREIGN KEY (`CompoundWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `chemical`
--

DROP TABLE IF EXISTS `chemical`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chemical` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Class` char(1) DEFAULT NULL,
  `BeilsteinName` varchar(50) DEFAULT NULL,
  `SystematicName` varchar(255) DEFAULT NULL,
  `CAS` varchar(50) DEFAULT NULL,
  `Charge` smallint(6) DEFAULT NULL,
  `EmpiricalFormula` varchar(50) DEFAULT NULL,
  `MolecularWeightCalc` float DEFAULT NULL,
  `MolecularWeightExp` float DEFAULT NULL,
  `OctH2OPartitionCoeff` varchar(50) DEFAULT NULL,
  `PKA1` float DEFAULT NULL,
  `PKA2` float DEFAULT NULL,
  `PKA3` float DEFAULT NULL,
  `WaterSolubility` char(1) DEFAULT NULL,
  `Smiles` varchar(255) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `CHEMICAL_DWID_NAME` (`DataSetWID`,`Name`),
  KEY `CHEMICAL_NAME` (`Name`),
  CONSTRAINT `FK_Chemical1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `chemicalatom`
--

DROP TABLE IF EXISTS `chemicalatom`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chemicalatom` (
  `ChemicalWID` bigint(20) NOT NULL,
  `AtomIndex` smallint(6) NOT NULL,
  `Atom` varchar(2) NOT NULL,
  `Charge` smallint(6) NOT NULL,
  `X` decimal(10,0) DEFAULT NULL,
  `Y` decimal(10,0) DEFAULT NULL,
  `Z` decimal(10,0) DEFAULT NULL,
  `StereoParity` decimal(10,0) DEFAULT NULL,
  UNIQUE KEY `UN_ChemicalAtom` (`ChemicalWID`,`AtomIndex`),
  CONSTRAINT `FK_ChemicalAtom` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `chemicalbond`
--

DROP TABLE IF EXISTS `chemicalbond`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chemicalbond` (
  `ChemicalWID` bigint(20) NOT NULL,
  `Atom1Index` smallint(6) NOT NULL,
  `Atom2Index` smallint(6) NOT NULL,
  `BondType` smallint(6) NOT NULL,
  `BondStereo` decimal(10,0) DEFAULT NULL,
  KEY `FK_ChemicalBond` (`ChemicalWID`),
  CONSTRAINT `FK_ChemicalBond` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `citation`
--

DROP TABLE IF EXISTS `citation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `citation` (
  `WID` bigint(20) NOT NULL,
  `Citation` text,
  `PMID` decimal(10,0) DEFAULT NULL,
  `Title` varchar(255) DEFAULT NULL,
  `Authors` varchar(255) DEFAULT NULL,
  `Publication` varchar(255) DEFAULT NULL,
  `Publisher` varchar(255) DEFAULT NULL,
  `Editor` varchar(255) DEFAULT NULL,
  `Year` varchar(255) DEFAULT NULL,
  `Volume` varchar(255) DEFAULT NULL,
  `Issue` varchar(255) DEFAULT NULL,
  `Pages` varchar(255) DEFAULT NULL,
  `URI` varchar(255) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `CITATION_PMID` (`PMID`),
  KEY `CITATION_CITATION` (`Citation`(20)),
  KEY `FK_Citation` (`DataSetWID`),
  CONSTRAINT `FK_Citation` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `citationwidotherwid`
--

DROP TABLE IF EXISTS `citationwidotherwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `citationwidotherwid` (
  `OtherWID` bigint(20) NOT NULL,
  `CitationWID` bigint(20) NOT NULL,
  KEY `FK_CitationWIDOtherWID` (`CitationWID`),
  CONSTRAINT `FK_CitationWIDOtherWID` FOREIGN KEY (`CitationWID`) REFERENCES `citation` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `commenttable`
--

DROP TABLE IF EXISTS `commenttable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `commenttable` (
  `OtherWID` bigint(20) NOT NULL,
  `Comm` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `composgrpwidcompossequencewid`
--

DROP TABLE IF EXISTS `composgrpwidcompossequencewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `composgrpwidcompossequencewid` (
  `CompositeGroupWID` bigint(20) NOT NULL,
  `CompositeSequenceWID` bigint(20) NOT NULL,
  KEY `FK_ComposGrpWIDComposSequenc1` (`CompositeGroupWID`),
  KEY `FK_ComposGrpWIDComposSequenc2` (`CompositeSequenceWID`),
  CONSTRAINT `FK_ComposGrpWIDComposSequenc1` FOREIGN KEY (`CompositeGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ComposGrpWIDComposSequenc2` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compositeseqwidbioseqwid`
--

DROP TABLE IF EXISTS `compositeseqwidbioseqwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compositeseqwidbioseqwid` (
  `CompositeSequenceWID` bigint(20) NOT NULL,
  `BioSequenceWID` bigint(20) NOT NULL,
  KEY `FK_CompositeSeqWIDBioSeqWID1` (`CompositeSequenceWID`),
  CONSTRAINT `FK_CompositeSeqWIDBioSeqWID1` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `composseqdimenswidcomposseqwid`
--

DROP TABLE IF EXISTS `composseqdimenswidcomposseqwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `composseqdimenswidcomposseqwid` (
  `CompositeSequenceDimensionWID` bigint(20) NOT NULL,
  `CompositeSequenceWID` bigint(20) NOT NULL,
  KEY `FK_ComposSeqDimensWIDComposS1` (`CompositeSequenceDimensionWID`),
  KEY `FK_ComposSeqDimensWIDComposS2` (`CompositeSequenceWID`),
  CONSTRAINT `FK_ComposSeqDimensWIDComposS1` FOREIGN KEY (`CompositeSequenceDimensionWID`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ComposSeqDimensWIDComposS2` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `composseqwidcomposcomposmapwid`
--

DROP TABLE IF EXISTS `composseqwidcomposcomposmapwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `composseqwidcomposcomposmapwid` (
  `CompositeSequenceWID` bigint(20) NOT NULL,
  `CompositeCompositeMapWID` bigint(20) NOT NULL,
  KEY `FK_ComposSeqWIDComposComposM1` (`CompositeSequenceWID`),
  KEY `FK_ComposSeqWIDComposComposM2` (`CompositeCompositeMapWID`),
  CONSTRAINT `FK_ComposSeqWIDComposComposM1` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ComposSeqWIDComposComposM2` FOREIGN KEY (`CompositeCompositeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `composseqwidrepocomposmapwid`
--

DROP TABLE IF EXISTS `composseqwidrepocomposmapwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `composseqwidrepocomposmapwid` (
  `CompositeSequenceWID` bigint(20) NOT NULL,
  `ReporterCompositeMapWID` bigint(20) NOT NULL,
  KEY `FK_ComposSeqWIDRepoComposMap1` (`CompositeSequenceWID`),
  KEY `FK_ComposSeqWIDRepoComposMap2` (`ReporterCompositeMapWID`),
  CONSTRAINT `FK_ComposSeqWIDRepoComposMap1` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ComposSeqWIDRepoComposMap2` FOREIGN KEY (`ReporterCompositeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compoundmeasurement`
--

DROP TABLE IF EXISTS `compoundmeasurement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compoundmeasurement` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Compound_ComponentCompounds` bigint(20) DEFAULT NULL,
  `Compound` bigint(20) DEFAULT NULL,
  `Measurement` bigint(20) DEFAULT NULL,
  `Treatment_CompoundMeasurements` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_CompoundMeasurement1` (`DataSetWID`),
  KEY `FK_CompoundMeasurement2` (`Compound_ComponentCompounds`),
  KEY `FK_CompoundMeasurement3` (`Compound`),
  KEY `FK_CompoundMeasurement4` (`Measurement`),
  KEY `FK_CompoundMeasurement5` (`Treatment_CompoundMeasurements`),
  CONSTRAINT `FK_CompoundMeasurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_CompoundMeasurement2` FOREIGN KEY (`Compound_ComponentCompounds`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_CompoundMeasurement3` FOREIGN KEY (`Compound`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_CompoundMeasurement4` FOREIGN KEY (`Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_CompoundMeasurement5` FOREIGN KEY (`Treatment_CompoundMeasurements`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `computation`
--

DROP TABLE IF EXISTS `computation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `computation` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Description` text,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Computation` (`DataSetWID`),
  CONSTRAINT `FK_Computation` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `contact`
--

DROP TABLE IF EXISTS `contact`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `contact` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `URI` varchar(255) DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `Phone` varchar(255) DEFAULT NULL,
  `TollFreePhone` varchar(255) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `Fax` varchar(255) DEFAULT NULL,
  `Parent` bigint(20) DEFAULT NULL,
  `LastName` varchar(255) DEFAULT NULL,
  `FirstName` varchar(255) DEFAULT NULL,
  `MidInitials` varchar(255) DEFAULT NULL,
  `Affiliation` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Contact1` (`DataSetWID`),
  KEY `FK_Contact3` (`Parent`),
  KEY `FK_Contact4` (`Affiliation`),
  CONSTRAINT `FK_Contact1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Contact3` FOREIGN KEY (`Parent`) REFERENCES `contact` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Contact4` FOREIGN KEY (`Affiliation`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `crossreference`
--

DROP TABLE IF EXISTS `crossreference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `crossreference` (
  `OtherWID` bigint(20) NOT NULL,
  `CrossWID` bigint(20) DEFAULT NULL,
  `XID` varchar(50) DEFAULT NULL,
  `Type` varchar(20) DEFAULT NULL,
  `Version` varchar(10) DEFAULT NULL,
  `Relationship` varchar(50) DEFAULT NULL,
  `DatabaseName` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `database_`
--

DROP TABLE IF EXISTS `database_`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `database_` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Version` varchar(255) DEFAULT NULL,
  `URI` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Database_1` (`DataSetWID`),
  CONSTRAINT `FK_Database_1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `databasewidcontactwid`
--

DROP TABLE IF EXISTS `databasewidcontactwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `databasewidcontactwid` (
  `DatabaseWID` bigint(20) NOT NULL,
  `ContactWID` bigint(20) NOT NULL,
  KEY `FK_DatabaseWIDContactWID1` (`DatabaseWID`),
  KEY `FK_DatabaseWIDContactWID2` (`ContactWID`),
  CONSTRAINT `FK_DatabaseWIDContactWID1` FOREIGN KEY (`DatabaseWID`) REFERENCES `database_` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DatabaseWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dataexternal`
--

DROP TABLE IF EXISTS `dataexternal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dataexternal` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `DataFormat` varchar(255) DEFAULT NULL,
  `DataFormatInfoURI` varchar(255) DEFAULT NULL,
  `FilenameURI` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_DataExternal1` (`DataSetWID`),
  CONSTRAINT `FK_DataExternal1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `datainternal`
--

DROP TABLE IF EXISTS `datainternal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `datainternal` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_DataInternal1` (`DataSetWID`),
  CONSTRAINT `FK_DataInternal1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dataset`
--

DROP TABLE IF EXISTS `dataset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dataset` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Version` varchar(50) DEFAULT NULL,
  `ReleaseDate` datetime DEFAULT NULL,
  `LoadDate` datetime NOT NULL,
  `ChangeDate` datetime DEFAULT NULL,
  `HomeURL` varchar(255) DEFAULT NULL,
  `QueryURL` varchar(255) DEFAULT NULL,
  `LoadedBy` varchar(255) DEFAULT NULL,
  `Application` varchar(255) DEFAULT NULL,
  `ApplicationVersion` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`WID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `datasethierarchy`
--

DROP TABLE IF EXISTS `datasethierarchy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `datasethierarchy` (
  `SuperWID` bigint(20) NOT NULL,
  `SubWID` bigint(20) NOT NULL,
  KEY `FK_DataSetHierarchy1` (`SuperWID`),
  KEY `FK_DataSetHierarchy2` (`SubWID`),
  CONSTRAINT `FK_DataSetHierarchy1` FOREIGN KEY (`SuperWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DataSetHierarchy2` FOREIGN KEY (`SubWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `datum`
--

DROP TABLE IF EXISTS `datum`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `datum` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Value` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Datum1` (`DataSetWID`),
  CONSTRAINT `FK_Datum1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dbid`
--

DROP TABLE IF EXISTS `dbid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dbid` (
  `OtherWID` bigint(20) NOT NULL,
  `XID` varchar(150) NOT NULL,
  `Type` varchar(20) DEFAULT NULL,
  `Version` varchar(10) DEFAULT NULL,
  KEY `DBID_XID_OTHERWID` (`XID`,`OtherWID`),
  KEY `DBID_OTHERWID` (`OtherWID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `derivbioassaywidbioassaymapwid`
--

DROP TABLE IF EXISTS `derivbioassaywidbioassaymapwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `derivbioassaywidbioassaymapwid` (
  `DerivedBioAssayWID` bigint(20) NOT NULL,
  `BioAssayMapWID` bigint(20) NOT NULL,
  KEY `FK_DerivBioAssayWIDBioAssayM1` (`DerivedBioAssayWID`),
  KEY `FK_DerivBioAssayWIDBioAssayM2` (`BioAssayMapWID`),
  CONSTRAINT `FK_DerivBioAssayWIDBioAssayM1` FOREIGN KEY (`DerivedBioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DerivBioAssayWIDBioAssayM2` FOREIGN KEY (`BioAssayMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `derivbioawidderivbioadatawid`
--

DROP TABLE IF EXISTS `derivbioawidderivbioadatawid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `derivbioawidderivbioadatawid` (
  `DerivedBioAssayWID` bigint(20) NOT NULL,
  `DerivedBioAssayDataWID` bigint(20) NOT NULL,
  KEY `FK_DerivBioAWIDDerivBioAData1` (`DerivedBioAssayWID`),
  KEY `FK_DerivBioAWIDDerivBioAData2` (`DerivedBioAssayDataWID`),
  CONSTRAINT `FK_DerivBioAWIDDerivBioAData1` FOREIGN KEY (`DerivedBioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DerivBioAWIDDerivBioAData2` FOREIGN KEY (`DerivedBioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `description`
--

DROP TABLE IF EXISTS `description`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `description` (
  `OtherWID` bigint(20) NOT NULL,
  `TableName` varchar(30) NOT NULL,
  `Comm` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `designelement`
--

DROP TABLE IF EXISTS `designelement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `designelement` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `FeatureGroup_Features` bigint(20) DEFAULT NULL,
  `DesignElement_ControlType` bigint(20) DEFAULT NULL,
  `Feature_Position` bigint(20) DEFAULT NULL,
  `Zone` bigint(20) DEFAULT NULL,
  `Feature_FeatureLocation` bigint(20) DEFAULT NULL,
  `FeatureGroup` bigint(20) DEFAULT NULL,
  `Reporter_WarningType` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_DesignElement1` (`DataSetWID`),
  KEY `FK_DesignElement3` (`FeatureGroup_Features`),
  KEY `FK_DesignElement4` (`DesignElement_ControlType`),
  KEY `FK_DesignElement5` (`Feature_Position`),
  KEY `FK_DesignElement6` (`Zone`),
  KEY `FK_DesignElement7` (`Feature_FeatureLocation`),
  KEY `FK_DesignElement8` (`FeatureGroup`),
  KEY `FK_DesignElement9` (`Reporter_WarningType`),
  CONSTRAINT `FK_DesignElement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElement3` FOREIGN KEY (`FeatureGroup_Features`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElement4` FOREIGN KEY (`DesignElement_ControlType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElement5` FOREIGN KEY (`Feature_Position`) REFERENCES `position_` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElement6` FOREIGN KEY (`Zone`) REFERENCES `zone` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElement7` FOREIGN KEY (`Feature_FeatureLocation`) REFERENCES `featurelocation` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElement8` FOREIGN KEY (`FeatureGroup`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElement9` FOREIGN KEY (`Reporter_WarningType`) REFERENCES `term` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `designelementdimension`
--

DROP TABLE IF EXISTS `designelementdimension`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `designelementdimension` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_DesignElementDimension1` (`DataSetWID`),
  CONSTRAINT `FK_DesignElementDimension1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `designelementgroup`
--

DROP TABLE IF EXISTS `designelementgroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `designelementgroup` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `ArrayDesign_FeatureGroups` bigint(20) DEFAULT NULL,
  `DesignElementGroup_Species` bigint(20) DEFAULT NULL,
  `FeatureWidth` float DEFAULT NULL,
  `FeatureLength` float DEFAULT NULL,
  `FeatureHeight` float DEFAULT NULL,
  `FeatureGroup_TechnologyType` bigint(20) DEFAULT NULL,
  `FeatureGroup_FeatureShape` bigint(20) DEFAULT NULL,
  `FeatureGroup_DistanceUnit` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_DesignElementGroup1` (`DataSetWID`),
  KEY `FK_DesignElementGroup3` (`ArrayDesign_FeatureGroups`),
  KEY `FK_DesignElementGroup4` (`DesignElementGroup_Species`),
  KEY `FK_DesignElementGroup5` (`FeatureGroup_TechnologyType`),
  KEY `FK_DesignElementGroup6` (`FeatureGroup_FeatureShape`),
  KEY `FK_DesignElementGroup7` (`FeatureGroup_DistanceUnit`),
  CONSTRAINT `FK_DesignElementGroup1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElementGroup3` FOREIGN KEY (`ArrayDesign_FeatureGroups`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElementGroup4` FOREIGN KEY (`DesignElementGroup_Species`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElementGroup5` FOREIGN KEY (`FeatureGroup_TechnologyType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElementGroup6` FOREIGN KEY (`FeatureGroup_FeatureShape`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElementGroup7` FOREIGN KEY (`FeatureGroup_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `designelementmapping`
--

DROP TABLE IF EXISTS `designelementmapping`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `designelementmapping` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_DesignElementMapping1` (`DataSetWID`),
  CONSTRAINT `FK_DesignElementMapping1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `designelementtuple`
--

DROP TABLE IF EXISTS `designelementtuple`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `designelementtuple` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `BioAssayTuple` bigint(20) DEFAULT NULL,
  `DesignElement` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_DesignElementTuple1` (`DataSetWID`),
  KEY `FK_DesignElementTuple2` (`BioAssayTuple`),
  KEY `FK_DesignElementTuple3` (`DesignElement`),
  CONSTRAINT `FK_DesignElementTuple1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElementTuple2` FOREIGN KEY (`BioAssayTuple`) REFERENCES `bioassaytuple` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElementTuple3` FOREIGN KEY (`DesignElement`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `desnelmappingwiddesnelmapwid`
--

DROP TABLE IF EXISTS `desnelmappingwiddesnelmapwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `desnelmappingwiddesnelmapwid` (
  `DesignElementMappingWID` bigint(20) NOT NULL,
  `DesignElementMapWID` bigint(20) NOT NULL,
  KEY `FK_DesnElMappingWIDDesnElMap1` (`DesignElementMappingWID`),
  KEY `FK_DesnElMappingWIDDesnElMap2` (`DesignElementMapWID`),
  CONSTRAINT `FK_DesnElMappingWIDDesnElMap1` FOREIGN KEY (`DesignElementMappingWID`) REFERENCES `designelementmapping` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesnElMappingWIDDesnElMap2` FOREIGN KEY (`DesignElementMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `division`
--

DROP TABLE IF EXISTS `division`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `division` (
  `WID` bigint(20) NOT NULL,
  `Code` varchar(10) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Division` (`DataSetWID`),
  CONSTRAINT `FK_Division` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `element`
--

DROP TABLE IF EXISTS `element`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `element` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(15) NOT NULL,
  `ElementSymbol` varchar(2) NOT NULL,
  `AtomicWeight` float NOT NULL,
  `AtomicNumber` smallint(6) NOT NULL,
  PRIMARY KEY (`WID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry`
--

DROP TABLE IF EXISTS `entry`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry` (
  `OtherWID` bigint(20) NOT NULL,
  `InsertDate` datetime NOT NULL,
  `CreationDate` datetime DEFAULT NULL,
  `ModifiedDate` datetime DEFAULT NULL,
  `LoadError` char(1) NOT NULL,
  `LineNumber` int(11) DEFAULT NULL,
  `ErrorMessage` text,
  `DatasetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`OtherWID`),
  KEY `FK_Entry` (`DatasetWID`),
  CONSTRAINT `FK_Entry` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enumeration`
--

DROP TABLE IF EXISTS `enumeration`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enumeration` (
  `TableName` varchar(50) NOT NULL,
  `ColumnName` varchar(50) NOT NULL,
  `Value` varchar(50) NOT NULL,
  `Meaning` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enzreactionaltcompound`
--

DROP TABLE IF EXISTS `enzreactionaltcompound`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enzreactionaltcompound` (
  `EnzymaticReactionWID` bigint(20) NOT NULL,
  `PrimaryWID` bigint(20) NOT NULL,
  `AlternativeWID` bigint(20) NOT NULL,
  `Cofactor` char(1) DEFAULT NULL,
  KEY `FK_ERAC1` (`EnzymaticReactionWID`),
  KEY `FK_ERAC2` (`PrimaryWID`),
  KEY `FK_ERAC3` (`AlternativeWID`),
  CONSTRAINT `FK_ERAC1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ERAC2` FOREIGN KEY (`PrimaryWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ERAC3` FOREIGN KEY (`AlternativeWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enzreactioncofactor`
--

DROP TABLE IF EXISTS `enzreactioncofactor`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enzreactioncofactor` (
  `EnzymaticReactionWID` bigint(20) NOT NULL,
  `ChemicalWID` bigint(20) NOT NULL,
  `Prosthetic` char(1) DEFAULT NULL,
  KEY `FK_EnzReactionCofactor1` (`EnzymaticReactionWID`),
  KEY `FK_EnzReactionCofactor2` (`ChemicalWID`),
  CONSTRAINT `FK_EnzReactionCofactor1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnzReactionCofactor2` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enzreactioninhibitoractivator`
--

DROP TABLE IF EXISTS `enzreactioninhibitoractivator`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enzreactioninhibitoractivator` (
  `EnzymaticReactionWID` bigint(20) NOT NULL,
  `CompoundWID` bigint(20) NOT NULL,
  `InhibitOrActivate` char(1) DEFAULT NULL,
  `Mechanism` char(1) DEFAULT NULL,
  `PhysioRelevant` char(1) DEFAULT NULL,
  KEY `FK_EnzReactionIA1` (`EnzymaticReactionWID`),
  CONSTRAINT `FK_EnzReactionIA1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enzymaticreaction`
--

DROP TABLE IF EXISTS `enzymaticreaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enzymaticreaction` (
  `WID` bigint(20) NOT NULL,
  `ReactionWID` bigint(20) NOT NULL,
  `ProteinWID` bigint(20) NOT NULL,
  `ComplexWID` bigint(20) DEFAULT NULL,
  `ReactionDirection` varchar(30) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `ER_DATASETWID` (`DataSetWID`),
  KEY `FK_EnzymaticReaction1` (`ReactionWID`),
  KEY `FK_EnzymaticReaction2` (`ProteinWID`),
  KEY `FK_EnzymaticReaction3` (`ComplexWID`),
  CONSTRAINT `FK_EnzymaticReaction1` FOREIGN KEY (`ReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnzymaticReaction2` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnzymaticReaction3` FOREIGN KEY (`ComplexWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnzymaticReaction4` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experiment`
--

DROP TABLE IF EXISTS `experiment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experiment` (
  `WID` bigint(20) NOT NULL,
  `Type` varchar(50) NOT NULL,
  `ContactWID` bigint(20) DEFAULT NULL,
  `ArchiveWID` bigint(20) DEFAULT NULL,
  `StartDate` datetime DEFAULT NULL,
  `EndDate` datetime DEFAULT NULL,
  `Description` text,
  `GroupWID` bigint(20) DEFAULT NULL,
  `GroupType` varchar(50) DEFAULT NULL,
  `GroupSize` int(11) NOT NULL,
  `GroupIndex` int(11) DEFAULT NULL,
  `TimePoint` int(11) DEFAULT NULL,
  `TimeUnit` varchar(20) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `BioSourceWID` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Experiment3` (`ContactWID`),
  KEY `FK_Experiment4` (`ArchiveWID`),
  KEY `FK_Experiment2` (`GroupWID`),
  KEY `FK_Experiment5` (`DataSetWID`),
  KEY `FK_Experiment6` (`BioSourceWID`),
  CONSTRAINT `FK_Experiment2` FOREIGN KEY (`GroupWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Experiment3` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Experiment4` FOREIGN KEY (`ArchiveWID`) REFERENCES `archive` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Experiment5` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Experiment6` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experimentalfactor`
--

DROP TABLE IF EXISTS `experimentalfactor`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experimentalfactor` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `ExperimentDesign` bigint(20) DEFAULT NULL,
  `ExperimentalFactor_Category` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ExperimentalFactor1` (`DataSetWID`),
  KEY `FK_ExperimentalFactor3` (`ExperimentDesign`),
  KEY `FK_ExperimentalFactor4` (`ExperimentalFactor_Category`),
  CONSTRAINT `FK_ExperimentalFactor1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExperimentalFactor3` FOREIGN KEY (`ExperimentDesign`) REFERENCES `experimentdesign` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExperimentalFactor4` FOREIGN KEY (`ExperimentalFactor_Category`) REFERENCES `term` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experimentdata`
--

DROP TABLE IF EXISTS `experimentdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experimentdata` (
  `WID` bigint(20) NOT NULL,
  `ExperimentWID` bigint(20) NOT NULL,
  `Data` longtext,
  `MageData` bigint(20) DEFAULT NULL,
  `Role` varchar(50) NOT NULL,
  `Kind` char(1) NOT NULL,
  `DateProduced` datetime DEFAULT NULL,
  `OtherWID` bigint(20) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ExpData1` (`ExperimentWID`),
  KEY `FK_ExpDataMD` (`MageData`),
  KEY `FK_ExpData2` (`DataSetWID`),
  CONSTRAINT `FK_ExpData1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExpData2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExpDataMD` FOREIGN KEY (`MageData`) REFERENCES `parametervalue` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experimentdesign`
--

DROP TABLE IF EXISTS `experimentdesign`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experimentdesign` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Experiment_ExperimentDesigns` bigint(20) DEFAULT NULL,
  `QualityControlDescription` bigint(20) DEFAULT NULL,
  `NormalizationDescription` bigint(20) DEFAULT NULL,
  `ReplicateDescription` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ExperimentDesign1` (`DataSetWID`),
  KEY `FK_ExperimentDesign3` (`Experiment_ExperimentDesigns`),
  CONSTRAINT `FK_ExperimentDesign1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExperimentDesign3` FOREIGN KEY (`Experiment_ExperimentDesigns`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experimentdesignwidbioassaywid`
--

DROP TABLE IF EXISTS `experimentdesignwidbioassaywid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experimentdesignwidbioassaywid` (
  `ExperimentDesignWID` bigint(20) NOT NULL,
  `BioAssayWID` bigint(20) NOT NULL,
  KEY `FK_ExperimentDesignWIDBioAss1` (`ExperimentDesignWID`),
  KEY `FK_ExperimentDesignWIDBioAss2` (`BioAssayWID`),
  CONSTRAINT `FK_ExperimentDesignWIDBioAss1` FOREIGN KEY (`ExperimentDesignWID`) REFERENCES `experimentdesign` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExperimentDesignWIDBioAss2` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experimentrelationship`
--

DROP TABLE IF EXISTS `experimentrelationship`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experimentrelationship` (
  `ExperimentWID` bigint(20) NOT NULL,
  `RelatedExperimentWID` bigint(20) NOT NULL,
  KEY `FK_ExpRelationship1` (`ExperimentWID`),
  KEY `FK_ExpRelationship2` (`RelatedExperimentWID`),
  CONSTRAINT `FK_ExpRelationship1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExpRelationship2` FOREIGN KEY (`RelatedExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experimentwidbioassaydatawid`
--

DROP TABLE IF EXISTS `experimentwidbioassaydatawid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experimentwidbioassaydatawid` (
  `ExperimentWID` bigint(20) NOT NULL,
  `BioAssayDataWID` bigint(20) NOT NULL,
  KEY `FK_ExperimentWIDBioAssayData1` (`ExperimentWID`),
  KEY `FK_ExperimentWIDBioAssayData2` (`BioAssayDataWID`),
  CONSTRAINT `FK_ExperimentWIDBioAssayData1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExperimentWIDBioAssayData2` FOREIGN KEY (`BioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experimentwidbioassaywid`
--

DROP TABLE IF EXISTS `experimentwidbioassaywid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experimentwidbioassaywid` (
  `ExperimentWID` bigint(20) NOT NULL,
  `BioAssayWID` bigint(20) NOT NULL,
  KEY `FK_ExperimentWIDBioAssayWID1` (`ExperimentWID`),
  KEY `FK_ExperimentWIDBioAssayWID2` (`BioAssayWID`),
  CONSTRAINT `FK_ExperimentWIDBioAssayWID1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExperimentWIDBioAssayWID2` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experimentwidcontactwid`
--

DROP TABLE IF EXISTS `experimentwidcontactwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experimentwidcontactwid` (
  `ExperimentWID` bigint(20) NOT NULL,
  `ContactWID` bigint(20) NOT NULL,
  KEY `FK_ExperimentWIDContactWID1` (`ExperimentWID`),
  KEY `FK_ExperimentWIDContactWID2` (`ContactWID`),
  CONSTRAINT `FK_ExperimentWIDContactWID1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExperimentWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experimwidbioassaydataclustwid`
--

DROP TABLE IF EXISTS `experimwidbioassaydataclustwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experimwidbioassaydataclustwid` (
  `ExperimentWID` bigint(20) NOT NULL,
  `BioAssayDataClusterWID` bigint(20) NOT NULL,
  KEY `FK_ExperimWIDBioAssayDataClu1` (`ExperimentWID`),
  KEY `FK_ExperimWIDBioAssayDataClu2` (`BioAssayDataClusterWID`),
  CONSTRAINT `FK_ExperimWIDBioAssayDataClu1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExperimWIDBioAssayDataClu2` FOREIGN KEY (`BioAssayDataClusterWID`) REFERENCES `bioassaydatacluster` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `factorvalue`
--

DROP TABLE IF EXISTS `factorvalue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `factorvalue` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `ExperimentalFactor` bigint(20) DEFAULT NULL,
  `ExperimentalFactor2` bigint(20) DEFAULT NULL,
  `FactorValue_Measurement` bigint(20) DEFAULT NULL,
  `FactorValue_Value` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_FactorValue1` (`DataSetWID`),
  KEY `FK_FactorValue3` (`ExperimentalFactor`),
  KEY `FK_FactorValue4` (`ExperimentalFactor2`),
  KEY `FK_FactorValue5` (`FactorValue_Measurement`),
  KEY `FK_FactorValue6` (`FactorValue_Value`),
  CONSTRAINT `FK_FactorValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FactorValue3` FOREIGN KEY (`ExperimentalFactor`) REFERENCES `experimentalfactor` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FactorValue4` FOREIGN KEY (`ExperimentalFactor2`) REFERENCES `experimentalfactor` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FactorValue5` FOREIGN KEY (`FactorValue_Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FactorValue6` FOREIGN KEY (`FactorValue_Value`) REFERENCES `term` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `feature`
--

DROP TABLE IF EXISTS `feature`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `feature` (
  `WID` bigint(20) NOT NULL,
  `Description` varchar(1300) DEFAULT NULL,
  `Type` varchar(50) DEFAULT NULL,
  `Class` varchar(50) DEFAULT NULL,
  `SequenceType` char(1) NOT NULL,
  `SequenceWID` bigint(20) DEFAULT NULL,
  `Variant` longtext,
  `RegionOrPoint` varchar(10) DEFAULT NULL,
  `PointType` varchar(10) DEFAULT NULL,
  `StartPosition` int(11) DEFAULT NULL,
  `EndPosition` int(11) DEFAULT NULL,
  `StartPositionApproximate` varchar(10) DEFAULT NULL,
  `EndPositionApproximate` varchar(10) DEFAULT NULL,
  `ExperimentalSupport` char(1) DEFAULT NULL,
  `ComputationalSupport` char(1) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Feature` (`DataSetWID`),
  CONSTRAINT `FK_Feature` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `featuredefect`
--

DROP TABLE IF EXISTS `featuredefect`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `featuredefect` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `ArrayManufactureDeviation` bigint(20) DEFAULT NULL,
  `FeatureDefect_DefectType` bigint(20) DEFAULT NULL,
  `FeatureDefect_PositionDelta` bigint(20) DEFAULT NULL,
  `Feature` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_FeatureDefect1` (`DataSetWID`),
  KEY `FK_FeatureDefect2` (`ArrayManufactureDeviation`),
  KEY `FK_FeatureDefect3` (`FeatureDefect_DefectType`),
  KEY `FK_FeatureDefect4` (`FeatureDefect_PositionDelta`),
  KEY `FK_FeatureDefect5` (`Feature`),
  CONSTRAINT `FK_FeatureDefect1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureDefect2` FOREIGN KEY (`ArrayManufactureDeviation`) REFERENCES `arraymanufacturedeviation` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureDefect3` FOREIGN KEY (`FeatureDefect_DefectType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureDefect4` FOREIGN KEY (`FeatureDefect_PositionDelta`) REFERENCES `positiondelta` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureDefect5` FOREIGN KEY (`Feature`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `featuredimensionwidfeaturewid`
--

DROP TABLE IF EXISTS `featuredimensionwidfeaturewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `featuredimensionwidfeaturewid` (
  `FeatureDimensionWID` bigint(20) NOT NULL,
  `FeatureWID` bigint(20) NOT NULL,
  KEY `FK_FeatureDimensionWIDFeatur1` (`FeatureDimensionWID`),
  KEY `FK_FeatureDimensionWIDFeatur2` (`FeatureWID`),
  CONSTRAINT `FK_FeatureDimensionWIDFeatur1` FOREIGN KEY (`FeatureDimensionWID`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureDimensionWIDFeatur2` FOREIGN KEY (`FeatureWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `featureinformation`
--

DROP TABLE IF EXISTS `featureinformation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `featureinformation` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Feature` bigint(20) DEFAULT NULL,
  `FeatureReporterMap` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_FeatureInformation1` (`DataSetWID`),
  KEY `FK_FeatureInformation2` (`Feature`),
  KEY `FK_FeatureInformation3` (`FeatureReporterMap`),
  CONSTRAINT `FK_FeatureInformation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureInformation2` FOREIGN KEY (`Feature`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureInformation3` FOREIGN KEY (`FeatureReporterMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `featurelocation`
--

DROP TABLE IF EXISTS `featurelocation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `featurelocation` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Row_` smallint(6) DEFAULT NULL,
  `Column_` smallint(6) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_FeatureLocation1` (`DataSetWID`),
  CONSTRAINT `FK_FeatureLocation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `featurewidfeaturewid`
--

DROP TABLE IF EXISTS `featurewidfeaturewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `featurewidfeaturewid` (
  `FeatureWID1` bigint(20) NOT NULL,
  `FeatureWID2` bigint(20) NOT NULL,
  KEY `FK_FeatureWIDFeatureWID1` (`FeatureWID1`),
  KEY `FK_FeatureWIDFeatureWID2` (`FeatureWID2`),
  CONSTRAINT `FK_FeatureWIDFeatureWID1` FOREIGN KEY (`FeatureWID1`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureWIDFeatureWID2` FOREIGN KEY (`FeatureWID2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `featurewidfeaturewid2`
--

DROP TABLE IF EXISTS `featurewidfeaturewid2`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
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
-- Table structure for table `fiducial`
--

DROP TABLE IF EXISTS `fiducial`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fiducial` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `ArrayGroup_Fiducials` bigint(20) DEFAULT NULL,
  `Fiducial_FiducialType` bigint(20) DEFAULT NULL,
  `Fiducial_DistanceUnit` bigint(20) DEFAULT NULL,
  `Fiducial_Position` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Fiducial1` (`DataSetWID`),
  KEY `FK_Fiducial3` (`ArrayGroup_Fiducials`),
  KEY `FK_Fiducial4` (`Fiducial_FiducialType`),
  KEY `FK_Fiducial5` (`Fiducial_DistanceUnit`),
  KEY `FK_Fiducial6` (`Fiducial_Position`),
  CONSTRAINT `FK_Fiducial1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Fiducial3` FOREIGN KEY (`ArrayGroup_Fiducials`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Fiducial4` FOREIGN KEY (`Fiducial_FiducialType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Fiducial5` FOREIGN KEY (`Fiducial_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Fiducial6` FOREIGN KEY (`Fiducial_Position`) REFERENCES `position_` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `flowcytometryprobe`
--

DROP TABLE IF EXISTS `flowcytometryprobe`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `flowcytometryprobe` (
  `WID` bigint(20) NOT NULL,
  `Type` varchar(100) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FlowCytometryProbe_DWID` (`DataSetWID`),
  CONSTRAINT `FK_Probe1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `flowcytometrysample`
--

DROP TABLE IF EXISTS `flowcytometrysample`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `flowcytometrysample` (
  `WID` bigint(20) NOT NULL,
  `BioSourceWID` bigint(20) DEFAULT NULL,
  `FlowCytometryProbeWID` bigint(20) DEFAULT NULL,
  `MeasurementWID` bigint(20) DEFAULT NULL,
  `ManufacturerWID` bigint(20) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FlowCytometrySample_DWID` (`DataSetWID`),
  KEY `FK_FlowCytometrySample1` (`BioSourceWID`),
  KEY `FK_FlowCytometrySample2` (`FlowCytometryProbeWID`),
  KEY `FK_FlowCytometrySample3` (`MeasurementWID`),
  KEY `FK_FlowCytometrySample4` (`ManufacturerWID`),
  CONSTRAINT `FK_FlowCytometrySample1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FlowCytometrySample2` FOREIGN KEY (`FlowCytometryProbeWID`) REFERENCES `flowcytometryprobe` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FlowCytometrySample3` FOREIGN KEY (`MeasurementWID`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FlowCytometrySample4` FOREIGN KEY (`ManufacturerWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FlowCytometrySampleDS` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `function`
--

DROP TABLE IF EXISTS `function`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `function` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Function` (`DataSetWID`),
  CONSTRAINT `FK_Function` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gellocation`
--

DROP TABLE IF EXISTS `gellocation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gellocation` (
  `WID` bigint(20) NOT NULL,
  `SpotWID` bigint(20) NOT NULL,
  `Xcoord` float DEFAULT NULL,
  `Ycoord` float DEFAULT NULL,
  `refGel` varchar(1) DEFAULT NULL,
  `ExperimentWID` bigint(20) NOT NULL,
  `DatasetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_GelLocSpotWid` (`SpotWID`),
  KEY `FK_GelLocExp` (`ExperimentWID`),
  KEY `FK_GelLocDataset` (`DatasetWID`),
  CONSTRAINT `FK_GelLocDataset` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_GelLocExp` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_GelLocSpotWid` FOREIGN KEY (`SpotWID`) REFERENCES `spot` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene`
--

DROP TABLE IF EXISTS `gene`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `NucleicAcidWID` bigint(20) DEFAULT NULL,
  `SubsequenceWID` bigint(20) DEFAULT NULL,
  `Type` varchar(100) DEFAULT NULL,
  `GenomeID` varchar(35) DEFAULT NULL,
  `CodingRegionStart` int(11) DEFAULT NULL,
  `CodingRegionEnd` int(11) DEFAULT NULL,
  `CodingRegionStartApproximate` varchar(10) DEFAULT NULL,
  `CodingRegionEndApproximate` varchar(10) DEFAULT NULL,
  `Direction` varchar(25) DEFAULT NULL,
  `Interrupted` char(1) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `GENE_DATASETWID` (`DataSetWID`),
  KEY `FK_Gene1` (`NucleicAcidWID`),
  CONSTRAINT `FK_Gene1` FOREIGN KEY (`NucleicAcidWID`) REFERENCES `nucleicacid` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Gene2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `geneexpressiondata`
--

DROP TABLE IF EXISTS `geneexpressiondata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geneexpressiondata` (
  `B` smallint(6) NOT NULL,
  `D` smallint(6) NOT NULL,
  `Q` smallint(6) NOT NULL,
  `Value` varchar(100) NOT NULL,
  `BioAssayValuesWID` bigint(20) NOT NULL,
  KEY `FK_GEDBAV` (`BioAssayValuesWID`),
  CONSTRAINT `FK_GEDBAV` FOREIGN KEY (`BioAssayValuesWID`) REFERENCES `biodatavalues` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `geneticcode`
--

DROP TABLE IF EXISTS `geneticcode`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geneticcode` (
  `WID` bigint(20) NOT NULL,
  `NCBIID` varchar(2) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `TranslationTable` varchar(64) DEFAULT NULL,
  `StartCodon` varchar(64) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_GeneticCode` (`DataSetWID`),
  CONSTRAINT `FK_GeneticCode` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `genewidnucleicacidwid`
--

DROP TABLE IF EXISTS `genewidnucleicacidwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `genewidnucleicacidwid` (
  `GeneWID` bigint(20) NOT NULL,
  `NucleicAcidWID` bigint(20) NOT NULL,
  KEY `FK_GeneWIDNucleicAcidWID1` (`GeneWID`),
  KEY `FK_GeneWIDNucleicAcidWID2` (`NucleicAcidWID`),
  CONSTRAINT `FK_GeneWIDNucleicAcidWID1` FOREIGN KEY (`GeneWID`) REFERENCES `gene` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_GeneWIDNucleicAcidWID2` FOREIGN KEY (`NucleicAcidWID`) REFERENCES `nucleicacid` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `genewidproteinwid`
--

DROP TABLE IF EXISTS `genewidproteinwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `genewidproteinwid` (
  `GeneWID` bigint(20) NOT NULL,
  `ProteinWID` bigint(20) NOT NULL,
  KEY `FK_GeneWIDProteinWID1` (`GeneWID`),
  KEY `FK_GeneWIDProteinWID2` (`ProteinWID`),
  CONSTRAINT `FK_GeneWIDProteinWID1` FOREIGN KEY (`GeneWID`) REFERENCES `gene` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_GeneWIDProteinWID2` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `hardwarewidcontactwid`
--

DROP TABLE IF EXISTS `hardwarewidcontactwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `hardwarewidcontactwid` (
  `HardwareWID` bigint(20) NOT NULL,
  `ContactWID` bigint(20) NOT NULL,
  KEY `FK_HardwareWIDContactWID1` (`HardwareWID`),
  KEY `FK_HardwareWIDContactWID2` (`ContactWID`),
  CONSTRAINT `FK_HardwareWIDContactWID1` FOREIGN KEY (`HardwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_HardwareWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `hardwarewidsoftwarewid`
--

DROP TABLE IF EXISTS `hardwarewidsoftwarewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `hardwarewidsoftwarewid` (
  `HardwareWID` bigint(20) NOT NULL,
  `SoftwareWID` bigint(20) NOT NULL,
  KEY `FK_HardwareWIDSoftwareWID1` (`HardwareWID`),
  KEY `FK_HardwareWIDSoftwareWID2` (`SoftwareWID`),
  CONSTRAINT `FK_HardwareWIDSoftwareWID1` FOREIGN KEY (`HardwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_HardwareWIDSoftwareWID2` FOREIGN KEY (`SoftwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `image`
--

DROP TABLE IF EXISTS `image`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `image` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `URI` varchar(255) DEFAULT NULL,
  `Image_Format` bigint(20) DEFAULT NULL,
  `PhysicalBioAssay` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Image1` (`DataSetWID`),
  KEY `FK_Image3` (`Image_Format`),
  KEY `FK_Image4` (`PhysicalBioAssay`),
  CONSTRAINT `FK_Image1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Image3` FOREIGN KEY (`Image_Format`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Image4` FOREIGN KEY (`PhysicalBioAssay`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `imageacquisitionwidimagewid`
--

DROP TABLE IF EXISTS `imageacquisitionwidimagewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `imageacquisitionwidimagewid` (
  `ImageAcquisitionWID` bigint(20) NOT NULL,
  `ImageWID` bigint(20) NOT NULL,
  KEY `FK_ImageAcquisitionWIDImageW1` (`ImageAcquisitionWID`),
  KEY `FK_ImageAcquisitionWIDImageW2` (`ImageWID`),
  CONSTRAINT `FK_ImageAcquisitionWIDImageW1` FOREIGN KEY (`ImageAcquisitionWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ImageAcquisitionWIDImageW2` FOREIGN KEY (`ImageWID`) REFERENCES `image` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `imagewidchannelwid`
--

DROP TABLE IF EXISTS `imagewidchannelwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `imagewidchannelwid` (
  `ImageWID` bigint(20) NOT NULL,
  `ChannelWID` bigint(20) NOT NULL,
  KEY `FK_ImageWIDChannelWID1` (`ImageWID`),
  KEY `FK_ImageWIDChannelWID2` (`ChannelWID`),
  CONSTRAINT `FK_ImageWIDChannelWID1` FOREIGN KEY (`ImageWID`) REFERENCES `image` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ImageWIDChannelWID2` FOREIGN KEY (`ChannelWID`) REFERENCES `channel` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `interaction`
--

DROP TABLE IF EXISTS `interaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `interaction` (
  `WID` bigint(20) NOT NULL,
  `Type` varchar(100) DEFAULT NULL,
  `Name` varchar(120) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `INTERACTION_DWID` (`DataSetWID`),
  CONSTRAINT `FK_Interaction1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `interactionparticipant`
--

DROP TABLE IF EXISTS `interactionparticipant`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `interactionparticipant` (
  `InteractionWID` bigint(20) NOT NULL,
  `OtherWID` bigint(20) NOT NULL,
  `Coefficient` smallint(6) DEFAULT NULL,
  KEY `PR_INTERACTIONWID_OTHERWID` (`InteractionWID`,`OtherWID`),
  CONSTRAINT `FK_InteractionParticipant1` FOREIGN KEY (`InteractionWID`) REFERENCES `interaction` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `labeledextractwidcompoundwid`
--

DROP TABLE IF EXISTS `labeledextractwidcompoundwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `labeledextractwidcompoundwid` (
  `LabeledExtractWID` bigint(20) NOT NULL,
  `CompoundWID` bigint(20) NOT NULL,
  KEY `FK_LabeledExtractWIDCompound1` (`LabeledExtractWID`),
  KEY `FK_LabeledExtractWIDCompound2` (`CompoundWID`),
  CONSTRAINT `FK_LabeledExtractWIDCompound1` FOREIGN KEY (`LabeledExtractWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_LabeledExtractWIDCompound2` FOREIGN KEY (`CompoundWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `lightsource`
--

DROP TABLE IF EXISTS `lightsource`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `lightsource` (
  `WID` bigint(20) NOT NULL,
  `Wavelength` float DEFAULT NULL,
  `Type` varchar(100) DEFAULT NULL,
  `InstrumentWID` bigint(20) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `LightSource_DWID` (`DataSetWID`),
  CONSTRAINT `FK_LightSource1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `location`
--

DROP TABLE IF EXISTS `location`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `location` (
  `ProteinWID` bigint(20) NOT NULL,
  `Location` varchar(100) NOT NULL,
  KEY `FK_Location` (`ProteinWID`),
  CONSTRAINT `FK_Location` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `manufacturelims`
--

DROP TABLE IF EXISTS `manufacturelims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `manufacturelims` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `ArrayManufacture_FeatureLIMSs` bigint(20) DEFAULT NULL,
  `Quality` varchar(255) DEFAULT NULL,
  `Feature` bigint(20) DEFAULT NULL,
  `BioMaterial` bigint(20) DEFAULT NULL,
  `ManufactureLIMS_IdentifierLIMS` bigint(20) DEFAULT NULL,
  `BioMaterialPlateIdentifier` varchar(255) DEFAULT NULL,
  `BioMaterialPlateRow` varchar(255) DEFAULT NULL,
  `BioMaterialPlateCol` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ManufactureLIMS1` (`DataSetWID`),
  KEY `FK_ManufactureLIMS3` (`ArrayManufacture_FeatureLIMSs`),
  KEY `FK_ManufactureLIMS4` (`Feature`),
  KEY `FK_ManufactureLIMS5` (`BioMaterial`),
  CONSTRAINT `FK_ManufactureLIMS1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ManufactureLIMS3` FOREIGN KEY (`ArrayManufacture_FeatureLIMSs`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ManufactureLIMS4` FOREIGN KEY (`Feature`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ManufactureLIMS5` FOREIGN KEY (`BioMaterial`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `measbassaywidmeasbassaydatawid`
--

DROP TABLE IF EXISTS `measbassaywidmeasbassaydatawid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `measbassaywidmeasbassaydatawid` (
  `MeasuredBioAssayWID` bigint(20) NOT NULL,
  `MeasuredBioAssayDataWID` bigint(20) NOT NULL,
  KEY `FK_MeasBAssayWIDMeasBAssayDa1` (`MeasuredBioAssayWID`),
  KEY `FK_MeasBAssayWIDMeasBAssayDa2` (`MeasuredBioAssayDataWID`),
  CONSTRAINT `FK_MeasBAssayWIDMeasBAssayDa1` FOREIGN KEY (`MeasuredBioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_MeasBAssayWIDMeasBAssayDa2` FOREIGN KEY (`MeasuredBioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `measurement`
--

DROP TABLE IF EXISTS `measurement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `measurement` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Type_` varchar(25) DEFAULT NULL,
  `Value` varchar(255) DEFAULT NULL,
  `KindCV` varchar(25) DEFAULT NULL,
  `OtherKind` varchar(255) DEFAULT NULL,
  `Measurement_Unit` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Measurement1` (`DataSetWID`),
  KEY `FK_Measurement2` (`Measurement_Unit`),
  CONSTRAINT `FK_Measurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Measurement2` FOREIGN KEY (`Measurement_Unit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mismatchinformation`
--

DROP TABLE IF EXISTS `mismatchinformation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mismatchinformation` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `CompositePosition` bigint(20) DEFAULT NULL,
  `FeatureInformation` bigint(20) DEFAULT NULL,
  `StartCoord` smallint(6) DEFAULT NULL,
  `NewSequence` varchar(255) DEFAULT NULL,
  `ReplacedLength` smallint(6) DEFAULT NULL,
  `ReporterPosition` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_MismatchInformation1` (`DataSetWID`),
  KEY `FK_MismatchInformation2` (`CompositePosition`),
  KEY `FK_MismatchInformation3` (`FeatureInformation`),
  KEY `FK_MismatchInformation4` (`ReporterPosition`),
  CONSTRAINT `FK_MismatchInformation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_MismatchInformation2` FOREIGN KEY (`CompositePosition`) REFERENCES `sequenceposition` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_MismatchInformation3` FOREIGN KEY (`FeatureInformation`) REFERENCES `featureinformation` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_MismatchInformation4` FOREIGN KEY (`ReporterPosition`) REFERENCES `sequenceposition` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `namevaluetype`
--

DROP TABLE IF EXISTS `namevaluetype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `namevaluetype` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Value` varchar(255) DEFAULT NULL,
  `Type_` varchar(255) DEFAULT NULL,
  `NameValueType_PropertySets` bigint(20) DEFAULT NULL,
  `OtherWID` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_NameValueType1` (`DataSetWID`),
  KEY `FK_NameValueType66` (`NameValueType_PropertySets`),
  CONSTRAINT `FK_NameValueType1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NameValueType66` FOREIGN KEY (`NameValueType_PropertySets`) REFERENCES `namevaluetype` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `node`
--

DROP TABLE IF EXISTS `node`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `node` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `BioAssayDataCluster_Nodes` bigint(20) DEFAULT NULL,
  `Node_Nodes` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Node1` (`DataSetWID`),
  KEY `FK_Node3` (`BioAssayDataCluster_Nodes`),
  KEY `FK_Node4` (`Node_Nodes`),
  CONSTRAINT `FK_Node1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Node3` FOREIGN KEY (`BioAssayDataCluster_Nodes`) REFERENCES `bioassaydatacluster` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Node4` FOREIGN KEY (`Node_Nodes`) REFERENCES `node` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `nodecontents`
--

DROP TABLE IF EXISTS `nodecontents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `nodecontents` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Node_NodeContents` bigint(20) DEFAULT NULL,
  `BioAssayDimension` bigint(20) DEFAULT NULL,
  `DesignElementDimension` bigint(20) DEFAULT NULL,
  `QuantitationDimension` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_NodeContents1` (`DataSetWID`),
  KEY `FK_NodeContents3` (`Node_NodeContents`),
  KEY `FK_NodeContents4` (`BioAssayDimension`),
  KEY `FK_NodeContents5` (`DesignElementDimension`),
  KEY `FK_NodeContents6` (`QuantitationDimension`),
  CONSTRAINT `FK_NodeContents1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeContents3` FOREIGN KEY (`Node_NodeContents`) REFERENCES `node` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeContents4` FOREIGN KEY (`BioAssayDimension`) REFERENCES `bioassaydimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeContents5` FOREIGN KEY (`DesignElementDimension`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeContents6` FOREIGN KEY (`QuantitationDimension`) REFERENCES `quantitationtypedimension` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `nodevalue`
--

DROP TABLE IF EXISTS `nodevalue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `nodevalue` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Node_NodeValue` bigint(20) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Value` varchar(255) DEFAULT NULL,
  `NodeValue_Type` bigint(20) DEFAULT NULL,
  `NodeValue_Scale` bigint(20) DEFAULT NULL,
  `NodeValue_DataType` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_NodeValue1` (`DataSetWID`),
  KEY `FK_NodeValue2` (`Node_NodeValue`),
  KEY `FK_NodeValue3` (`NodeValue_Type`),
  KEY `FK_NodeValue4` (`NodeValue_Scale`),
  KEY `FK_NodeValue5` (`NodeValue_DataType`),
  CONSTRAINT `FK_NodeValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeValue2` FOREIGN KEY (`Node_NodeValue`) REFERENCES `node` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeValue3` FOREIGN KEY (`NodeValue_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeValue4` FOREIGN KEY (`NodeValue_Scale`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeValue5` FOREIGN KEY (`NodeValue_DataType`) REFERENCES `term` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `nucleicacid`
--

DROP TABLE IF EXISTS `nucleicacid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `nucleicacid` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(200) DEFAULT NULL,
  `Type` varchar(30) NOT NULL,
  `Class` varchar(30) DEFAULT NULL,
  `Topology` varchar(30) DEFAULT NULL,
  `Strandedness` varchar(30) DEFAULT NULL,
  `SequenceDerivation` varchar(30) DEFAULT NULL,
  `Fragment` char(1) DEFAULT NULL,
  `FullySequenced` char(1) DEFAULT NULL,
  `MoleculeLength` int(11) DEFAULT NULL,
  `MoleculeLengthApproximate` varchar(10) DEFAULT NULL,
  `CumulativeLength` int(11) DEFAULT NULL,
  `CumulativeLengthApproximate` varchar(10) DEFAULT NULL,
  `GeneticCodeWID` bigint(20) DEFAULT NULL,
  `BioSourceWID` bigint(20) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_NucleicAcid1` (`GeneticCodeWID`),
  KEY `FK_NucleicAcid2` (`BioSourceWID`),
  KEY `FK_NucleicAcid3` (`DataSetWID`),
  CONSTRAINT `FK_NucleicAcid1` FOREIGN KEY (`GeneticCodeWID`) REFERENCES `geneticcode` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NucleicAcid2` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NucleicAcid3` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `parameter`
--

DROP TABLE IF EXISTS `parameter`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `parameter` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Parameter_DefaultValue` bigint(20) DEFAULT NULL,
  `Parameter_DataType` bigint(20) DEFAULT NULL,
  `Parameterizable_ParameterTypes` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Parameter1` (`DataSetWID`),
  KEY `FK_Parameter3` (`Parameter_DefaultValue`),
  KEY `FK_Parameter4` (`Parameter_DataType`),
  KEY `FK_Parameter5` (`Parameterizable_ParameterTypes`),
  CONSTRAINT `FK_Parameter1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Parameter3` FOREIGN KEY (`Parameter_DefaultValue`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Parameter4` FOREIGN KEY (`Parameter_DataType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Parameter5` FOREIGN KEY (`Parameterizable_ParameterTypes`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `parameterizable`
--

DROP TABLE IF EXISTS `parameterizable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `parameterizable` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `URI` varchar(255) DEFAULT NULL,
  `Model` varchar(255) DEFAULT NULL,
  `Make` varchar(255) DEFAULT NULL,
  `Hardware_Type` bigint(20) DEFAULT NULL,
  `Text` varchar(1000) DEFAULT NULL,
  `Title` varchar(255) DEFAULT NULL,
  `Protocol_Type` bigint(20) DEFAULT NULL,
  `Software_Type` bigint(20) DEFAULT NULL,
  `Hardware` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Parameterizable1` (`DataSetWID`),
  KEY `FK_Parameterizable3` (`Hardware_Type`),
  KEY `FK_Parameterizable4` (`Protocol_Type`),
  KEY `FK_Parameterizable5` (`Software_Type`),
  KEY `FK_Parameterizable6` (`Hardware`),
  CONSTRAINT `FK_Parameterizable1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Parameterizable3` FOREIGN KEY (`Hardware_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Parameterizable4` FOREIGN KEY (`Protocol_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Parameterizable5` FOREIGN KEY (`Software_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Parameterizable6` FOREIGN KEY (`Hardware`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `parameterizableapplication`
--

DROP TABLE IF EXISTS `parameterizableapplication`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `parameterizableapplication` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `ArrayDesign` bigint(20) DEFAULT NULL,
  `ArrayManufacture` bigint(20) DEFAULT NULL,
  `BioEvent_ProtocolApplications` bigint(20) DEFAULT NULL,
  `SerialNumber` varchar(255) DEFAULT NULL,
  `Hardware` bigint(20) DEFAULT NULL,
  `ActivityDate` varchar(255) DEFAULT NULL,
  `ProtocolApplication` bigint(20) DEFAULT NULL,
  `ProtocolApplication2` bigint(20) DEFAULT NULL,
  `Protocol` bigint(20) DEFAULT NULL,
  `Version` varchar(255) DEFAULT NULL,
  `ReleaseDate` datetime DEFAULT NULL,
  `Software` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ParameterizableApplicatio1` (`DataSetWID`),
  KEY `FK_ParameterizableApplicatio3` (`ArrayDesign`),
  KEY `FK_ParameterizableApplicatio4` (`ArrayManufacture`),
  KEY `FK_ParameterizableApplicatio5` (`BioEvent_ProtocolApplications`),
  KEY `FK_ParameterizableApplicatio6` (`Hardware`),
  KEY `FK_ParameterizableApplicatio7` (`ProtocolApplication`),
  KEY `FK_ParameterizableApplicatio8` (`ProtocolApplication2`),
  KEY `FK_ParameterizableApplicatio9` (`Protocol`),
  KEY `FK_ParameterizableApplicatio10` (`Software`),
  CONSTRAINT `FK_ParameterizableApplicatio1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio10` FOREIGN KEY (`Software`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio3` FOREIGN KEY (`ArrayDesign`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio4` FOREIGN KEY (`ArrayManufacture`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio5` FOREIGN KEY (`BioEvent_ProtocolApplications`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio6` FOREIGN KEY (`Hardware`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio7` FOREIGN KEY (`ProtocolApplication`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio8` FOREIGN KEY (`ProtocolApplication2`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio9` FOREIGN KEY (`Protocol`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `parametervalue`
--

DROP TABLE IF EXISTS `parametervalue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `parametervalue` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Value` varchar(255) DEFAULT NULL,
  `ParameterType` bigint(20) DEFAULT NULL,
  `ParameterizableApplication` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ParameterValue1` (`DataSetWID`),
  KEY `FK_ParameterValue2` (`ParameterType`),
  KEY `FK_ParameterValue3` (`ParameterizableApplication`),
  CONSTRAINT `FK_ParameterValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterValue2` FOREIGN KEY (`ParameterType`) REFERENCES `parameter` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterValue3` FOREIGN KEY (`ParameterizableApplication`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathway`
--

DROP TABLE IF EXISTS `pathway`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathway` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Type` char(1) NOT NULL,
  `BioSourceWID` bigint(20) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `PATHWAY_BSWID_WID_DWID` (`BioSourceWID`,`WID`,`DataSetWID`),
  KEY `PATHWAY_TYPE_WID_DWID` (`Type`,`WID`,`DataSetWID`),
  KEY `PATHWAY_DWID` (`DataSetWID`),
  CONSTRAINT `FK_Pathway1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Pathway2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathwaylink`
--

DROP TABLE IF EXISTS `pathwaylink`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathwaylink` (
  `Pathway1WID` bigint(20) NOT NULL,
  `Pathway2WID` bigint(20) NOT NULL,
  `ChemicalWID` bigint(20) NOT NULL,
  KEY `FK_PathwayLink1` (`Pathway1WID`),
  KEY `FK_PathwayLink2` (`Pathway2WID`),
  KEY `FK_PathwayLink3` (`ChemicalWID`),
  CONSTRAINT `FK_PathwayLink1` FOREIGN KEY (`Pathway1WID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_PathwayLink2` FOREIGN KEY (`Pathway2WID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_PathwayLink3` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathwayreaction`
--

DROP TABLE IF EXISTS `pathwayreaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathwayreaction` (
  `PathwayWID` bigint(20) NOT NULL,
  `ReactionWID` bigint(20) NOT NULL,
  `PriorReactionWID` bigint(20) DEFAULT NULL,
  `Hypothetical` char(1) NOT NULL,
  KEY `PR_PATHWID_REACTIONWID` (`PathwayWID`,`ReactionWID`),
  KEY `FK_PathwayReaction3` (`PriorReactionWID`),
  CONSTRAINT `FK_PathwayReaction1` FOREIGN KEY (`PathwayWID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_PathwayReaction3` FOREIGN KEY (`PriorReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `position_`
--

DROP TABLE IF EXISTS `position_`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `position_` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `X` float DEFAULT NULL,
  `Y` float DEFAULT NULL,
  `Position_DistanceUnit` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Position_1` (`DataSetWID`),
  KEY `FK_Position_2` (`Position_DistanceUnit`),
  CONSTRAINT `FK_Position_1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Position_2` FOREIGN KEY (`Position_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `positiondelta`
--

DROP TABLE IF EXISTS `positiondelta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `positiondelta` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `DeltaX` float DEFAULT NULL,
  `DeltaY` float DEFAULT NULL,
  `PositionDelta_DistanceUnit` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_PositionDelta1` (`DataSetWID`),
  KEY `FK_PositionDelta2` (`PositionDelta_DistanceUnit`),
  CONSTRAINT `FK_PositionDelta1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_PositionDelta2` FOREIGN KEY (`PositionDelta_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `product` (
  `ReactionWID` bigint(20) NOT NULL,
  `OtherWID` bigint(20) NOT NULL,
  `Coefficient` smallint(6) NOT NULL,
  KEY `FK_Product` (`ReactionWID`),
  CONSTRAINT `FK_Product` FOREIGN KEY (`ReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein`
--

DROP TABLE IF EXISTS `protein`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein` (
  `WID` bigint(20) NOT NULL,
  `Name` text,
  `AASequence` longtext,
  `Length` int(11) DEFAULT NULL,
  `LengthApproximate` varchar(10) DEFAULT NULL,
  `Charge` smallint(6) DEFAULT NULL,
  `Fragment` char(1) DEFAULT NULL,
  `MolecularWeightCalc` float DEFAULT NULL,
  `MolecularWeightExp` float DEFAULT NULL,
  `PICalc` varchar(50) DEFAULT NULL,
  `PIExp` varchar(50) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `PROTEIN_DWID` (`DataSetWID`),
  CONSTRAINT `FK_Protein` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `proteinwidfunctionwid`
--

DROP TABLE IF EXISTS `proteinwidfunctionwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `proteinwidfunctionwid` (
  `ProteinWID` bigint(20) NOT NULL,
  `FunctionWID` bigint(20) NOT NULL,
  KEY `FK_ProteinWIDFunctionWID2` (`ProteinWID`),
  KEY `FK_ProteinWIDFunctionWID3` (`FunctionWID`),
  CONSTRAINT `FK_ProteinWIDFunctionWID2` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProteinWIDFunctionWID3` FOREIGN KEY (`FunctionWID`) REFERENCES `function` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `proteinwidspotwid`
--

DROP TABLE IF EXISTS `proteinwidspotwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `proteinwidspotwid` (
  `ProteinWID` bigint(20) NOT NULL,
  `SpotWID` bigint(20) NOT NULL,
  KEY `FK_ProteinWIDSpotWID1` (`ProteinWID`),
  KEY `FK_ProteinWIDSpotWID2` (`SpotWID`),
  CONSTRAINT `FK_ProteinWIDSpotWID1` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProteinWIDSpotWID2` FOREIGN KEY (`SpotWID`) REFERENCES `spot` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protocolapplwidpersonwid`
--

DROP TABLE IF EXISTS `protocolapplwidpersonwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protocolapplwidpersonwid` (
  `ProtocolApplicationWID` bigint(20) NOT NULL,
  `PersonWID` bigint(20) NOT NULL,
  KEY `FK_ProtocolApplWIDPersonWID1` (`ProtocolApplicationWID`),
  KEY `FK_ProtocolApplWIDPersonWID2` (`PersonWID`),
  CONSTRAINT `FK_ProtocolApplWIDPersonWID1` FOREIGN KEY (`ProtocolApplicationWID`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProtocolApplWIDPersonWID2` FOREIGN KEY (`PersonWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protocolwidhardwarewid`
--

DROP TABLE IF EXISTS `protocolwidhardwarewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protocolwidhardwarewid` (
  `ProtocolWID` bigint(20) NOT NULL,
  `HardwareWID` bigint(20) NOT NULL,
  KEY `FK_ProtocolWIDHardwareWID1` (`ProtocolWID`),
  KEY `FK_ProtocolWIDHardwareWID2` (`HardwareWID`),
  CONSTRAINT `FK_ProtocolWIDHardwareWID1` FOREIGN KEY (`ProtocolWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProtocolWIDHardwareWID2` FOREIGN KEY (`HardwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protocolwidsoftwarewid`
--

DROP TABLE IF EXISTS `protocolwidsoftwarewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protocolwidsoftwarewid` (
  `ProtocolWID` bigint(20) NOT NULL,
  `SoftwareWID` bigint(20) NOT NULL,
  KEY `FK_ProtocolWIDSoftwareWID1` (`ProtocolWID`),
  KEY `FK_ProtocolWIDSoftwareWID2` (`SoftwareWID`),
  CONSTRAINT `FK_ProtocolWIDSoftwareWID1` FOREIGN KEY (`ProtocolWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProtocolWIDSoftwareWID2` FOREIGN KEY (`SoftwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quantitationtype`
--

DROP TABLE IF EXISTS `quantitationtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quantitationtype` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `IsBackground` char(1) DEFAULT NULL,
  `Channel` bigint(20) DEFAULT NULL,
  `QuantitationType_Scale` bigint(20) DEFAULT NULL,
  `QuantitationType_DataType` bigint(20) DEFAULT NULL,
  `TargetQuantitationType` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_QuantitationType1` (`DataSetWID`),
  KEY `FK_QuantitationType3` (`Channel`),
  KEY `FK_QuantitationType4` (`QuantitationType_Scale`),
  KEY `FK_QuantitationType5` (`QuantitationType_DataType`),
  KEY `FK_QuantitationType6` (`TargetQuantitationType`),
  CONSTRAINT `FK_QuantitationType1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantitationType3` FOREIGN KEY (`Channel`) REFERENCES `channel` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantitationType4` FOREIGN KEY (`QuantitationType_Scale`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantitationType5` FOREIGN KEY (`QuantitationType_DataType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantitationType6` FOREIGN KEY (`TargetQuantitationType`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quantitationtypedimension`
--

DROP TABLE IF EXISTS `quantitationtypedimension`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quantitationtypedimension` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_QuantitationTypeDimension1` (`DataSetWID`),
  CONSTRAINT `FK_QuantitationTypeDimension1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quantitationtypemapping`
--

DROP TABLE IF EXISTS `quantitationtypemapping`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quantitationtypemapping` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_QuantitationTypeMapping1` (`DataSetWID`),
  CONSTRAINT `FK_QuantitationTypeMapping1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quantitationtypetuple`
--

DROP TABLE IF EXISTS `quantitationtypetuple`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quantitationtypetuple` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `DesignElementTuple` bigint(20) DEFAULT NULL,
  `QuantitationType` bigint(20) DEFAULT NULL,
  `QuantitationTypeTuple_Datum` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_QuantitationTypeTuple1` (`DataSetWID`),
  KEY `FK_QuantitationTypeTuple2` (`DesignElementTuple`),
  KEY `FK_QuantitationTypeTuple3` (`QuantitationType`),
  KEY `FK_QuantitationTypeTuple4` (`QuantitationTypeTuple_Datum`),
  CONSTRAINT `FK_QuantitationTypeTuple1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantitationTypeTuple2` FOREIGN KEY (`DesignElementTuple`) REFERENCES `designelementtuple` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantitationTypeTuple3` FOREIGN KEY (`QuantitationType`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantitationTypeTuple4` FOREIGN KEY (`QuantitationTypeTuple_Datum`) REFERENCES `datum` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quanttymapwidquanttymapwi`
--

DROP TABLE IF EXISTS `quanttymapwidquanttymapwi`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quanttymapwidquanttymapwi` (
  `QuantitationTypeMappingWID` bigint(20) NOT NULL,
  `QuantitationTypeMapWID` bigint(20) NOT NULL,
  KEY `FK_QuantTyMapWIDQuantTyMapWI1` (`QuantitationTypeMappingWID`),
  KEY `FK_QuantTyMapWIDQuantTyMapWI2` (`QuantitationTypeMapWID`),
  CONSTRAINT `FK_QuantTyMapWIDQuantTyMapWI1` FOREIGN KEY (`QuantitationTypeMappingWID`) REFERENCES `quantitationtypemapping` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantTyMapWIDQuantTyMapWI2` FOREIGN KEY (`QuantitationTypeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quanttypedimenswidquanttypewid`
--

DROP TABLE IF EXISTS `quanttypedimenswidquanttypewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quanttypedimenswidquanttypewid` (
  `QuantitationTypeDimensionWID` bigint(20) NOT NULL,
  `QuantitationTypeWID` bigint(20) NOT NULL,
  KEY `FK_QuantTypeDimensWIDQuantTy1` (`QuantitationTypeDimensionWID`),
  KEY `FK_QuantTypeDimensWIDQuantTy2` (`QuantitationTypeWID`),
  CONSTRAINT `FK_QuantTypeDimensWIDQuantTy1` FOREIGN KEY (`QuantitationTypeDimensionWID`) REFERENCES `quantitationtypedimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantTypeDimensWIDQuantTy2` FOREIGN KEY (`QuantitationTypeWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quanttypemapwidquanttypewid`
--

DROP TABLE IF EXISTS `quanttypemapwidquanttypewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quanttypemapwidquanttypewid` (
  `QuantitationTypeMapWID` bigint(20) NOT NULL,
  `QuantitationTypeWID` bigint(20) NOT NULL,
  KEY `FK_QuantTypeMapWIDQuantTypeW1` (`QuantitationTypeMapWID`),
  KEY `FK_QuantTypeMapWIDQuantTypeW2` (`QuantitationTypeWID`),
  CONSTRAINT `FK_QuantTypeMapWIDQuantTypeW1` FOREIGN KEY (`QuantitationTypeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantTypeMapWIDQuantTypeW2` FOREIGN KEY (`QuantitationTypeWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quanttypewidconfidenceindwid`
--

DROP TABLE IF EXISTS `quanttypewidconfidenceindwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quanttypewidconfidenceindwid` (
  `QuantitationTypeWID` bigint(20) NOT NULL,
  `ConfidenceIndicatorWID` bigint(20) NOT NULL,
  KEY `FK_QuantTypeWIDConfidenceInd1` (`QuantitationTypeWID`),
  KEY `FK_QuantTypeWIDConfidenceInd2` (`ConfidenceIndicatorWID`),
  CONSTRAINT `FK_QuantTypeWIDConfidenceInd1` FOREIGN KEY (`QuantitationTypeWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantTypeWIDConfidenceInd2` FOREIGN KEY (`ConfidenceIndicatorWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quanttypewidquanttypemapwid`
--

DROP TABLE IF EXISTS `quanttypewidquanttypemapwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quanttypewidquanttypemapwid` (
  `QuantitationTypeWID` bigint(20) NOT NULL,
  `QuantitationTypeMapWID` bigint(20) NOT NULL,
  KEY `FK_QuantTypeWIDQuantTypeMapW1` (`QuantitationTypeWID`),
  KEY `FK_QuantTypeWIDQuantTypeMapW2` (`QuantitationTypeMapWID`),
  CONSTRAINT `FK_QuantTypeWIDQuantTypeMapW1` FOREIGN KEY (`QuantitationTypeWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantTypeWIDQuantTypeMapW2` FOREIGN KEY (`QuantitationTypeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactant`
--

DROP TABLE IF EXISTS `reactant`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactant` (
  `ReactionWID` bigint(20) NOT NULL,
  `OtherWID` bigint(20) NOT NULL,
  `Coefficient` smallint(6) NOT NULL,
  KEY `FK_Reactant` (`ReactionWID`),
  CONSTRAINT `FK_Reactant` FOREIGN KEY (`ReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reaction`
--

DROP TABLE IF EXISTS `reaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reaction` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(250) DEFAULT NULL,
  `DeltaG` varchar(50) DEFAULT NULL,
  `ECNumber` varchar(50) DEFAULT NULL,
  `ECNumberProposed` varchar(50) DEFAULT NULL,
  `Spontaneous` char(1) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `REACTION_DWID` (`DataSetWID`),
  CONSTRAINT `FK_Reaction` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `relatedterm`
--

DROP TABLE IF EXISTS `relatedterm`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `relatedterm` (
  `TermWID` bigint(20) NOT NULL,
  `OtherWID` bigint(20) NOT NULL,
  `Relationship` varchar(50) DEFAULT NULL,
  KEY `FK_RelatedTerm1` (`TermWID`),
  CONSTRAINT `FK_RelatedTerm1` FOREIGN KEY (`TermWID`) REFERENCES `term` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reporterdimenswidreporterwid`
--

DROP TABLE IF EXISTS `reporterdimenswidreporterwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reporterdimenswidreporterwid` (
  `ReporterDimensionWID` bigint(20) NOT NULL,
  `ReporterWID` bigint(20) NOT NULL,
  KEY `FK_ReporterDimensWIDReporter1` (`ReporterDimensionWID`),
  KEY `FK_ReporterDimensWIDReporter2` (`ReporterWID`),
  CONSTRAINT `FK_ReporterDimensWIDReporter1` FOREIGN KEY (`ReporterDimensionWID`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ReporterDimensWIDReporter2` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reportergroupwidreporterwid`
--

DROP TABLE IF EXISTS `reportergroupwidreporterwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reportergroupwidreporterwid` (
  `ReporterGroupWID` bigint(20) NOT NULL,
  `ReporterWID` bigint(20) NOT NULL,
  KEY `FK_ReporterGroupWIDReporterW1` (`ReporterGroupWID`),
  KEY `FK_ReporterGroupWIDReporterW2` (`ReporterWID`),
  CONSTRAINT `FK_ReporterGroupWIDReporterW1` FOREIGN KEY (`ReporterGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ReporterGroupWIDReporterW2` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reporterwidbiosequencewid`
--

DROP TABLE IF EXISTS `reporterwidbiosequencewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reporterwidbiosequencewid` (
  `ReporterWID` bigint(20) NOT NULL,
  `BioSequenceWID` bigint(20) NOT NULL,
  KEY `FK_ReporterWIDBioSequenceWID1` (`ReporterWID`),
  CONSTRAINT `FK_ReporterWIDBioSequenceWID1` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reporterwidfeaturerepormapwid`
--

DROP TABLE IF EXISTS `reporterwidfeaturerepormapwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reporterwidfeaturerepormapwid` (
  `ReporterWID` bigint(20) NOT NULL,
  `FeatureReporterMapWID` bigint(20) NOT NULL,
  KEY `FK_ReporterWIDFeatureReporMa1` (`ReporterWID`),
  KEY `FK_ReporterWIDFeatureReporMa2` (`FeatureReporterMapWID`),
  CONSTRAINT `FK_ReporterWIDFeatureReporMa1` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ReporterWIDFeatureReporMa2` FOREIGN KEY (`FeatureReporterMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `seqfeaturelocation`
--

DROP TABLE IF EXISTS `seqfeaturelocation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `seqfeaturelocation` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `SeqFeature_Regions` bigint(20) DEFAULT NULL,
  `StrandType` varchar(255) DEFAULT NULL,
  `SeqFeatureLocation_Subregions` bigint(20) DEFAULT NULL,
  `SeqFeatureLocation_Coordinate` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_SeqFeatureLocation1` (`DataSetWID`),
  KEY `FK_SeqFeatureLocation2` (`SeqFeature_Regions`),
  KEY `FK_SeqFeatureLocation3` (`SeqFeatureLocation_Subregions`),
  KEY `FK_SeqFeatureLocation4` (`SeqFeatureLocation_Coordinate`),
  CONSTRAINT `FK_SeqFeatureLocation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SeqFeatureLocation2` FOREIGN KEY (`SeqFeature_Regions`) REFERENCES `feature` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SeqFeatureLocation3` FOREIGN KEY (`SeqFeatureLocation_Subregions`) REFERENCES `seqfeaturelocation` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SeqFeatureLocation4` FOREIGN KEY (`SeqFeatureLocation_Coordinate`) REFERENCES `sequenceposition` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sequencematch`
--

DROP TABLE IF EXISTS `sequencematch`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sequencematch` (
  `QueryWID` bigint(20) NOT NULL,
  `MatchWID` bigint(20) NOT NULL,
  `ComputationWID` bigint(20) NOT NULL,
  `EValue` double DEFAULT NULL,
  `PValue` double DEFAULT NULL,
  `PercentIdentical` float DEFAULT NULL,
  `PercentSimilar` float DEFAULT NULL,
  `Rank` smallint(6) DEFAULT NULL,
  `Length` int(11) DEFAULT NULL,
  `QueryStart` int(11) DEFAULT NULL,
  `QueryEnd` int(11) DEFAULT NULL,
  `MatchStart` int(11) DEFAULT NULL,
  `MatchEnd` int(11) DEFAULT NULL,
  KEY `FK_SequenceMatch` (`ComputationWID`),
  CONSTRAINT `FK_SequenceMatch` FOREIGN KEY (`ComputationWID`) REFERENCES `computation` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sequenceposition`
--

DROP TABLE IF EXISTS `sequenceposition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sequenceposition` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Start_` smallint(6) DEFAULT NULL,
  `End` smallint(6) DEFAULT NULL,
  `CompositeCompositeMap` bigint(20) DEFAULT NULL,
  `Composite` bigint(20) DEFAULT NULL,
  `ReporterCompositeMap` bigint(20) DEFAULT NULL,
  `Reporter` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_SequencePosition1` (`DataSetWID`),
  KEY `FK_SequencePosition2` (`CompositeCompositeMap`),
  KEY `FK_SequencePosition3` (`Composite`),
  KEY `FK_SequencePosition4` (`ReporterCompositeMap`),
  KEY `FK_SequencePosition5` (`Reporter`),
  CONSTRAINT `FK_SequencePosition1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SequencePosition2` FOREIGN KEY (`CompositeCompositeMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SequencePosition3` FOREIGN KEY (`Composite`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SequencePosition4` FOREIGN KEY (`ReporterCompositeMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SequencePosition5` FOREIGN KEY (`Reporter`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `softwarewidcontactwid`
--

DROP TABLE IF EXISTS `softwarewidcontactwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `softwarewidcontactwid` (
  `SoftwareWID` bigint(20) NOT NULL,
  `ContactWID` bigint(20) NOT NULL,
  KEY `FK_SoftwareWIDContactWID1` (`SoftwareWID`),
  KEY `FK_SoftwareWIDContactWID2` (`ContactWID`),
  CONSTRAINT `FK_SoftwareWIDContactWID1` FOREIGN KEY (`SoftwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SoftwareWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `softwarewidsoftwarewid`
--

DROP TABLE IF EXISTS `softwarewidsoftwarewid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `softwarewidsoftwarewid` (
  `SoftwareWID1` bigint(20) NOT NULL,
  `SoftwareWID2` bigint(20) NOT NULL,
  KEY `FK_SoftwareWIDSoftwareWID1` (`SoftwareWID1`),
  KEY `FK_SoftwareWIDSoftwareWID2` (`SoftwareWID2`),
  CONSTRAINT `FK_SoftwareWIDSoftwareWID1` FOREIGN KEY (`SoftwareWID1`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SoftwareWIDSoftwareWID2` FOREIGN KEY (`SoftwareWID2`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `specialwidtable`
--

DROP TABLE IF EXISTS `specialwidtable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `specialwidtable` (
  `PreviousWID` bigint(20) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`PreviousWID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `spot`
--

DROP TABLE IF EXISTS `spot`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `spot` (
  `WID` bigint(20) NOT NULL,
  `SpotId` varchar(25) DEFAULT NULL,
  `MolecularWeightEst` float DEFAULT NULL,
  `PIEst` varchar(50) DEFAULT NULL,
  `DatasetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Spot` (`DatasetWID`),
  CONSTRAINT `FK_Spot` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `spotidmethod`
--

DROP TABLE IF EXISTS `spotidmethod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `spotidmethod` (
  `WID` bigint(20) NOT NULL,
  `MethodName` varchar(100) NOT NULL,
  `MethodDesc` varchar(500) DEFAULT NULL,
  `MethodAbbrev` varchar(10) DEFAULT NULL,
  `DatasetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_SpotIdMethDataset` (`DatasetWID`),
  CONSTRAINT `FK_SpotIdMethDataset` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `spotwidspotidmethodwid`
--

DROP TABLE IF EXISTS `spotwidspotidmethodwid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `spotwidspotidmethodwid` (
  `SpotWID` bigint(20) NOT NULL,
  `SpotIdMethodWID` bigint(20) NOT NULL,
  KEY `FK_SpotWIDMethWID1` (`SpotWID`),
  KEY `FK_SpotWIDMethWID2` (`SpotIdMethodWID`),
  CONSTRAINT `FK_SpotWIDMethWID1` FOREIGN KEY (`SpotWID`) REFERENCES `spot` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SpotWIDMethWID2` FOREIGN KEY (`SpotIdMethodWID`) REFERENCES `spotidmethod` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `subsequence`
--

DROP TABLE IF EXISTS `subsequence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `subsequence` (
  `WID` bigint(20) NOT NULL,
  `NucleicAcidWID` bigint(20) NOT NULL,
  `FullSequence` char(1) DEFAULT NULL,
  `Sequence` longtext,
  `Length` int(11) DEFAULT NULL,
  `LengthApproximate` varchar(10) DEFAULT NULL,
  `PercentGC` float DEFAULT NULL,
  `Version` varchar(30) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Subsequence1` (`NucleicAcidWID`),
  KEY `FK_Subsequence2` (`DataSetWID`),
  CONSTRAINT `FK_Subsequence1` FOREIGN KEY (`NucleicAcidWID`) REFERENCES `nucleicacid` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Subsequence2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `subunit`
--

DROP TABLE IF EXISTS `subunit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `subunit` (
  `ComplexWID` bigint(20) NOT NULL,
  `SubunitWID` bigint(20) NOT NULL,
  `Coefficient` smallint(6) DEFAULT NULL,
  KEY `FK_Subunit1` (`ComplexWID`),
  KEY `FK_Subunit2` (`SubunitWID`),
  CONSTRAINT `FK_Subunit1` FOREIGN KEY (`ComplexWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Subunit2` FOREIGN KEY (`SubunitWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `superpathway`
--

DROP TABLE IF EXISTS `superpathway`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `superpathway` (
  `SubPathwayWID` bigint(20) NOT NULL,
  `SuperPathwayWID` bigint(20) NOT NULL,
  KEY `FK_SuperPathway1` (`SubPathwayWID`),
  KEY `FK_SuperPathway2` (`SuperPathwayWID`),
  CONSTRAINT `FK_SuperPathway1` FOREIGN KEY (`SubPathwayWID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SuperPathway2` FOREIGN KEY (`SuperPathwayWID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `support`
--

DROP TABLE IF EXISTS `support`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `support` (
  `WID` bigint(20) NOT NULL,
  `OtherWID` bigint(20) NOT NULL,
  `Type` varchar(100) DEFAULT NULL,
  `EvidenceType` varchar(100) DEFAULT NULL,
  `Confidence` float DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `synonymtable`
--

DROP TABLE IF EXISTS `synonymtable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `synonymtable` (
  `OtherWID` bigint(20) NOT NULL,
  `Syn` varchar(255) NOT NULL,
  KEY `SYNONYM_OTHERWID_SYN` (`OtherWID`,`Syn`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `taxon`
--

DROP TABLE IF EXISTS `taxon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxon` (
  `WID` bigint(20) NOT NULL,
  `ParentWID` bigint(20) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `Rank` varchar(100) DEFAULT NULL,
  `DivisionWID` bigint(20) DEFAULT NULL,
  `InheritedDivision` char(1) DEFAULT NULL,
  `GencodeWID` bigint(20) DEFAULT NULL,
  `InheritedGencode` char(1) DEFAULT NULL,
  `MCGencodeWID` bigint(20) DEFAULT NULL,
  `InheritedMCGencode` char(1) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Taxon_Division` (`DivisionWID`),
  KEY `FK_Taxon_GeneticCode` (`GencodeWID`),
  KEY `FK_Taxon` (`DataSetWID`),
  CONSTRAINT `FK_Taxon` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Taxon_Division` FOREIGN KEY (`DivisionWID`) REFERENCES `division` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Taxon_GeneticCode` FOREIGN KEY (`GencodeWID`) REFERENCES `geneticcode` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term`
--

DROP TABLE IF EXISTS `term`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Definition` text,
  `Hierarchical` char(1) DEFAULT NULL,
  `Root` char(1) DEFAULT NULL,
  `Obsolete` char(1) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `termrelationship`
--

DROP TABLE IF EXISTS `termrelationship`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `termrelationship` (
  `TermWID` bigint(20) NOT NULL,
  `RelatedTermWID` bigint(20) NOT NULL,
  `Relationship` varchar(10) NOT NULL,
  KEY `FK_TermRelationship1` (`TermWID`),
  KEY `FK_TermRelationship2` (`RelatedTermWID`),
  CONSTRAINT `FK_TermRelationship1` FOREIGN KEY (`TermWID`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_TermRelationship2` FOREIGN KEY (`RelatedTermWID`) REFERENCES `term` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tooladvice`
--

DROP TABLE IF EXISTS `tooladvice`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tooladvice` (
  `OtherWID` bigint(20) NOT NULL,
  `ToolName` varchar(50) NOT NULL,
  `Advice` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `transcriptionunit`
--

DROP TABLE IF EXISTS `transcriptionunit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transcriptionunit` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `TranscriptionUnit_DWID` (`DataSetWID`),
  CONSTRAINT `FK_TranscriptionUnit1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `transcriptionunitcomponent`
--

DROP TABLE IF EXISTS `transcriptionunitcomponent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transcriptionunitcomponent` (
  `Type` varchar(100) NOT NULL,
  `TranscriptionUnitWID` bigint(20) NOT NULL,
  `OtherWID` bigint(20) NOT NULL,
  KEY `FK_TranscriptionUnitComponent1` (`TranscriptionUnitWID`),
  CONSTRAINT `FK_TranscriptionUnitComponent1` FOREIGN KEY (`TranscriptionUnitWID`) REFERENCES `transcriptionunit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `transformwidbioassaydatawid`
--

DROP TABLE IF EXISTS `transformwidbioassaydatawid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transformwidbioassaydatawid` (
  `TransformationWID` bigint(20) NOT NULL,
  `BioAssayDataWID` bigint(20) NOT NULL,
  KEY `FK_TransformWIDBioAssayDataW1` (`TransformationWID`),
  KEY `FK_TransformWIDBioAssayDataW2` (`BioAssayDataWID`),
  CONSTRAINT `FK_TransformWIDBioAssayDataW1` FOREIGN KEY (`TransformationWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_TransformWIDBioAssayDataW2` FOREIGN KEY (`BioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `unit`
--

DROP TABLE IF EXISTS `unit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `unit` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `UnitName` varchar(255) DEFAULT NULL,
  `UnitNameCV` varchar(25) DEFAULT NULL,
  `UnitNameCV2` varchar(25) DEFAULT NULL,
  `UnitNameCV3` varchar(25) DEFAULT NULL,
  `UnitNameCV4` varchar(25) DEFAULT NULL,
  `UnitNameCV5` varchar(25) DEFAULT NULL,
  `UnitNameCV6` varchar(25) DEFAULT NULL,
  `UnitNameCV7` varchar(25) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Unit1` (`DataSetWID`),
  CONSTRAINT `FK_Unit1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `valence`
--

DROP TABLE IF EXISTS `valence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `valence` (
  `OtherWID` bigint(20) NOT NULL,
  `Valence` smallint(6) NOT NULL,
  KEY `FK_Valence` (`OtherWID`),
  CONSTRAINT `FK_Valence` FOREIGN KEY (`OtherWID`) REFERENCES `element` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `warehouse`
--

DROP TABLE IF EXISTS `warehouse`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `warehouse` (
  `Version` decimal(6,3) NOT NULL,
  `LoadDate` datetime NOT NULL,
  `MaxSpecialWID` bigint(20) NOT NULL,
  `MaxReservedWID` bigint(20) NOT NULL,
  `Description` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `widtable`
--

DROP TABLE IF EXISTS `widtable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `widtable` (
  `PreviousWID` bigint(20) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`PreviousWID`)
) ENGINE=InnoDB AUTO_INCREMENT=1000000 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zone`
--

DROP TABLE IF EXISTS `zone`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `zone` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Row_` smallint(6) DEFAULT NULL,
  `Column_` smallint(6) DEFAULT NULL,
  `UpperLeftX` float DEFAULT NULL,
  `UpperLeftY` float DEFAULT NULL,
  `LowerRightX` float DEFAULT NULL,
  `LowerRightY` float DEFAULT NULL,
  `Zone_DistanceUnit` bigint(20) DEFAULT NULL,
  `ZoneGroup_ZoneLocations` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Zone1` (`DataSetWID`),
  KEY `FK_Zone3` (`Zone_DistanceUnit`),
  KEY `FK_Zone4` (`ZoneGroup_ZoneLocations`),
  CONSTRAINT `FK_Zone1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Zone3` FOREIGN KEY (`Zone_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Zone4` FOREIGN KEY (`ZoneGroup_ZoneLocations`) REFERENCES `zonegroup` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zonedefect`
--

DROP TABLE IF EXISTS `zonedefect`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `zonedefect` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `ArrayManufactureDeviation` bigint(20) DEFAULT NULL,
  `ZoneDefect_DefectType` bigint(20) DEFAULT NULL,
  `ZoneDefect_PositionDelta` bigint(20) DEFAULT NULL,
  `Zone` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ZoneDefect1` (`DataSetWID`),
  KEY `FK_ZoneDefect2` (`ArrayManufactureDeviation`),
  KEY `FK_ZoneDefect3` (`ZoneDefect_DefectType`),
  KEY `FK_ZoneDefect4` (`ZoneDefect_PositionDelta`),
  KEY `FK_ZoneDefect5` (`Zone`),
  CONSTRAINT `FK_ZoneDefect1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneDefect2` FOREIGN KEY (`ArrayManufactureDeviation`) REFERENCES `arraymanufacturedeviation` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneDefect3` FOREIGN KEY (`ZoneDefect_DefectType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneDefect4` FOREIGN KEY (`ZoneDefect_PositionDelta`) REFERENCES `positiondelta` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneDefect5` FOREIGN KEY (`Zone`) REFERENCES `zone` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zonegroup`
--

DROP TABLE IF EXISTS `zonegroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `zonegroup` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `PhysicalArrayDesign_ZoneGroups` bigint(20) DEFAULT NULL,
  `SpacingsBetweenZonesX` float DEFAULT NULL,
  `SpacingsBetweenZonesY` float DEFAULT NULL,
  `ZonesPerX` smallint(6) DEFAULT NULL,
  `ZonesPerY` smallint(6) DEFAULT NULL,
  `ZoneGroup_DistanceUnit` bigint(20) DEFAULT NULL,
  `ZoneGroup_ZoneLayout` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ZoneGroup1` (`DataSetWID`),
  KEY `FK_ZoneGroup2` (`PhysicalArrayDesign_ZoneGroups`),
  KEY `FK_ZoneGroup3` (`ZoneGroup_DistanceUnit`),
  KEY `FK_ZoneGroup4` (`ZoneGroup_ZoneLayout`),
  CONSTRAINT `FK_ZoneGroup1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneGroup2` FOREIGN KEY (`PhysicalArrayDesign_ZoneGroups`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneGroup3` FOREIGN KEY (`ZoneGroup_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneGroup4` FOREIGN KEY (`ZoneGroup_ZoneLayout`) REFERENCES `zonelayout` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zonelayout`
--

DROP TABLE IF EXISTS `zonelayout`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `zonelayout` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `NumFeaturesPerRow` smallint(6) DEFAULT NULL,
  `NumFeaturesPerCol` smallint(6) DEFAULT NULL,
  `SpacingBetweenRows` float DEFAULT NULL,
  `SpacingBetweenCols` float DEFAULT NULL,
  `ZoneLayout_DistanceUnit` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ZoneLayout1` (`DataSetWID`),
  KEY `FK_ZoneLayout2` (`ZoneLayout_DistanceUnit`),
  CONSTRAINT `FK_ZoneLayout1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneLayout2` FOREIGN KEY (`ZoneLayout_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-12-03 20:02:01

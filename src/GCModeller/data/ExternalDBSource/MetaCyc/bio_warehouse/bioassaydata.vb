#Region "Microsoft.VisualBasic::ef86df42f2c3bd6106b367bcdc9442bd, data\ExternalDBSource\MetaCyc\bio_warehouse\bioassaydata.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class bioassaydata
    ' 
    '     Properties: BioAssayData_BioDataValues, BioAssayDimension, DataSetWID, DesignElementDimension, Identifier
    '                 MAGEClass, Name, ProducerTransformation, QuantitationTypeDimension, WID
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:02:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `bioassaydata`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `bioassaydata` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `BioAssayDimension` bigint(20) DEFAULT NULL,
'''   `DesignElementDimension` bigint(20) DEFAULT NULL,
'''   `QuantitationTypeDimension` bigint(20) DEFAULT NULL,
'''   `BioAssayData_BioDataValues` bigint(20) DEFAULT NULL,
'''   `ProducerTransformation` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_BioAssayData1` (`DataSetWID`),
'''   KEY `FK_BioAssayData3` (`BioAssayDimension`),
'''   KEY `FK_BioAssayData4` (`DesignElementDimension`),
'''   KEY `FK_BioAssayData5` (`QuantitationTypeDimension`),
'''   KEY `FK_BioAssayData6` (`BioAssayData_BioDataValues`),
'''   KEY `FK_BioAssayData7` (`ProducerTransformation`),
'''   CONSTRAINT `FK_BioAssayData1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioAssayData3` FOREIGN KEY (`BioAssayDimension`) REFERENCES `bioassaydimension` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioAssayData4` FOREIGN KEY (`DesignElementDimension`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioAssayData5` FOREIGN KEY (`QuantitationTypeDimension`) REFERENCES `quantitationtypedimension` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioAssayData6` FOREIGN KEY (`BioAssayData_BioDataValues`) REFERENCES `biodatavalues` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioAssayData7` FOREIGN KEY (`ProducerTransformation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("bioassaydata", Database:="warehouse")>
Public Class bioassaydata: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property MAGEClass As String
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("BioAssayDimension"), DataType(MySqlDbType.Int64, "20")> Public Property BioAssayDimension As Long
    <DatabaseField("DesignElementDimension"), DataType(MySqlDbType.Int64, "20")> Public Property DesignElementDimension As Long
    <DatabaseField("QuantitationTypeDimension"), DataType(MySqlDbType.Int64, "20")> Public Property QuantitationTypeDimension As Long
    <DatabaseField("BioAssayData_BioDataValues"), DataType(MySqlDbType.Int64, "20")> Public Property BioAssayData_BioDataValues As Long
    <DatabaseField("ProducerTransformation"), DataType(MySqlDbType.Int64, "20")> Public Property ProducerTransformation As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `bioassaydata` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `BioAssayDimension`, `DesignElementDimension`, `QuantitationTypeDimension`, `BioAssayData_BioDataValues`, `ProducerTransformation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `bioassaydata` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `BioAssayDimension`, `DesignElementDimension`, `QuantitationTypeDimension`, `BioAssayData_BioDataValues`, `ProducerTransformation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `bioassaydata` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `bioassaydata` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `BioAssayDimension`='{5}', `DesignElementDimension`='{6}', `QuantitationTypeDimension`='{7}', `BioAssayData_BioDataValues`='{8}', `ProducerTransformation`='{9}' WHERE `WID` = '{10}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, BioAssayDimension, DesignElementDimension, QuantitationTypeDimension, BioAssayData_BioDataValues, ProducerTransformation)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, BioAssayDimension, DesignElementDimension, QuantitationTypeDimension, BioAssayData_BioDataValues, ProducerTransformation)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, BioAssayDimension, DesignElementDimension, QuantitationTypeDimension, BioAssayData_BioDataValues, ProducerTransformation, WID)
    End Function
#End Region
End Class


End Namespace

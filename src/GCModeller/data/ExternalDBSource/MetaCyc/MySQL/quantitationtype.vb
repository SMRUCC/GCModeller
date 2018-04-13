#Region "Microsoft.VisualBasic::9f9950b3818d6f94716cccdf02d7ff87, data\ExternalDBSource\MetaCyc\MySQL\quantitationtype.vb"

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

    ' Class quantitationtype
    ' 
    '     Properties: Channel, DataSetWID, Identifier, IsBackground, MAGEClass
    '                 Name, QuantitationType_DataType, QuantitationType_Scale, TargetQuantitationType, WID
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:19 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `quantitationtype`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `quantitationtype` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `IsBackground` char(1) DEFAULT NULL,
'''   `Channel` bigint(20) DEFAULT NULL,
'''   `QuantitationType_Scale` bigint(20) DEFAULT NULL,
'''   `QuantitationType_DataType` bigint(20) DEFAULT NULL,
'''   `TargetQuantitationType` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_QuantitationType1` (`DataSetWID`),
'''   KEY `FK_QuantitationType3` (`Channel`),
'''   KEY `FK_QuantitationType4` (`QuantitationType_Scale`),
'''   KEY `FK_QuantitationType5` (`QuantitationType_DataType`),
'''   KEY `FK_QuantitationType6` (`TargetQuantitationType`),
'''   CONSTRAINT `FK_QuantitationType1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_QuantitationType3` FOREIGN KEY (`Channel`) REFERENCES `channel` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_QuantitationType4` FOREIGN KEY (`QuantitationType_Scale`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_QuantitationType5` FOREIGN KEY (`QuantitationType_DataType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_QuantitationType6` FOREIGN KEY (`TargetQuantitationType`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("quantitationtype", Database:="warehouse", SchemaSQL:="
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class quantitationtype: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="MAGEClass")> Public Property MAGEClass As String
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Identifier")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("IsBackground"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="IsBackground")> Public Property IsBackground As String
    <DatabaseField("Channel"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Channel")> Public Property Channel As Long
    <DatabaseField("QuantitationType_Scale"), DataType(MySqlDbType.Int64, "20"), Column(Name:="QuantitationType_Scale")> Public Property QuantitationType_Scale As Long
    <DatabaseField("QuantitationType_DataType"), DataType(MySqlDbType.Int64, "20"), Column(Name:="QuantitationType_DataType")> Public Property QuantitationType_DataType As Long
    <DatabaseField("TargetQuantitationType"), DataType(MySqlDbType.Int64, "20"), Column(Name:="TargetQuantitationType")> Public Property TargetQuantitationType As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `quantitationtype` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `IsBackground`, `Channel`, `QuantitationType_Scale`, `QuantitationType_DataType`, `TargetQuantitationType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `quantitationtype` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `IsBackground`, `Channel`, `QuantitationType_Scale`, `QuantitationType_DataType`, `TargetQuantitationType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `quantitationtype` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `quantitationtype` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `IsBackground`='{5}', `Channel`='{6}', `QuantitationType_Scale`='{7}', `QuantitationType_DataType`='{8}', `TargetQuantitationType`='{9}' WHERE `WID` = '{10}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `quantitationtype` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `quantitationtype` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `IsBackground`, `Channel`, `QuantitationType_Scale`, `QuantitationType_DataType`, `TargetQuantitationType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, IsBackground, Channel, QuantitationType_Scale, QuantitationType_DataType, TargetQuantitationType)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{Identifier}', '{Name}', '{IsBackground}', '{Channel}', '{QuantitationType_Scale}', '{QuantitationType_DataType}', '{TargetQuantitationType}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `quantitationtype` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `IsBackground`, `Channel`, `QuantitationType_Scale`, `QuantitationType_DataType`, `TargetQuantitationType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, IsBackground, Channel, QuantitationType_Scale, QuantitationType_DataType, TargetQuantitationType)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `quantitationtype` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `IsBackground`='{5}', `Channel`='{6}', `QuantitationType_Scale`='{7}', `QuantitationType_DataType`='{8}', `TargetQuantitationType`='{9}' WHERE `WID` = '{10}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, IsBackground, Channel, QuantitationType_Scale, QuantitationType_DataType, TargetQuantitationType, WID)
    End Function
#End Region
Public Function Clone() As quantitationtype
                  Return DirectCast(MyClass.MemberwiseClone, quantitationtype)
              End Function
End Class


End Namespace

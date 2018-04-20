#Region "Microsoft.VisualBasic::a97eedb23414f6c61371d15090df8152, data\ExternalDBSource\MetaCyc\MySQL\image.vb"

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

    ' Class image
    ' 
    '     Properties: DataSetWID, Identifier, Image_Format, Name, PhysicalBioAssay
    '                 URI, WID
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
''' DROP TABLE IF EXISTS `image`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `image` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `URI` varchar(255) DEFAULT NULL,
'''   `Image_Format` bigint(20) DEFAULT NULL,
'''   `PhysicalBioAssay` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Image1` (`DataSetWID`),
'''   KEY `FK_Image3` (`Image_Format`),
'''   KEY `FK_Image4` (`PhysicalBioAssay`),
'''   CONSTRAINT `FK_Image1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Image3` FOREIGN KEY (`Image_Format`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Image4` FOREIGN KEY (`PhysicalBioAssay`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("image", Database:="warehouse", SchemaSQL:="
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class image: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Identifier")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("URI"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="URI")> Public Property URI As String
    <DatabaseField("Image_Format"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Image_Format")> Public Property Image_Format As Long
    <DatabaseField("PhysicalBioAssay"), DataType(MySqlDbType.Int64, "20"), Column(Name:="PhysicalBioAssay")> Public Property PhysicalBioAssay As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `image` (`WID`, `DataSetWID`, `Identifier`, `Name`, `URI`, `Image_Format`, `PhysicalBioAssay`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `image` (`WID`, `DataSetWID`, `Identifier`, `Name`, `URI`, `Image_Format`, `PhysicalBioAssay`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `image` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `image` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `URI`='{4}', `Image_Format`='{5}', `PhysicalBioAssay`='{6}' WHERE `WID` = '{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `image` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `image` (`WID`, `DataSetWID`, `Identifier`, `Name`, `URI`, `Image_Format`, `PhysicalBioAssay`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, URI, Image_Format, PhysicalBioAssay)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{DataSetWID}', '{Identifier}', '{Name}', '{URI}', '{Image_Format}', '{PhysicalBioAssay}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `image` (`WID`, `DataSetWID`, `Identifier`, `Name`, `URI`, `Image_Format`, `PhysicalBioAssay`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, URI, Image_Format, PhysicalBioAssay)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `image` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `URI`='{4}', `Image_Format`='{5}', `PhysicalBioAssay`='{6}' WHERE `WID` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Identifier, Name, URI, Image_Format, PhysicalBioAssay, WID)
    End Function
#End Region
Public Function Clone() As image
                  Return DirectCast(MyClass.MemberwiseClone, image)
              End Function
End Class


End Namespace

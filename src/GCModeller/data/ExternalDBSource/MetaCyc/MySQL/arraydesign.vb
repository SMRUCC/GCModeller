#Region "Microsoft.VisualBasic::f283b4c097cd37a59ccd2b05d89e75d2, data\ExternalDBSource\MetaCyc\MySQL\arraydesign.vb"

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

    ' Class arraydesign
    ' 
    '     Properties: DataSetWID, Identifier, MAGEClass, Name, NumberOfFeatures
    '                 SurfaceType, Version, WID
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:40


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
''' DROP TABLE IF EXISTS `arraydesign`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `arraydesign` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `Version` varchar(255) DEFAULT NULL,
'''   `NumberOfFeatures` smallint(6) DEFAULT NULL,
'''   `SurfaceType` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_ArrayDesign1` (`DataSetWID`),
'''   KEY `FK_ArrayDesign3` (`SurfaceType`),
'''   CONSTRAINT `FK_ArrayDesign1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ArrayDesign3` FOREIGN KEY (`SurfaceType`) REFERENCES `term` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("arraydesign", Database:="warehouse", SchemaSQL:="
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class arraydesign: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="MAGEClass")> Public Property MAGEClass As String
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Identifier")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("Version"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Version")> Public Property Version As String
    <DatabaseField("NumberOfFeatures"), DataType(MySqlDbType.Int64, "6"), Column(Name:="NumberOfFeatures")> Public Property NumberOfFeatures As Long
    <DatabaseField("SurfaceType"), DataType(MySqlDbType.Int64, "20"), Column(Name:="SurfaceType")> Public Property SurfaceType As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `arraydesign` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `Version`, `NumberOfFeatures`, `SurfaceType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `arraydesign` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `Version`, `NumberOfFeatures`, `SurfaceType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `arraydesign` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `Version`, `NumberOfFeatures`, `SurfaceType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `arraydesign` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `Version`, `NumberOfFeatures`, `SurfaceType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `arraydesign` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `arraydesign` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `Version`='{5}', `NumberOfFeatures`='{6}', `SurfaceType`='{7}' WHERE `WID` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `arraydesign` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `arraydesign` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `Version`, `NumberOfFeatures`, `SurfaceType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, Version, NumberOfFeatures, SurfaceType)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `arraydesign` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `Version`, `NumberOfFeatures`, `SurfaceType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, Version, NumberOfFeatures, SurfaceType)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, Version, NumberOfFeatures, SurfaceType)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{Identifier}', '{Name}', '{Version}', '{NumberOfFeatures}', '{SurfaceType}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{Identifier}', '{Name}', '{Version}', '{NumberOfFeatures}', '{SurfaceType}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `arraydesign` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `Version`, `NumberOfFeatures`, `SurfaceType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, Version, NumberOfFeatures, SurfaceType)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `arraydesign` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `Version`, `NumberOfFeatures`, `SurfaceType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, Version, NumberOfFeatures, SurfaceType)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, Version, NumberOfFeatures, SurfaceType)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `arraydesign` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `Version`='{5}', `NumberOfFeatures`='{6}', `SurfaceType`='{7}' WHERE `WID` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, Version, NumberOfFeatures, SurfaceType, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As arraydesign
                         Return DirectCast(MyClass.MemberwiseClone, arraydesign)
                     End Function
End Class


End Namespace

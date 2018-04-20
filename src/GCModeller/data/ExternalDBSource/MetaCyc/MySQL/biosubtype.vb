#Region "Microsoft.VisualBasic::ba62e5a62465401d5062172209036d43, data\ExternalDBSource\MetaCyc\MySQL\biosubtype.vb"

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

    ' Class biosubtype
    ' 
    '     Properties: DataSetWID, Type, Value, WID
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
''' DROP TABLE IF EXISTS `biosubtype`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `biosubtype` (
'''   `WID` bigint(20) NOT NULL,
'''   `Type` varchar(25) DEFAULT NULL,
'''   `Value` varchar(50) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_BioSubtype2` (`DataSetWID`),
'''   CONSTRAINT `FK_BioSubtype2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("biosubtype", Database:="warehouse", SchemaSQL:="
CREATE TABLE `biosubtype` (
  `WID` bigint(20) NOT NULL,
  `Type` varchar(25) DEFAULT NULL,
  `Value` varchar(50) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioSubtype2` (`DataSetWID`),
  CONSTRAINT `FK_BioSubtype2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class biosubtype: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("Type"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="Type")> Public Property Type As String
    <DatabaseField("Value"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="Value")> Public Property Value As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `biosubtype` (`WID`, `Type`, `Value`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `biosubtype` (`WID`, `Type`, `Value`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `biosubtype` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `biosubtype` SET `WID`='{0}', `Type`='{1}', `Value`='{2}', `DataSetWID`='{3}' WHERE `WID` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `biosubtype` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `biosubtype` (`WID`, `Type`, `Value`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Type, Value, DataSetWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{Type}', '{Value}', '{DataSetWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `biosubtype` (`WID`, `Type`, `Value`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Type, Value, DataSetWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `biosubtype` SET `WID`='{0}', `Type`='{1}', `Value`='{2}', `DataSetWID`='{3}' WHERE `WID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Type, Value, DataSetWID, WID)
    End Function
#End Region
Public Function Clone() As biosubtype
                  Return DirectCast(MyClass.MemberwiseClone, biosubtype)
              End Function
End Class


End Namespace

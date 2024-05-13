#Region "Microsoft.VisualBasic::6e0b9f78698a989a05f158768f46d060, data\Reactome\LocalMySQL\gk_current\physicalentity.vb"

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


    ' Code Statistics:

    '   Total Lines: 182
    '    Code Lines: 93
    ' Comment Lines: 67
    '   Blank Lines: 22
    '     File Size: 10.14 KB


    ' Class physicalentity
    ' 
    '     Properties: authored, authored_class, cellType, cellType_class, DB_ID
    '                 definition, goCellularComponent, goCellularComponent_class, systematicName
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

REM  Dump @2018/5/23 13:13:41


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `physicalentity`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `physicalentity` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `authored` int(10) unsigned DEFAULT NULL,
'''   `authored_class` varchar(64) DEFAULT NULL,
'''   `cellType` int(10) unsigned DEFAULT NULL,
'''   `cellType_class` varchar(64) DEFAULT NULL,
'''   `definition` text,
'''   `goCellularComponent` int(10) unsigned DEFAULT NULL,
'''   `goCellularComponent_class` varchar(64) DEFAULT NULL,
'''   `systematicName` text,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `authored` (`authored`),
'''   KEY `cellType` (`cellType`),
'''   KEY `goCellularComponent` (`goCellularComponent`),
'''   FULLTEXT KEY `definition` (`definition`),
'''   FULLTEXT KEY `systematicName` (`systematicName`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("physicalentity", Database:="gk_current", SchemaSQL:="
CREATE TABLE `physicalentity` (
  `DB_ID` int(10) unsigned NOT NULL,
  `authored` int(10) unsigned DEFAULT NULL,
  `authored_class` varchar(64) DEFAULT NULL,
  `cellType` int(10) unsigned DEFAULT NULL,
  `cellType_class` varchar(64) DEFAULT NULL,
  `definition` text,
  `goCellularComponent` int(10) unsigned DEFAULT NULL,
  `goCellularComponent_class` varchar(64) DEFAULT NULL,
  `systematicName` text,
  PRIMARY KEY (`DB_ID`),
  KEY `authored` (`authored`),
  KEY `cellType` (`cellType`),
  KEY `goCellularComponent` (`goCellularComponent`),
  FULLTEXT KEY `definition` (`definition`),
  FULLTEXT KEY `systematicName` (`systematicName`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class physicalentity: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("authored"), DataType(MySqlDbType.Int64, "10"), Column(Name:="authored")> Public Property authored As Long
    <DatabaseField("authored_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="authored_class")> Public Property authored_class As String
    <DatabaseField("cellType"), DataType(MySqlDbType.Int64, "10"), Column(Name:="cellType")> Public Property cellType As Long
    <DatabaseField("cellType_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="cellType_class")> Public Property cellType_class As String
    <DatabaseField("definition"), DataType(MySqlDbType.Text), Column(Name:="definition")> Public Property definition As String
    <DatabaseField("goCellularComponent"), DataType(MySqlDbType.Int64, "10"), Column(Name:="goCellularComponent")> Public Property goCellularComponent As Long
    <DatabaseField("goCellularComponent_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="goCellularComponent_class")> Public Property goCellularComponent_class As String
    <DatabaseField("systematicName"), DataType(MySqlDbType.Text), Column(Name:="systematicName")> Public Property systematicName As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `physicalentity` (`DB_ID`, `authored`, `authored_class`, `cellType`, `cellType_class`, `definition`, `goCellularComponent`, `goCellularComponent_class`, `systematicName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `physicalentity` (`DB_ID`, `authored`, `authored_class`, `cellType`, `cellType_class`, `definition`, `goCellularComponent`, `goCellularComponent_class`, `systematicName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `physicalentity` (`DB_ID`, `authored`, `authored_class`, `cellType`, `cellType_class`, `definition`, `goCellularComponent`, `goCellularComponent_class`, `systematicName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `physicalentity` (`DB_ID`, `authored`, `authored_class`, `cellType`, `cellType_class`, `definition`, `goCellularComponent`, `goCellularComponent_class`, `systematicName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `physicalentity` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `physicalentity` SET `DB_ID`='{0}', `authored`='{1}', `authored_class`='{2}', `cellType`='{3}', `cellType_class`='{4}', `definition`='{5}', `goCellularComponent`='{6}', `goCellularComponent_class`='{7}', `systematicName`='{8}' WHERE `DB_ID` = '{9}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `physicalentity` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `physicalentity` (`DB_ID`, `authored`, `authored_class`, `cellType`, `cellType_class`, `definition`, `goCellularComponent`, `goCellularComponent_class`, `systematicName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, authored, authored_class, cellType, cellType_class, definition, goCellularComponent, goCellularComponent_class, systematicName)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `physicalentity` (`DB_ID`, `authored`, `authored_class`, `cellType`, `cellType_class`, `definition`, `goCellularComponent`, `goCellularComponent_class`, `systematicName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, authored, authored_class, cellType, cellType_class, definition, goCellularComponent, goCellularComponent_class, systematicName)
        Else
        Return String.Format(INSERT_SQL, DB_ID, authored, authored_class, cellType, cellType_class, definition, goCellularComponent, goCellularComponent_class, systematicName)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{authored}', '{authored_class}', '{cellType}', '{cellType_class}', '{definition}', '{goCellularComponent}', '{goCellularComponent_class}', '{systematicName}')"
        Else
            Return $"('{DB_ID}', '{authored}', '{authored_class}', '{cellType}', '{cellType_class}', '{definition}', '{goCellularComponent}', '{goCellularComponent_class}', '{systematicName}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `physicalentity` (`DB_ID`, `authored`, `authored_class`, `cellType`, `cellType_class`, `definition`, `goCellularComponent`, `goCellularComponent_class`, `systematicName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, authored, authored_class, cellType, cellType_class, definition, goCellularComponent, goCellularComponent_class, systematicName)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `physicalentity` (`DB_ID`, `authored`, `authored_class`, `cellType`, `cellType_class`, `definition`, `goCellularComponent`, `goCellularComponent_class`, `systematicName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, authored, authored_class, cellType, cellType_class, definition, goCellularComponent, goCellularComponent_class, systematicName)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, authored, authored_class, cellType, cellType_class, definition, goCellularComponent, goCellularComponent_class, systematicName)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `physicalentity` SET `DB_ID`='{0}', `authored`='{1}', `authored_class`='{2}', `cellType`='{3}', `cellType_class`='{4}', `definition`='{5}', `goCellularComponent`='{6}', `goCellularComponent_class`='{7}', `systematicName`='{8}' WHERE `DB_ID` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, authored, authored_class, cellType, cellType_class, definition, goCellularComponent, goCellularComponent_class, systematicName, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As physicalentity
                         Return DirectCast(MyClass.MemberwiseClone, physicalentity)
                     End Function
End Class


End Namespace

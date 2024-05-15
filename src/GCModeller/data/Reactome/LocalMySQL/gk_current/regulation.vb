#Region "Microsoft.VisualBasic::140586f7de1f73f3d3351f970cfcede3, data\Reactome\LocalMySQL\gk_current\regulation.vb"

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

    '   Total Lines: 185
    '    Code Lines: 95
    ' Comment Lines: 68
    '   Blank Lines: 22
    '     File Size: 11.02 KB


    ' Class regulation
    ' 
    '     Properties: authored, authored_class, DB_ID, regulatedEntity, regulatedEntity_class
    '                 regulationType, regulationType_class, regulator, regulator_class, releaseDate
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
''' DROP TABLE IF EXISTS `regulation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `regulation` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `authored` int(10) unsigned DEFAULT NULL,
'''   `authored_class` varchar(64) DEFAULT NULL,
'''   `regulatedEntity` int(10) unsigned DEFAULT NULL,
'''   `regulatedEntity_class` varchar(64) DEFAULT NULL,
'''   `regulationType` int(10) unsigned DEFAULT NULL,
'''   `regulationType_class` varchar(64) DEFAULT NULL,
'''   `regulator` int(10) unsigned DEFAULT NULL,
'''   `regulator_class` varchar(64) DEFAULT NULL,
'''   `releaseDate` date DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `authored` (`authored`),
'''   KEY `regulatedEntity` (`regulatedEntity`),
'''   KEY `regulationType` (`regulationType`),
'''   KEY `regulator` (`regulator`),
'''   KEY `releaseDate` (`releaseDate`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("regulation", Database:="gk_current", SchemaSQL:="
CREATE TABLE `regulation` (
  `DB_ID` int(10) unsigned NOT NULL,
  `authored` int(10) unsigned DEFAULT NULL,
  `authored_class` varchar(64) DEFAULT NULL,
  `regulatedEntity` int(10) unsigned DEFAULT NULL,
  `regulatedEntity_class` varchar(64) DEFAULT NULL,
  `regulationType` int(10) unsigned DEFAULT NULL,
  `regulationType_class` varchar(64) DEFAULT NULL,
  `regulator` int(10) unsigned DEFAULT NULL,
  `regulator_class` varchar(64) DEFAULT NULL,
  `releaseDate` date DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `authored` (`authored`),
  KEY `regulatedEntity` (`regulatedEntity`),
  KEY `regulationType` (`regulationType`),
  KEY `regulator` (`regulator`),
  KEY `releaseDate` (`releaseDate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class regulation: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("authored"), DataType(MySqlDbType.Int64, "10"), Column(Name:="authored")> Public Property authored As Long
    <DatabaseField("authored_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="authored_class")> Public Property authored_class As String
    <DatabaseField("regulatedEntity"), DataType(MySqlDbType.Int64, "10"), Column(Name:="regulatedEntity")> Public Property regulatedEntity As Long
    <DatabaseField("regulatedEntity_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="regulatedEntity_class")> Public Property regulatedEntity_class As String
    <DatabaseField("regulationType"), DataType(MySqlDbType.Int64, "10"), Column(Name:="regulationType")> Public Property regulationType As Long
    <DatabaseField("regulationType_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="regulationType_class")> Public Property regulationType_class As String
    <DatabaseField("regulator"), DataType(MySqlDbType.Int64, "10"), Column(Name:="regulator")> Public Property regulator As Long
    <DatabaseField("regulator_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="regulator_class")> Public Property regulator_class As String
    <DatabaseField("releaseDate"), DataType(MySqlDbType.DateTime), Column(Name:="releaseDate")> Public Property releaseDate As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `regulation` (`DB_ID`, `authored`, `authored_class`, `regulatedEntity`, `regulatedEntity_class`, `regulationType`, `regulationType_class`, `regulator`, `regulator_class`, `releaseDate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `regulation` (`DB_ID`, `authored`, `authored_class`, `regulatedEntity`, `regulatedEntity_class`, `regulationType`, `regulationType_class`, `regulator`, `regulator_class`, `releaseDate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `regulation` (`DB_ID`, `authored`, `authored_class`, `regulatedEntity`, `regulatedEntity_class`, `regulationType`, `regulationType_class`, `regulator`, `regulator_class`, `releaseDate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `regulation` (`DB_ID`, `authored`, `authored_class`, `regulatedEntity`, `regulatedEntity_class`, `regulationType`, `regulationType_class`, `regulator`, `regulator_class`, `releaseDate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `regulation` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `regulation` SET `DB_ID`='{0}', `authored`='{1}', `authored_class`='{2}', `regulatedEntity`='{3}', `regulatedEntity_class`='{4}', `regulationType`='{5}', `regulationType_class`='{6}', `regulator`='{7}', `regulator_class`='{8}', `releaseDate`='{9}' WHERE `DB_ID` = '{10}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `regulation` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `regulation` (`DB_ID`, `authored`, `authored_class`, `regulatedEntity`, `regulatedEntity_class`, `regulationType`, `regulationType_class`, `regulator`, `regulator_class`, `releaseDate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, authored, authored_class, regulatedEntity, regulatedEntity_class, regulationType, regulationType_class, regulator, regulator_class, MySqlScript.ToMySqlDateTimeString(releaseDate))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `regulation` (`DB_ID`, `authored`, `authored_class`, `regulatedEntity`, `regulatedEntity_class`, `regulationType`, `regulationType_class`, `regulator`, `regulator_class`, `releaseDate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, authored, authored_class, regulatedEntity, regulatedEntity_class, regulationType, regulationType_class, regulator, regulator_class, MySqlScript.ToMySqlDateTimeString(releaseDate))
        Else
        Return String.Format(INSERT_SQL, DB_ID, authored, authored_class, regulatedEntity, regulatedEntity_class, regulationType, regulationType_class, regulator, regulator_class, MySqlScript.ToMySqlDateTimeString(releaseDate))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{authored}', '{authored_class}', '{regulatedEntity}', '{regulatedEntity_class}', '{regulationType}', '{regulationType_class}', '{regulator}', '{regulator_class}', '{releaseDate}')"
        Else
            Return $"('{DB_ID}', '{authored}', '{authored_class}', '{regulatedEntity}', '{regulatedEntity_class}', '{regulationType}', '{regulationType_class}', '{regulator}', '{regulator_class}', '{releaseDate}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `regulation` (`DB_ID`, `authored`, `authored_class`, `regulatedEntity`, `regulatedEntity_class`, `regulationType`, `regulationType_class`, `regulator`, `regulator_class`, `releaseDate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, authored, authored_class, regulatedEntity, regulatedEntity_class, regulationType, regulationType_class, regulator, regulator_class, MySqlScript.ToMySqlDateTimeString(releaseDate))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `regulation` (`DB_ID`, `authored`, `authored_class`, `regulatedEntity`, `regulatedEntity_class`, `regulationType`, `regulationType_class`, `regulator`, `regulator_class`, `releaseDate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, authored, authored_class, regulatedEntity, regulatedEntity_class, regulationType, regulationType_class, regulator, regulator_class, MySqlScript.ToMySqlDateTimeString(releaseDate))
        Else
        Return String.Format(REPLACE_SQL, DB_ID, authored, authored_class, regulatedEntity, regulatedEntity_class, regulationType, regulationType_class, regulator, regulator_class, MySqlScript.ToMySqlDateTimeString(releaseDate))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `regulation` SET `DB_ID`='{0}', `authored`='{1}', `authored_class`='{2}', `regulatedEntity`='{3}', `regulatedEntity_class`='{4}', `regulationType`='{5}', `regulationType_class`='{6}', `regulator`='{7}', `regulator_class`='{8}', `releaseDate`='{9}' WHERE `DB_ID` = '{10}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, authored, authored_class, regulatedEntity, regulatedEntity_class, regulationType, regulationType_class, regulator, regulator_class, MySqlScript.ToMySqlDateTimeString(releaseDate), DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As regulation
                         Return DirectCast(MyClass.MemberwiseClone, regulation)
                     End Function
End Class


End Namespace

﻿#Region "Microsoft.VisualBasic::7c710f32050d4b23a70d9d740d3fde36, data\Reactome\LocalMySQL\gk_current\physicalentity_2_inferredfrom.vb"

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

    '   Total Lines: 159
    '    Code Lines: 79 (49.69%)
    ' Comment Lines: 58 (36.48%)
    '    - Xml Docs: 94.83%
    ' 
    '   Blank Lines: 22 (13.84%)
    '     File Size: 7.04 KB


    ' Class physicalentity_2_inferredfrom
    ' 
    '     Properties: DB_ID, inferredFrom, inferredFrom_class, inferredFrom_rank
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
''' DROP TABLE IF EXISTS `physicalentity_2_inferredfrom`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `physicalentity_2_inferredfrom` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `inferredFrom_rank` int(10) unsigned DEFAULT NULL,
'''   `inferredFrom` int(10) unsigned DEFAULT NULL,
'''   `inferredFrom_class` varchar(64) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `inferredFrom` (`inferredFrom`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("physicalentity_2_inferredfrom", Database:="gk_current", SchemaSQL:="
CREATE TABLE `physicalentity_2_inferredfrom` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `inferredFrom_rank` int(10) unsigned DEFAULT NULL,
  `inferredFrom` int(10) unsigned DEFAULT NULL,
  `inferredFrom_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `inferredFrom` (`inferredFrom`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class physicalentity_2_inferredfrom: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("inferredFrom_rank"), DataType(MySqlDbType.Int64, "10"), Column(Name:="inferredFrom_rank")> Public Property inferredFrom_rank As Long
    <DatabaseField("inferredFrom"), DataType(MySqlDbType.Int64, "10"), Column(Name:="inferredFrom")> Public Property inferredFrom As Long
    <DatabaseField("inferredFrom_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="inferredFrom_class")> Public Property inferredFrom_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `physicalentity_2_inferredfrom` (`DB_ID`, `inferredFrom_rank`, `inferredFrom`, `inferredFrom_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `physicalentity_2_inferredfrom` (`DB_ID`, `inferredFrom_rank`, `inferredFrom`, `inferredFrom_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `physicalentity_2_inferredfrom` (`DB_ID`, `inferredFrom_rank`, `inferredFrom`, `inferredFrom_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `physicalentity_2_inferredfrom` (`DB_ID`, `inferredFrom_rank`, `inferredFrom`, `inferredFrom_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `physicalentity_2_inferredfrom` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `physicalentity_2_inferredfrom` SET `DB_ID`='{0}', `inferredFrom_rank`='{1}', `inferredFrom`='{2}', `inferredFrom_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `physicalentity_2_inferredfrom` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `physicalentity_2_inferredfrom` (`DB_ID`, `inferredFrom_rank`, `inferredFrom`, `inferredFrom_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, inferredFrom_rank, inferredFrom, inferredFrom_class)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `physicalentity_2_inferredfrom` (`DB_ID`, `inferredFrom_rank`, `inferredFrom`, `inferredFrom_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, inferredFrom_rank, inferredFrom, inferredFrom_class)
        Else
        Return String.Format(INSERT_SQL, DB_ID, inferredFrom_rank, inferredFrom, inferredFrom_class)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{inferredFrom_rank}', '{inferredFrom}', '{inferredFrom_class}')"
        Else
            Return $"('{DB_ID}', '{inferredFrom_rank}', '{inferredFrom}', '{inferredFrom_class}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `physicalentity_2_inferredfrom` (`DB_ID`, `inferredFrom_rank`, `inferredFrom`, `inferredFrom_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, inferredFrom_rank, inferredFrom, inferredFrom_class)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `physicalentity_2_inferredfrom` (`DB_ID`, `inferredFrom_rank`, `inferredFrom`, `inferredFrom_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, inferredFrom_rank, inferredFrom, inferredFrom_class)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, inferredFrom_rank, inferredFrom, inferredFrom_class)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `physicalentity_2_inferredfrom` SET `DB_ID`='{0}', `inferredFrom_rank`='{1}', `inferredFrom`='{2}', `inferredFrom_class`='{3}' WHERE `DB_ID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, inferredFrom_rank, inferredFrom, inferredFrom_class, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As physicalentity_2_inferredfrom
                         Return DirectCast(MyClass.MemberwiseClone, physicalentity_2_inferredfrom)
                     End Function
End Class


End Namespace

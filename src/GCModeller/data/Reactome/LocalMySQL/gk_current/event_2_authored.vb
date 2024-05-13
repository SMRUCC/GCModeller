#Region "Microsoft.VisualBasic::cc1a83df10bea4e6b3ffed8f40e42387, data\Reactome\LocalMySQL\gk_current\event_2_authored.vb"

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
    '    Code Lines: 79
    ' Comment Lines: 58
    '   Blank Lines: 22
    '     File Size: 6.49 KB


    ' Class event_2_authored
    ' 
    '     Properties: authored, authored_class, authored_rank, DB_ID
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
''' DROP TABLE IF EXISTS `event_2_authored`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `event_2_authored` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `authored_rank` int(10) unsigned DEFAULT NULL,
'''   `authored` int(10) unsigned DEFAULT NULL,
'''   `authored_class` varchar(64) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `authored` (`authored`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("event_2_authored", Database:="gk_current", SchemaSQL:="
CREATE TABLE `event_2_authored` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `authored_rank` int(10) unsigned DEFAULT NULL,
  `authored` int(10) unsigned DEFAULT NULL,
  `authored_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `authored` (`authored`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class event_2_authored: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("authored_rank"), DataType(MySqlDbType.Int64, "10"), Column(Name:="authored_rank")> Public Property authored_rank As Long
    <DatabaseField("authored"), DataType(MySqlDbType.Int64, "10"), Column(Name:="authored")> Public Property authored As Long
    <DatabaseField("authored_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="authored_class")> Public Property authored_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `event_2_authored` (`DB_ID`, `authored_rank`, `authored`, `authored_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `event_2_authored` (`DB_ID`, `authored_rank`, `authored`, `authored_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `event_2_authored` (`DB_ID`, `authored_rank`, `authored`, `authored_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `event_2_authored` (`DB_ID`, `authored_rank`, `authored`, `authored_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `event_2_authored` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `event_2_authored` SET `DB_ID`='{0}', `authored_rank`='{1}', `authored`='{2}', `authored_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `event_2_authored` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `event_2_authored` (`DB_ID`, `authored_rank`, `authored`, `authored_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, authored_rank, authored, authored_class)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `event_2_authored` (`DB_ID`, `authored_rank`, `authored`, `authored_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, authored_rank, authored, authored_class)
        Else
        Return String.Format(INSERT_SQL, DB_ID, authored_rank, authored, authored_class)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{authored_rank}', '{authored}', '{authored_class}')"
        Else
            Return $"('{DB_ID}', '{authored_rank}', '{authored}', '{authored_class}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `event_2_authored` (`DB_ID`, `authored_rank`, `authored`, `authored_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, authored_rank, authored, authored_class)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `event_2_authored` (`DB_ID`, `authored_rank`, `authored`, `authored_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, authored_rank, authored, authored_class)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, authored_rank, authored, authored_class)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `event_2_authored` SET `DB_ID`='{0}', `authored_rank`='{1}', `authored`='{2}', `authored_class`='{3}' WHERE `DB_ID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, authored_rank, authored, authored_class, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As event_2_authored
                         Return DirectCast(MyClass.MemberwiseClone, event_2_authored)
                     End Function
End Class


End Namespace

#Region "Microsoft.VisualBasic::c7f2785bb7cf60cd2f93f9cf88ae3a9f, data\Reactome\LocalMySQL\gk_current\concurrenteventset_2_focusentity.vb"

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
    '     File Size: 7.02 KB


    ' Class concurrenteventset_2_focusentity
    ' 
    '     Properties: DB_ID, focusEntity, focusEntity_class, focusEntity_rank
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
''' DROP TABLE IF EXISTS `concurrenteventset_2_focusentity`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `concurrenteventset_2_focusentity` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `focusEntity_rank` int(10) unsigned DEFAULT NULL,
'''   `focusEntity` int(10) unsigned DEFAULT NULL,
'''   `focusEntity_class` varchar(64) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `focusEntity` (`focusEntity`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("concurrenteventset_2_focusentity", Database:="gk_current", SchemaSQL:="
CREATE TABLE `concurrenteventset_2_focusentity` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `focusEntity_rank` int(10) unsigned DEFAULT NULL,
  `focusEntity` int(10) unsigned DEFAULT NULL,
  `focusEntity_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `focusEntity` (`focusEntity`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class concurrenteventset_2_focusentity: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("focusEntity_rank"), DataType(MySqlDbType.Int64, "10"), Column(Name:="focusEntity_rank")> Public Property focusEntity_rank As Long
    <DatabaseField("focusEntity"), DataType(MySqlDbType.Int64, "10"), Column(Name:="focusEntity")> Public Property focusEntity As Long
    <DatabaseField("focusEntity_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="focusEntity_class")> Public Property focusEntity_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `concurrenteventset_2_focusentity` (`DB_ID`, `focusEntity_rank`, `focusEntity`, `focusEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `concurrenteventset_2_focusentity` (`DB_ID`, `focusEntity_rank`, `focusEntity`, `focusEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `concurrenteventset_2_focusentity` (`DB_ID`, `focusEntity_rank`, `focusEntity`, `focusEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `concurrenteventset_2_focusentity` (`DB_ID`, `focusEntity_rank`, `focusEntity`, `focusEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `concurrenteventset_2_focusentity` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `concurrenteventset_2_focusentity` SET `DB_ID`='{0}', `focusEntity_rank`='{1}', `focusEntity`='{2}', `focusEntity_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `concurrenteventset_2_focusentity` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `concurrenteventset_2_focusentity` (`DB_ID`, `focusEntity_rank`, `focusEntity`, `focusEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, focusEntity_rank, focusEntity, focusEntity_class)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `concurrenteventset_2_focusentity` (`DB_ID`, `focusEntity_rank`, `focusEntity`, `focusEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, focusEntity_rank, focusEntity, focusEntity_class)
        Else
        Return String.Format(INSERT_SQL, DB_ID, focusEntity_rank, focusEntity, focusEntity_class)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{focusEntity_rank}', '{focusEntity}', '{focusEntity_class}')"
        Else
            Return $"('{DB_ID}', '{focusEntity_rank}', '{focusEntity}', '{focusEntity_class}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `concurrenteventset_2_focusentity` (`DB_ID`, `focusEntity_rank`, `focusEntity`, `focusEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, focusEntity_rank, focusEntity, focusEntity_class)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `concurrenteventset_2_focusentity` (`DB_ID`, `focusEntity_rank`, `focusEntity`, `focusEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, focusEntity_rank, focusEntity, focusEntity_class)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, focusEntity_rank, focusEntity, focusEntity_class)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `concurrenteventset_2_focusentity` SET `DB_ID`='{0}', `focusEntity_rank`='{1}', `focusEntity`='{2}', `focusEntity_class`='{3}' WHERE `DB_ID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, focusEntity_rank, focusEntity, focusEntity_class, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As concurrenteventset_2_focusentity
                         Return DirectCast(MyClass.MemberwiseClone, concurrenteventset_2_focusentity)
                     End Function
End Class


End Namespace

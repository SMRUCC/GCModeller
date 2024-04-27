#Region "Microsoft.VisualBasic::09f02eab1054b0583368cd43b3ef8bd1, G:/GCModeller/src/GCModeller/data/Reactome//LocalMySQL/gk_current/catalystactivity_2_activeunit.vb"

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
    '     File Size: 6.89 KB


    ' Class catalystactivity_2_activeunit
    ' 
    '     Properties: activeUnit, activeUnit_class, activeUnit_rank, DB_ID
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
''' DROP TABLE IF EXISTS `catalystactivity_2_activeunit`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `catalystactivity_2_activeunit` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `activeUnit_rank` int(10) unsigned DEFAULT NULL,
'''   `activeUnit` int(10) unsigned DEFAULT NULL,
'''   `activeUnit_class` varchar(64) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `activeUnit` (`activeUnit`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("catalystactivity_2_activeunit", Database:="gk_current", SchemaSQL:="
CREATE TABLE `catalystactivity_2_activeunit` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `activeUnit_rank` int(10) unsigned DEFAULT NULL,
  `activeUnit` int(10) unsigned DEFAULT NULL,
  `activeUnit_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `activeUnit` (`activeUnit`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class catalystactivity_2_activeunit: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("activeUnit_rank"), DataType(MySqlDbType.Int64, "10"), Column(Name:="activeUnit_rank")> Public Property activeUnit_rank As Long
    <DatabaseField("activeUnit"), DataType(MySqlDbType.Int64, "10"), Column(Name:="activeUnit")> Public Property activeUnit As Long
    <DatabaseField("activeUnit_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="activeUnit_class")> Public Property activeUnit_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `catalystactivity_2_activeunit` (`DB_ID`, `activeUnit_rank`, `activeUnit`, `activeUnit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `catalystactivity_2_activeunit` (`DB_ID`, `activeUnit_rank`, `activeUnit`, `activeUnit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `catalystactivity_2_activeunit` (`DB_ID`, `activeUnit_rank`, `activeUnit`, `activeUnit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `catalystactivity_2_activeunit` (`DB_ID`, `activeUnit_rank`, `activeUnit`, `activeUnit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `catalystactivity_2_activeunit` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `catalystactivity_2_activeunit` SET `DB_ID`='{0}', `activeUnit_rank`='{1}', `activeUnit`='{2}', `activeUnit_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `catalystactivity_2_activeunit` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `catalystactivity_2_activeunit` (`DB_ID`, `activeUnit_rank`, `activeUnit`, `activeUnit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, activeUnit_rank, activeUnit, activeUnit_class)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `catalystactivity_2_activeunit` (`DB_ID`, `activeUnit_rank`, `activeUnit`, `activeUnit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, activeUnit_rank, activeUnit, activeUnit_class)
        Else
        Return String.Format(INSERT_SQL, DB_ID, activeUnit_rank, activeUnit, activeUnit_class)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{activeUnit_rank}', '{activeUnit}', '{activeUnit_class}')"
        Else
            Return $"('{DB_ID}', '{activeUnit_rank}', '{activeUnit}', '{activeUnit_class}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `catalystactivity_2_activeunit` (`DB_ID`, `activeUnit_rank`, `activeUnit`, `activeUnit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, activeUnit_rank, activeUnit, activeUnit_class)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `catalystactivity_2_activeunit` (`DB_ID`, `activeUnit_rank`, `activeUnit`, `activeUnit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, activeUnit_rank, activeUnit, activeUnit_class)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, activeUnit_rank, activeUnit, activeUnit_class)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `catalystactivity_2_activeunit` SET `DB_ID`='{0}', `activeUnit_rank`='{1}', `activeUnit`='{2}', `activeUnit_class`='{3}' WHERE `DB_ID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, activeUnit_rank, activeUnit, activeUnit_class, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As catalystactivity_2_activeunit
                         Return DirectCast(MyClass.MemberwiseClone, catalystactivity_2_activeunit)
                     End Function
End Class


End Namespace

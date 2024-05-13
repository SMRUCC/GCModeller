#Region "Microsoft.VisualBasic::bb5916f386d1e72918cc040f0abc4069, data\Reactome\LocalMySQL\gk_current\catalystactivity.vb"

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

    '   Total Lines: 164
    '    Code Lines: 82
    ' Comment Lines: 60
    '   Blank Lines: 22
    '     File Size: 7.39 KB


    ' Class catalystactivity
    ' 
    '     Properties: activity, activity_class, DB_ID, physicalEntity, physicalEntity_class
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
''' DROP TABLE IF EXISTS `catalystactivity`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `catalystactivity` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `activity` int(10) unsigned DEFAULT NULL,
'''   `activity_class` varchar(64) DEFAULT NULL,
'''   `physicalEntity` int(10) unsigned DEFAULT NULL,
'''   `physicalEntity_class` varchar(64) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `activity` (`activity`),
'''   KEY `physicalEntity` (`physicalEntity`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("catalystactivity", Database:="gk_current", SchemaSQL:="
CREATE TABLE `catalystactivity` (
  `DB_ID` int(10) unsigned NOT NULL,
  `activity` int(10) unsigned DEFAULT NULL,
  `activity_class` varchar(64) DEFAULT NULL,
  `physicalEntity` int(10) unsigned DEFAULT NULL,
  `physicalEntity_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `activity` (`activity`),
  KEY `physicalEntity` (`physicalEntity`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class catalystactivity: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("activity"), DataType(MySqlDbType.Int64, "10"), Column(Name:="activity")> Public Property activity As Long
    <DatabaseField("activity_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="activity_class")> Public Property activity_class As String
    <DatabaseField("physicalEntity"), DataType(MySqlDbType.Int64, "10"), Column(Name:="physicalEntity")> Public Property physicalEntity As Long
    <DatabaseField("physicalEntity_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="physicalEntity_class")> Public Property physicalEntity_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `catalystactivity` (`DB_ID`, `activity`, `activity_class`, `physicalEntity`, `physicalEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `catalystactivity` (`DB_ID`, `activity`, `activity_class`, `physicalEntity`, `physicalEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `catalystactivity` (`DB_ID`, `activity`, `activity_class`, `physicalEntity`, `physicalEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `catalystactivity` (`DB_ID`, `activity`, `activity_class`, `physicalEntity`, `physicalEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `catalystactivity` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `catalystactivity` SET `DB_ID`='{0}', `activity`='{1}', `activity_class`='{2}', `physicalEntity`='{3}', `physicalEntity_class`='{4}' WHERE `DB_ID` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `catalystactivity` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `catalystactivity` (`DB_ID`, `activity`, `activity_class`, `physicalEntity`, `physicalEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, activity, activity_class, physicalEntity, physicalEntity_class)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `catalystactivity` (`DB_ID`, `activity`, `activity_class`, `physicalEntity`, `physicalEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, activity, activity_class, physicalEntity, physicalEntity_class)
        Else
        Return String.Format(INSERT_SQL, DB_ID, activity, activity_class, physicalEntity, physicalEntity_class)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{activity}', '{activity_class}', '{physicalEntity}', '{physicalEntity_class}')"
        Else
            Return $"('{DB_ID}', '{activity}', '{activity_class}', '{physicalEntity}', '{physicalEntity_class}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `catalystactivity` (`DB_ID`, `activity`, `activity_class`, `physicalEntity`, `physicalEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, activity, activity_class, physicalEntity, physicalEntity_class)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `catalystactivity` (`DB_ID`, `activity`, `activity_class`, `physicalEntity`, `physicalEntity_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, activity, activity_class, physicalEntity, physicalEntity_class)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, activity, activity_class, physicalEntity, physicalEntity_class)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `catalystactivity` SET `DB_ID`='{0}', `activity`='{1}', `activity_class`='{2}', `physicalEntity`='{3}', `physicalEntity_class`='{4}' WHERE `DB_ID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, activity, activity_class, physicalEntity, physicalEntity_class, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As catalystactivity
                         Return DirectCast(MyClass.MemberwiseClone, catalystactivity)
                     End Function
End Class


End Namespace

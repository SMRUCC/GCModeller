#Region "Microsoft.VisualBasic::82942fb1ec92847f9f412fdc8395d70a, data\Reactome\LocalMySQL\gk_current_dn\physicalentity.vb"

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

    '   Total Lines: 162
    '    Code Lines: 81 (50.00%)
    ' Comment Lines: 59 (36.42%)
    '    - Xml Docs: 94.92%
    ' 
    '   Blank Lines: 22 (13.58%)
    '     File Size: 6.59 KB


    ' Class physicalentity
    ' 
    '     Properties: [class], displayName, id, species, stableId
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

REM  Dump @2018/5/23 13:13:42


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_current_dn

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `physicalentity`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `physicalentity` (
'''   `id` int(32) NOT NULL,
'''   `displayName` varchar(255) NOT NULL,
'''   `species` varchar(255) DEFAULT NULL,
'''   `class` varchar(255) NOT NULL,
'''   `stableId` varchar(32) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `stableId` (`stableId`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("physicalentity", Database:="gk_current_dn", SchemaSQL:="
CREATE TABLE `physicalentity` (
  `id` int(32) NOT NULL,
  `displayName` varchar(255) NOT NULL,
  `species` varchar(255) DEFAULT NULL,
  `class` varchar(255) NOT NULL,
  `stableId` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `stableId` (`stableId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class physicalentity: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "32"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("displayName"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="displayName")> Public Property displayName As String
    <DatabaseField("species"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="species")> Public Property species As String
    <DatabaseField("class"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="class")> Public Property [class] As String
    <DatabaseField("stableId"), DataType(MySqlDbType.VarChar, "32"), Column(Name:="stableId")> Public Property stableId As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `physicalentity` (`id`, `displayName`, `species`, `class`, `stableId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `physicalentity` (`id`, `displayName`, `species`, `class`, `stableId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `physicalentity` (`id`, `displayName`, `species`, `class`, `stableId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `physicalentity` (`id`, `displayName`, `species`, `class`, `stableId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `physicalentity` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `physicalentity` SET `id`='{0}', `displayName`='{1}', `species`='{2}', `class`='{3}', `stableId`='{4}' WHERE `id` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `physicalentity` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `physicalentity` (`id`, `displayName`, `species`, `class`, `stableId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, displayName, species, [class], stableId)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `physicalentity` (`id`, `displayName`, `species`, `class`, `stableId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, displayName, species, [class], stableId)
        Else
        Return String.Format(INSERT_SQL, id, displayName, species, [class], stableId)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{displayName}', '{species}', '{[class]}', '{stableId}')"
        Else
            Return $"('{id}', '{displayName}', '{species}', '{[class]}', '{stableId}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `physicalentity` (`id`, `displayName`, `species`, `class`, `stableId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, displayName, species, [class], stableId)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `physicalentity` (`id`, `displayName`, `species`, `class`, `stableId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, displayName, species, [class], stableId)
        Else
        Return String.Format(REPLACE_SQL, id, displayName, species, [class], stableId)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `physicalentity` SET `id`='{0}', `displayName`='{1}', `species`='{2}', `class`='{3}', `stableId`='{4}' WHERE `id` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, displayName, species, [class], stableId, id)
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

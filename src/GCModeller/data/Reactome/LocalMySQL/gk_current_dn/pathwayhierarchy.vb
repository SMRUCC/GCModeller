#Region "Microsoft.VisualBasic::15c52814f44a68630020941135be12a4, data\Reactome\LocalMySQL\gk_current_dn\pathwayhierarchy.vb"

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

    '   Total Lines: 153
    '    Code Lines: 75 (49.02%)
    ' Comment Lines: 56 (36.60%)
    '    - Xml Docs: 94.64%
    ' 
    '   Blank Lines: 22 (14.38%)
    '     File Size: 5.83 KB


    ' Class pathwayhierarchy
    ' 
    '     Properties: childPathwayId, pathwayId
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
''' DROP TABLE IF EXISTS `pathwayhierarchy`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pathwayhierarchy` (
'''   `pathwayId` int(32) NOT NULL DEFAULT '0',
'''   `childPathwayId` int(32) NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`pathwayId`,`childPathwayId`),
'''   CONSTRAINT `PathwayHierarchy_ibfk_1` FOREIGN KEY (`pathwayId`) REFERENCES `pathway` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pathwayhierarchy", Database:="gk_current_dn", SchemaSQL:="
CREATE TABLE `pathwayhierarchy` (
  `pathwayId` int(32) NOT NULL DEFAULT '0',
  `childPathwayId` int(32) NOT NULL DEFAULT '0',
  PRIMARY KEY (`pathwayId`,`childPathwayId`),
  CONSTRAINT `PathwayHierarchy_ibfk_1` FOREIGN KEY (`pathwayId`) REFERENCES `pathway` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class pathwayhierarchy: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pathwayId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "32"), Column(Name:="pathwayId"), XmlAttribute> Public Property pathwayId As Long
    <DatabaseField("childPathwayId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "32"), Column(Name:="childPathwayId"), XmlAttribute> Public Property childPathwayId As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `pathwayhierarchy` (`pathwayId`, `childPathwayId`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `pathwayhierarchy` (`pathwayId`, `childPathwayId`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `pathwayhierarchy` (`pathwayId`, `childPathwayId`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `pathwayhierarchy` (`pathwayId`, `childPathwayId`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `pathwayhierarchy` WHERE `pathwayId`='{0}' and `childPathwayId`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `pathwayhierarchy` SET `pathwayId`='{0}', `childPathwayId`='{1}' WHERE `pathwayId`='{2}' and `childPathwayId`='{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `pathwayhierarchy` WHERE `pathwayId`='{0}' and `childPathwayId`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pathwayId, childPathwayId)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `pathwayhierarchy` (`pathwayId`, `childPathwayId`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pathwayId, childPathwayId)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `pathwayhierarchy` (`pathwayId`, `childPathwayId`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pathwayId, childPathwayId)
        Else
        Return String.Format(INSERT_SQL, pathwayId, childPathwayId)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pathwayId}', '{childPathwayId}')"
        Else
            Return $"('{pathwayId}', '{childPathwayId}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `pathwayhierarchy` (`pathwayId`, `childPathwayId`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pathwayId, childPathwayId)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `pathwayhierarchy` (`pathwayId`, `childPathwayId`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pathwayId, childPathwayId)
        Else
        Return String.Format(REPLACE_SQL, pathwayId, childPathwayId)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `pathwayhierarchy` SET `pathwayId`='{0}', `childPathwayId`='{1}' WHERE `pathwayId`='{2}' and `childPathwayId`='{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pathwayId, childPathwayId, pathwayId, childPathwayId)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As pathwayhierarchy
                         Return DirectCast(MyClass.MemberwiseClone, pathwayhierarchy)
                     End Function
End Class


End Namespace

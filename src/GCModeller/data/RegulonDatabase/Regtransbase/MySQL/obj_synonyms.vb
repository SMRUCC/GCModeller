#Region "Microsoft.VisualBasic::b9f1843e36f34d667b4465e10dfcc7a6, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\obj_synonyms.vb"

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

    '   Total Lines: 170
    '    Code Lines: 85
    ' Comment Lines: 63
    '   Blank Lines: 22
    '     File Size: 7.69 KB


    ' Class obj_synonyms
    ' 
    '     Properties: art_guid, fl_real_name, obj_guid, pkg_guid, syn_name
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

REM  Dump @2018/5/23 13:13:38


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Regtransbase.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `obj_synonyms`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `obj_synonyms` (
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `obj_guid` int(11) NOT NULL DEFAULT '0',
'''   `syn_name` varchar(50) NOT NULL DEFAULT '',
'''   `fl_real_name` int(1) DEFAULT NULL,
'''   PRIMARY KEY (`obj_guid`,`syn_name`),
'''   KEY `pkg_guid` (`pkg_guid`),
'''   KEY `art_guid` (`art_guid`),
'''   CONSTRAINT `obj_synonyms_ibfk_1` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
'''   CONSTRAINT `obj_synonyms_ibfk_2` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `obj_synonyms_ibfk_3` FOREIGN KEY (`obj_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("obj_synonyms", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `obj_synonyms` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `obj_guid` int(11) NOT NULL DEFAULT '0',
  `syn_name` varchar(50) NOT NULL DEFAULT '',
  `fl_real_name` int(1) DEFAULT NULL,
  PRIMARY KEY (`obj_guid`,`syn_name`),
  KEY `pkg_guid` (`pkg_guid`),
  KEY `art_guid` (`art_guid`),
  CONSTRAINT `obj_synonyms_ibfk_1` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `obj_synonyms_ibfk_2` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `obj_synonyms_ibfk_3` FOREIGN KEY (`obj_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class obj_synonyms: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="art_guid")> Public Property art_guid As Long
    <DatabaseField("obj_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="obj_guid"), XmlAttribute> Public Property obj_guid As Long
    <DatabaseField("syn_name"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="syn_name"), XmlAttribute> Public Property syn_name As String
    <DatabaseField("fl_real_name"), DataType(MySqlDbType.Int64, "1"), Column(Name:="fl_real_name")> Public Property fl_real_name As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `obj_synonyms` (`pkg_guid`, `art_guid`, `obj_guid`, `syn_name`, `fl_real_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `obj_synonyms` (`pkg_guid`, `art_guid`, `obj_guid`, `syn_name`, `fl_real_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `obj_synonyms` (`pkg_guid`, `art_guid`, `obj_guid`, `syn_name`, `fl_real_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `obj_synonyms` (`pkg_guid`, `art_guid`, `obj_guid`, `syn_name`, `fl_real_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `obj_synonyms` WHERE `obj_guid`='{0}' and `syn_name`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `obj_synonyms` SET `pkg_guid`='{0}', `art_guid`='{1}', `obj_guid`='{2}', `syn_name`='{3}', `fl_real_name`='{4}' WHERE `obj_guid`='{5}' and `syn_name`='{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `obj_synonyms` WHERE `obj_guid`='{0}' and `syn_name`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, obj_guid, syn_name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `obj_synonyms` (`pkg_guid`, `art_guid`, `obj_guid`, `syn_name`, `fl_real_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pkg_guid, art_guid, obj_guid, syn_name, fl_real_name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `obj_synonyms` (`pkg_guid`, `art_guid`, `obj_guid`, `syn_name`, `fl_real_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pkg_guid, art_guid, obj_guid, syn_name, fl_real_name)
        Else
        Return String.Format(INSERT_SQL, pkg_guid, art_guid, obj_guid, syn_name, fl_real_name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pkg_guid}', '{art_guid}', '{obj_guid}', '{syn_name}', '{fl_real_name}')"
        Else
            Return $"('{pkg_guid}', '{art_guid}', '{obj_guid}', '{syn_name}', '{fl_real_name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `obj_synonyms` (`pkg_guid`, `art_guid`, `obj_guid`, `syn_name`, `fl_real_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pkg_guid, art_guid, obj_guid, syn_name, fl_real_name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `obj_synonyms` (`pkg_guid`, `art_guid`, `obj_guid`, `syn_name`, `fl_real_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pkg_guid, art_guid, obj_guid, syn_name, fl_real_name)
        Else
        Return String.Format(REPLACE_SQL, pkg_guid, art_guid, obj_guid, syn_name, fl_real_name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `obj_synonyms` SET `pkg_guid`='{0}', `art_guid`='{1}', `obj_guid`='{2}', `syn_name`='{3}', `fl_real_name`='{4}' WHERE `obj_guid`='{5}' and `syn_name`='{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pkg_guid, art_guid, obj_guid, syn_name, fl_real_name, obj_guid, syn_name)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As obj_synonyms
                         Return DirectCast(MyClass.MemberwiseClone, obj_synonyms)
                     End Function
End Class


End Namespace

#Region "Microsoft.VisualBasic::2f8efa1fffb73112b83b3bfaf57544f6, DataMySql\kb_UniProtKB\MySQL\tissue_locations.vb"

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

    ' Class tissue_locations
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2017/9/3 12:29:35


Imports System.Data.Linq.Mapping
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports System.Xml.Serialization

Namespace kb_UniProtKB.mysql

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `tissue_locations`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tissue_locations` (
'''   `hash_code` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) DEFAULT NULL,
'''   `name` varchar(45) DEFAULT NULL,
'''   `tissue_id` int(10) unsigned NOT NULL,
'''   `tissue_name` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`hash_code`,`tissue_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tissue_locations", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `tissue_locations` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `tissue_id` int(10) unsigned NOT NULL,
  `tissue_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`,`tissue_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class tissue_locations: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("hash_code"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code"), XmlAttribute> Public Property hash_code As Long
    <DatabaseField("uniprot_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
    <DatabaseField("tissue_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="tissue_id"), XmlAttribute> Public Property tissue_id As Long
    <DatabaseField("tissue_name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="tissue_name")> Public Property tissue_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `tissue_locations` (`hash_code`, `uniprot_id`, `name`, `tissue_id`, `tissue_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `tissue_locations` (`hash_code`, `uniprot_id`, `name`, `tissue_id`, `tissue_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `tissue_locations` WHERE `hash_code`='{0}' and `tissue_id`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `tissue_locations` SET `hash_code`='{0}', `uniprot_id`='{1}', `name`='{2}', `tissue_id`='{3}', `tissue_name`='{4}' WHERE `hash_code`='{5}' and `tissue_id`='{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `tissue_locations` WHERE `hash_code`='{0}' and `tissue_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, hash_code, tissue_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `tissue_locations` (`hash_code`, `uniprot_id`, `name`, `tissue_id`, `tissue_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, name, tissue_id, tissue_name)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{hash_code}', '{uniprot_id}', '{name}', '{tissue_id}', '{tissue_name}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `tissue_locations` (`hash_code`, `uniprot_id`, `name`, `tissue_id`, `tissue_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, name, tissue_id, tissue_name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `tissue_locations` SET `hash_code`='{0}', `uniprot_id`='{1}', `name`='{2}', `tissue_id`='{3}', `tissue_name`='{4}' WHERE `hash_code`='{5}' and `tissue_id`='{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, hash_code, uniprot_id, name, tissue_id, tissue_name, hash_code, tissue_id)
    End Function
#End Region
Public Function Clone() As tissue_locations
                  Return DirectCast(MyClass.MemberwiseClone, tissue_locations)
              End Function
End Class


End Namespace

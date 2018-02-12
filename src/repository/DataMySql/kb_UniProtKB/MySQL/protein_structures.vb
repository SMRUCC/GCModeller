#Region "Microsoft.VisualBasic::3679f242d42993d0f80909794c95e785, DataMySql\kb_UniProtKB\MySQL\protein_structures.vb"

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

    ' Class protein_structures
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
''' 主要是pdb结构记录数据
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `protein_structures`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein_structures` (
'''   `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `hash_code` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) NOT NULL,
'''   `pdb_id` varchar(45) DEFAULT NULL,
'''   `method` varchar(45) DEFAULT NULL,
'''   `resolution` varchar(45) DEFAULT NULL,
'''   `chains` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='主要是pdb结构记录数据';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein_structures", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `protein_structures` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `pdb_id` varchar(45) DEFAULT NULL,
  `method` varchar(45) DEFAULT NULL,
  `resolution` varchar(45) DEFAULT NULL,
  `chains` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='主要是pdb结构记录数据';")>
Public Class protein_structures: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("hash_code"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code")> Public Property hash_code As Long
    <DatabaseField("uniprot_id"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("pdb_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="pdb_id")> Public Property pdb_id As String
    <DatabaseField("method"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="method")> Public Property method As String
    <DatabaseField("resolution"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="resolution")> Public Property resolution As String
    <DatabaseField("chains"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="chains")> Public Property chains As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `protein_structures` (`uid`, `hash_code`, `uniprot_id`, `pdb_id`, `method`, `resolution`, `chains`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `protein_structures` (`uid`, `hash_code`, `uniprot_id`, `pdb_id`, `method`, `resolution`, `chains`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `protein_structures` WHERE `uid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `protein_structures` SET `uid`='{0}', `hash_code`='{1}', `uniprot_id`='{2}', `pdb_id`='{3}', `method`='{4}', `resolution`='{5}', `chains`='{6}' WHERE `uid` = '{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `protein_structures` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `protein_structures` (`uid`, `hash_code`, `uniprot_id`, `pdb_id`, `method`, `resolution`, `chains`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, hash_code, uniprot_id, pdb_id, method, resolution, chains)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{uid}', '{hash_code}', '{uniprot_id}', '{pdb_id}', '{method}', '{resolution}', '{chains}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `protein_structures` (`uid`, `hash_code`, `uniprot_id`, `pdb_id`, `method`, `resolution`, `chains`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, hash_code, uniprot_id, pdb_id, method, resolution, chains)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `protein_structures` SET `uid`='{0}', `hash_code`='{1}', `uniprot_id`='{2}', `pdb_id`='{3}', `method`='{4}', `resolution`='{5}', `chains`='{6}' WHERE `uid` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, hash_code, uniprot_id, pdb_id, method, resolution, chains, uid)
    End Function
#End Region
Public Function Clone() As protein_structures
                  Return DirectCast(MyClass.MemberwiseClone, protein_structures)
              End Function
End Class


End Namespace

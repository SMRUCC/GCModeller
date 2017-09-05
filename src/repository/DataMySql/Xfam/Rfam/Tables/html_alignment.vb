#Region "Microsoft.VisualBasic::ab5ad5077fb7fabea4c1caf7ee9beda8, ..\repository\DataMySql\Xfam\Rfam\Tables\html_alignment.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 11:55:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `html_alignment`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `html_alignment` (
'''   `rfam_acc` varchar(7) NOT NULL,
'''   `type` enum('seed','genome','seedColorstock','genomeColorstock') NOT NULL,
'''   `html` longblob,
'''   `block` int(6) NOT NULL,
'''   `html_alignmentscol` varchar(45) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   KEY `fk_html_alignments_family1_idx` (`rfam_acc`),
'''   KEY `htmlTypeIdx` (`type`),
'''   KEY `htmlBlockIdx` (`block`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("html_alignment", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `html_alignment` (
  `rfam_acc` varchar(7) NOT NULL,
  `type` enum('seed','genome','seedColorstock','genomeColorstock') NOT NULL,
  `html` longblob,
  `block` int(6) NOT NULL,
  `html_alignmentscol` varchar(45) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  KEY `fk_html_alignments_family1_idx` (`rfam_acc`),
  KEY `htmlTypeIdx` (`type`),
  KEY `htmlBlockIdx` (`block`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class html_alignment: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.String)> Public Property type As String
    <DatabaseField("html"), DataType(MySqlDbType.Blob)> Public Property html As Byte()
    <DatabaseField("block"), NotNull, DataType(MySqlDbType.Int64, "6")> Public Property block As Long
    <DatabaseField("html_alignmentscol"), DataType(MySqlDbType.VarChar, "45")> Public Property html_alignmentscol As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `html_alignment` (`rfam_acc`, `type`, `html`, `block`, `html_alignmentscol`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `html_alignment` (`rfam_acc`, `type`, `html`, `block`, `html_alignmentscol`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `html_alignment` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `html_alignment` SET `rfam_acc`='{0}', `type`='{1}', `html`='{2}', `block`='{3}', `html_alignmentscol`='{4}' WHERE `rfam_acc` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `html_alignment` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `html_alignment` (`rfam_acc`, `type`, `html`, `block`, `html_alignmentscol`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, type, html, block, html_alignmentscol)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{rfam_acc}', '{type}', '{html}', '{block}', '{html_alignmentscol}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `html_alignment` (`rfam_acc`, `type`, `html`, `block`, `html_alignmentscol`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, type, html, block, html_alignmentscol)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `html_alignment` SET `rfam_acc`='{0}', `type`='{1}', `html`='{2}', `block`='{3}', `html_alignmentscol`='{4}' WHERE `rfam_acc` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, type, html, block, html_alignmentscol, rfam_acc)
    End Function
#End Region
End Class


End Namespace


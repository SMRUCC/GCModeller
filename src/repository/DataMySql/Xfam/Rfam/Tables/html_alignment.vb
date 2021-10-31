#Region "Microsoft.VisualBasic::6e5190f46cb9cfbb4f7df3914b6d0c17, DataMySql\Xfam\Rfam\Tables\html_alignment.vb"

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

    ' Class html_alignment
    ' 
    '     Properties: block, html, html_alignmentscol, rfam_acc, type
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

REM  Dump @2018/5/23 13:13:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class html_alignment: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc"), XmlAttribute> Public Property rfam_acc As String
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.String), Column(Name:="type")> Public Property type As String
    <DatabaseField("html"), DataType(MySqlDbType.Blob), Column(Name:="html")> Public Property html As Byte()
    <DatabaseField("block"), NotNull, DataType(MySqlDbType.Int64, "6"), Column(Name:="block")> Public Property block As Long
    <DatabaseField("html_alignmentscol"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="html_alignmentscol")> Public Property html_alignmentscol As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `html_alignment` (`rfam_acc`, `type`, `html`, `block`, `html_alignmentscol`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `html_alignment` (`rfam_acc`, `type`, `html`, `block`, `html_alignmentscol`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `html_alignment` (`rfam_acc`, `type`, `html`, `block`, `html_alignmentscol`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `html_alignment` (`rfam_acc`, `type`, `html`, `block`, `html_alignmentscol`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `html_alignment` WHERE `rfam_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `html_alignment` SET `rfam_acc`='{0}', `type`='{1}', `html`='{2}', `block`='{3}', `html_alignmentscol`='{4}' WHERE `rfam_acc` = '{5}';</SQL>

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
''' ```SQL
''' INSERT INTO `html_alignment` (`rfam_acc`, `type`, `html`, `block`, `html_alignmentscol`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_acc, type, html, block, html_alignmentscol)
        Else
        Return String.Format(INSERT_SQL, rfam_acc, type, html, block, html_alignmentscol)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_acc}', '{type}', '{html}', '{block}', '{html_alignmentscol}')"
        Else
            Return $"('{rfam_acc}', '{type}', '{html}', '{block}', '{html_alignmentscol}')"
        End If
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
''' REPLACE INTO `html_alignment` (`rfam_acc`, `type`, `html`, `block`, `html_alignmentscol`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_acc, type, html, block, html_alignmentscol)
        Else
        Return String.Format(REPLACE_SQL, rfam_acc, type, html, block, html_alignmentscol)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As html_alignment
                         Return DirectCast(MyClass.MemberwiseClone, html_alignment)
                     End Function
End Class


End Namespace

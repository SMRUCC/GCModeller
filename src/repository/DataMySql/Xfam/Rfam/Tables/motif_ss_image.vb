#Region "Microsoft.VisualBasic::d702d850981ece8cb5136b04b5c71cb5, ..\repository\DataMySql\Xfam\Rfam\Tables\motif_ss_image.vb"

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
''' DROP TABLE IF EXISTS `motif_ss_image`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `motif_ss_image` (
'''   `rfam_acc` varchar(7) NOT NULL,
'''   `motif_acc` varchar(7) NOT NULL,
'''   `image` longblob,
'''   KEY `fk_motif_ss_images_family1_idx` (`rfam_acc`),
'''   KEY `fk_motif_ss_images_motif1_idx` (`motif_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("motif_ss_image", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `motif_ss_image` (
  `rfam_acc` varchar(7) NOT NULL,
  `motif_acc` varchar(7) NOT NULL,
  `image` longblob,
  KEY `fk_motif_ss_images_family1_idx` (`rfam_acc`),
  KEY `fk_motif_ss_images_motif1_idx` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class motif_ss_image: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("motif_acc"), NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property motif_acc As String
    <DatabaseField("image"), DataType(MySqlDbType.Blob)> Public Property image As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `motif_ss_image` (`rfam_acc`, `motif_acc`, `image`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `motif_ss_image` (`rfam_acc`, `motif_acc`, `image`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `motif_ss_image` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `motif_ss_image` SET `rfam_acc`='{0}', `motif_acc`='{1}', `image`='{2}' WHERE `rfam_acc` = '{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `motif_ss_image` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `motif_ss_image` (`rfam_acc`, `motif_acc`, `image`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, motif_acc, image)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{rfam_acc}', '{motif_acc}', '{image}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `motif_ss_image` (`rfam_acc`, `motif_acc`, `image`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, motif_acc, image)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `motif_ss_image` SET `rfam_acc`='{0}', `motif_acc`='{1}', `image`='{2}' WHERE `rfam_acc` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, motif_acc, image, rfam_acc)
    End Function
#End Region
End Class


End Namespace


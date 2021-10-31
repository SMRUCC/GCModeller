#Region "Microsoft.VisualBasic::51f28d5f7dfa117b6965b5ae7c615a4c, DataMySql\Xfam\Rfam\Tables\secondary_structure_image.vb"

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

    ' Class secondary_structure_image
    ' 
    '     Properties: image, rfam_acc, type
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
''' DROP TABLE IF EXISTS `secondary_structure_image`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `secondary_structure_image` (
'''   `rfam_acc` varchar(7) NOT NULL,
'''   `type` enum('cons','dist','ent','fcbp','cov','disttruc','maxcm','norm','rchie','species','ss','rscape','rscape-cyk') DEFAULT NULL,
'''   `image` longblob,
'''   KEY `fk_secondary_structure_images_family1_idx` (`rfam_acc`),
'''   KEY `secondatStructureTypeIdx` (`type`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("secondary_structure_image", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `secondary_structure_image` (
  `rfam_acc` varchar(7) NOT NULL,
  `type` enum('cons','dist','ent','fcbp','cov','disttruc','maxcm','norm','rchie','species','ss','rscape','rscape-cyk') DEFAULT NULL,
  `image` longblob,
  KEY `fk_secondary_structure_images_family1_idx` (`rfam_acc`),
  KEY `secondatStructureTypeIdx` (`type`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class secondary_structure_image: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc"), XmlAttribute> Public Property rfam_acc As String
    <DatabaseField("type"), DataType(MySqlDbType.String), Column(Name:="type")> Public Property type As String
    <DatabaseField("image"), DataType(MySqlDbType.Blob), Column(Name:="image")> Public Property image As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `secondary_structure_image` (`rfam_acc`, `type`, `image`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `secondary_structure_image` (`rfam_acc`, `type`, `image`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `secondary_structure_image` (`rfam_acc`, `type`, `image`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `secondary_structure_image` (`rfam_acc`, `type`, `image`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `secondary_structure_image` WHERE `rfam_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `secondary_structure_image` SET `rfam_acc`='{0}', `type`='{1}', `image`='{2}' WHERE `rfam_acc` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `secondary_structure_image` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `secondary_structure_image` (`rfam_acc`, `type`, `image`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, type, image)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `secondary_structure_image` (`rfam_acc`, `type`, `image`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_acc, type, image)
        Else
        Return String.Format(INSERT_SQL, rfam_acc, type, image)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_acc}', '{type}', '{image}')"
        Else
            Return $"('{rfam_acc}', '{type}', '{image}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `secondary_structure_image` (`rfam_acc`, `type`, `image`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, type, image)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `secondary_structure_image` (`rfam_acc`, `type`, `image`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_acc, type, image)
        Else
        Return String.Format(REPLACE_SQL, rfam_acc, type, image)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `secondary_structure_image` SET `rfam_acc`='{0}', `type`='{1}', `image`='{2}' WHERE `rfam_acc` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, type, image, rfam_acc)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As secondary_structure_image
                         Return DirectCast(MyClass.MemberwiseClone, secondary_structure_image)
                     End Function
End Class


End Namespace

#Region "Microsoft.VisualBasic::c1e72174ae12276909da674909d49904, DataMySql\Interpro\Tables\entry2common.vb"

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

    ' Class entry2common
    ' 
    '     Properties: ann_id, entry_ac, order_in
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

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `entry2common`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entry2common` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `ann_id` varchar(7) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `order_in` int(3) NOT NULL,
'''   PRIMARY KEY (`entry_ac`,`ann_id`,`order_in`),
'''   KEY `fk_entry2common$ann_id` (`ann_id`),
'''   CONSTRAINT `fk_entry2common$ann_id` FOREIGN KEY (`ann_id`) REFERENCES `common_annotation` (`ann_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_entry2common$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry2common", Database:="interpro", SchemaSQL:="
CREATE TABLE `entry2common` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `ann_id` varchar(7) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `order_in` int(3) NOT NULL,
  PRIMARY KEY (`entry_ac`,`ann_id`,`order_in`),
  KEY `fk_entry2common$ann_id` (`ann_id`),
  CONSTRAINT `fk_entry2common$ann_id` FOREIGN KEY (`ann_id`) REFERENCES `common_annotation` (`ann_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2common$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class entry2common: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="entry_ac"), XmlAttribute> Public Property entry_ac As String
    <DatabaseField("ann_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="ann_id"), XmlAttribute> Public Property ann_id As String
    <DatabaseField("order_in"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "3"), Column(Name:="order_in"), XmlAttribute> Public Property order_in As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `entry2common` (`entry_ac`, `ann_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `entry2common` (`entry_ac`, `ann_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `entry2common` (`entry_ac`, `ann_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `entry2common` (`entry_ac`, `ann_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `entry2common` WHERE `entry_ac`='{0}' and `ann_id`='{1}' and `order_in`='{2}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `entry2common` SET `entry_ac`='{0}', `ann_id`='{1}', `order_in`='{2}' WHERE `entry_ac`='{3}' and `ann_id`='{4}' and `order_in`='{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `entry2common` WHERE `entry_ac`='{0}' and `ann_id`='{1}' and `order_in`='{2}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac, ann_id, order_in)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry2common` (`entry_ac`, `ann_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, ann_id, order_in)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry2common` (`entry_ac`, `ann_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, entry_ac, ann_id, order_in)
        Else
        Return String.Format(INSERT_SQL, entry_ac, ann_id, order_in)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{entry_ac}', '{ann_id}', '{order_in}')"
        Else
            Return $"('{entry_ac}', '{ann_id}', '{order_in}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `entry2common` (`entry_ac`, `ann_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, ann_id, order_in)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `entry2common` (`entry_ac`, `ann_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, entry_ac, ann_id, order_in)
        Else
        Return String.Format(REPLACE_SQL, entry_ac, ann_id, order_in)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `entry2common` SET `entry_ac`='{0}', `ann_id`='{1}', `order_in`='{2}' WHERE `entry_ac`='{3}' and `ann_id`='{4}' and `order_in`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, ann_id, order_in, entry_ac, ann_id, order_in)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As entry2common
                         Return DirectCast(MyClass.MemberwiseClone, entry2common)
                     End Function
End Class


End Namespace

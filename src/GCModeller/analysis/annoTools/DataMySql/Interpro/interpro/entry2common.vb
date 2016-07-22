#Region "Microsoft.VisualBasic::754cb1b51528d6c200a250a294614896, ..\GCModeller\analysis\Annotation\Interpro\interpro\entry2common.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:52:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
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
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry2common", Database:="interpro")>
Public Class entry2common: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9")> Public Property entry_ac As String
    <DatabaseField("ann_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property ann_id As String
    <DatabaseField("order_in"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "3")> Public Property order_in As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `entry2common` (`entry_ac`, `ann_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `entry2common` (`entry_ac`, `ann_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `entry2common` WHERE `entry_ac`='{0}' and `ann_id`='{1}' and `order_in`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `entry2common` SET `entry_ac`='{0}', `ann_id`='{1}', `order_in`='{2}' WHERE `entry_ac`='{3}' and `ann_id`='{4}' and `order_in`='{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac, ann_id, order_in)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, ann_id, order_in)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, ann_id, order_in)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, ann_id, order_in, entry_ac, ann_id, order_in)
    End Function
#End Region
End Class


End Namespace


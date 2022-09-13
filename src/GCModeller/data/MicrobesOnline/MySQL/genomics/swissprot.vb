#Region "Microsoft.VisualBasic::40c37fd8e34bda991d30648ae44cb198, GCModeller\data\MicrobesOnline\MySQL\genomics\swissprot.vb"

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

    '   Total Lines: 65
    '    Code Lines: 36
    ' Comment Lines: 22
    '   Blank Lines: 7
    '     File Size: 2.99 KB


    ' Class swissprot
    ' 
    '     Properties: accession, description, id, length
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `swissprot`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `swissprot` (
'''   `id` varchar(20) NOT NULL DEFAULT '',
'''   `accession` varchar(20) NOT NULL DEFAULT '',
'''   `length` int(10) NOT NULL DEFAULT '0',
'''   `description` text NOT NULL,
'''   PRIMARY KEY (`id`,`accession`),
'''   UNIQUE KEY `id` (`id`),
'''   KEY `accession` (`accession`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("swissprot")>
Public Class swissprot: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property id As String
    <DatabaseField("accession"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property accession As String
    <DatabaseField("length"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property length As Long
    <DatabaseField("description"), NotNull, DataType(MySqlDbType.Text)> Public Property description As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `swissprot` (`id`, `accession`, `length`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `swissprot` (`id`, `accession`, `length`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `swissprot` WHERE `id`='{0}' and `accession`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `swissprot` SET `id`='{0}', `accession`='{1}', `length`='{2}', `description`='{3}' WHERE `id`='{4}' and `accession`='{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id, accession)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, accession, length, description)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, accession, length, description)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, accession, length, description, id, accession)
    End Function
#End Region
End Class


End Namespace

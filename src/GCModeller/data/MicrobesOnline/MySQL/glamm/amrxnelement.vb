#Region "Microsoft.VisualBasic::ee511cb6e9f3790766dc5db0e7b25e56, GCModeller\data\MicrobesOnline\MySQL\glamm\amrxnelement.vb"

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

    '   Total Lines: 64
    '    Code Lines: 36
    ' Comment Lines: 21
    '   Blank Lines: 7
    '     File Size: 2.85 KB


    ' Class amrxnelement
    ' 
    '     Properties: amRxn_id, id, type, xrefId
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:22:12 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.glamm

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `amrxnelement`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `amrxnelement` (
'''   `id` bigint(20) NOT NULL AUTO_INCREMENT,
'''   `type` varchar(255) NOT NULL,
'''   `xrefId` varchar(255) NOT NULL,
'''   `amRxn_id` bigint(20) NOT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `FK1F5C04406B73B23C` (`amRxn_id`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=12988 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("amrxnelement", Database:="glamm")>
Public Class amrxnelement: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property id As Long
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property type As String
    <DatabaseField("xrefId"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property xrefId As String
    <DatabaseField("amRxn_id"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property amRxn_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `amrxnelement` (`type`, `xrefId`, `amRxn_id`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `amrxnelement` (`type`, `xrefId`, `amRxn_id`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `amrxnelement` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `amrxnelement` SET `id`='{0}', `type`='{1}', `xrefId`='{2}', `amRxn_id`='{3}' WHERE `id` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, type, xrefId, amRxn_id)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, type, xrefId, amRxn_id)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, type, xrefId, amRxn_id, id)
    End Function
#End Region
End Class


End Namespace

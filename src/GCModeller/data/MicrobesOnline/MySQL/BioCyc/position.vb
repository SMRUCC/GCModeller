#Region "Microsoft.VisualBasic::441ffdead62e01fe2d28f84fd202e1ed, GCModeller\data\MicrobesOnline\MySQL\BioCyc\position.vb"

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

    '   Total Lines: 69
    '    Code Lines: 37
    ' Comment Lines: 25
    '   Blank Lines: 7
    '     File Size: 3.21 KB


    ' Class position
    ' 
    '     Properties: objectId, posLeft, posRight, strand, taxId
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:32:12 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.BioCyc

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `position`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `position` (
'''   `taxId` int(20) unsigned NOT NULL,
'''   `objectId` varchar(100) NOT NULL,
'''   `posLeft` int(50) unsigned DEFAULT NULL,
'''   `posRight` int(50) unsigned DEFAULT NULL,
'''   `strand` varchar(4) DEFAULT NULL,
'''   PRIMARY KEY (`objectId`),
'''   KEY `taxId` (`taxId`),
'''   KEY `posLeft` (`posLeft`),
'''   KEY `posRight` (`posRight`),
'''   KEY `strand` (`strand`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("position", Database:="biocyc")>
Public Class position: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("taxId"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property taxId As Long
    <DatabaseField("objectId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property objectId As String
    <DatabaseField("posLeft"), DataType(MySqlDbType.Int64, "50")> Public Property posLeft As Long
    <DatabaseField("posRight"), DataType(MySqlDbType.Int64, "50")> Public Property posRight As Long
    <DatabaseField("strand"), DataType(MySqlDbType.VarChar, "4")> Public Property strand As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `position` (`taxId`, `objectId`, `posLeft`, `posRight`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `position` (`taxId`, `objectId`, `posLeft`, `posRight`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `position` WHERE `objectId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `position` SET `taxId`='{0}', `objectId`='{1}', `posLeft`='{2}', `posRight`='{3}', `strand`='{4}' WHERE `objectId` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, objectId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, taxId, objectId, posLeft, posRight, strand)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, taxId, objectId, posLeft, posRight, strand)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, taxId, objectId, posLeft, posRight, strand, objectId)
    End Function
#End Region
End Class


End Namespace

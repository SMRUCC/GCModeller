#Region "Microsoft.VisualBasic::a3143c5b3bd957f4edef0daa078fac30, GCModeller\data\MicrobesOnline\MySQL\genomics\regulonlinks.vb"

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

    '   Total Lines: 68
    '    Code Lines: 37
    ' Comment Lines: 24
    '   Blank Lines: 7
    '     File Size: 3.51 KB


    ' Class regulonlinks
    ' 
    '     Properties: cluster1, cluster2, link, score, updated
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
''' DROP TABLE IF EXISTS `regulonlinks`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `regulonlinks` (
'''   `cluster1` int(10) unsigned NOT NULL DEFAULT '0',
'''   `cluster2` int(10) unsigned NOT NULL DEFAULT '0',
'''   `link` varchar(50) NOT NULL DEFAULT '',
'''   `score` decimal(10,3) unsigned DEFAULT NULL,
'''   `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`cluster1`,`cluster2`,`link`),
'''   KEY `cluster1` (`cluster1`),
'''   KEY `cluster2` (`cluster2`),
'''   KEY `link` (`link`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("regulonlinks")>
Public Class regulonlinks: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("cluster1"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property cluster1 As Long
    <DatabaseField("cluster2"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property cluster2 As Long
    <DatabaseField("link"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property link As String
    <DatabaseField("score"), DataType(MySqlDbType.Decimal)> Public Property score As Decimal
    <DatabaseField("updated"), NotNull, DataType(MySqlDbType.DateTime)> Public Property updated As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `regulonlinks` (`cluster1`, `cluster2`, `link`, `score`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `regulonlinks` (`cluster1`, `cluster2`, `link`, `score`, `updated`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `regulonlinks` WHERE `cluster1`='{0}' and `cluster2`='{1}' and `link`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `regulonlinks` SET `cluster1`='{0}', `cluster2`='{1}', `link`='{2}', `score`='{3}', `updated`='{4}' WHERE `cluster1`='{5}' and `cluster2`='{6}' and `link`='{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, cluster1, cluster2, link)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, cluster1, cluster2, link, score, DataType.ToMySqlDateTimeString(updated))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, cluster1, cluster2, link, score, DataType.ToMySqlDateTimeString(updated))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, cluster1, cluster2, link, score, DataType.ToMySqlDateTimeString(updated), cluster1, cluster2, link)
    End Function
#End Region
End Class


End Namespace

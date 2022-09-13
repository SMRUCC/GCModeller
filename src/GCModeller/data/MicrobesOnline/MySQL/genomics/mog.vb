#Region "Microsoft.VisualBasic::a4b884083714b43ccebd010d15f75986, GCModeller\data\MicrobesOnline\MySQL\genomics\mog.vb"

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

    '   Total Lines: 63
    '    Code Lines: 36
    ' Comment Lines: 20
    '   Blank Lines: 7
    '     File Size: 2.82 KB


    ' Class mog
    ' 
    '     Properties: metric, mogId, nComponents, nLoci
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
''' DROP TABLE IF EXISTS `mog`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `mog` (
'''   `mogId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `nComponents` int(10) unsigned NOT NULL DEFAULT '0',
'''   `nLoci` int(10) unsigned NOT NULL DEFAULT '0',
'''   `metric` float NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`mogId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("mog")>
Public Class mog: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("mogId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property mogId As Long
    <DatabaseField("nComponents"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nComponents As Long
    <DatabaseField("nLoci"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nLoci As Long
    <DatabaseField("metric"), NotNull, DataType(MySqlDbType.Double)> Public Property metric As Double
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `mog` (`mogId`, `nComponents`, `nLoci`, `metric`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `mog` (`mogId`, `nComponents`, `nLoci`, `metric`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `mog` WHERE `mogId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `mog` SET `mogId`='{0}', `nComponents`='{1}', `nLoci`='{2}', `metric`='{3}' WHERE `mogId` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, mogId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, mogId, nComponents, nLoci, metric)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, mogId, nComponents, nLoci, metric)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, mogId, nComponents, nLoci, metric, mogId)
    End Function
#End Region
End Class


End Namespace

#Region "Microsoft.VisualBasic::5b52867bbeecc913e3cefd374932b24a, GCModeller\data\MicrobesOnline\MySQL\genomics\mogneighborscores.vb"

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
    '    Code Lines: 38
    ' Comment Lines: 23
    '   Blank Lines: 7
    '     File Size: 3.58 KB


    ' Class mogneighborscores
    ' 
    '     Properties: mog1, mog2, nTaxGroups1, nTaxGroups2, nTaxGroupsBoth
    '                 score
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
''' DROP TABLE IF EXISTS `mogneighborscores`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `mogneighborscores` (
'''   `mog1` int(10) unsigned NOT NULL DEFAULT '0',
'''   `mog2` int(10) unsigned NOT NULL DEFAULT '0',
'''   `score` float NOT NULL DEFAULT '0',
'''   `nTaxGroupsBoth` int(10) unsigned NOT NULL DEFAULT '0',
'''   `nTaxGroups1` int(10) unsigned NOT NULL DEFAULT '0',
'''   `nTaxGroups2` int(10) unsigned NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`mog1`,`mog2`),
'''   KEY `mog2` (`mog2`,`mog1`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("mogneighborscores")>
Public Class mogneighborscores: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("mog1"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property mog1 As Long
    <DatabaseField("mog2"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property mog2 As Long
    <DatabaseField("score"), NotNull, DataType(MySqlDbType.Double)> Public Property score As Double
    <DatabaseField("nTaxGroupsBoth"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nTaxGroupsBoth As Long
    <DatabaseField("nTaxGroups1"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nTaxGroups1 As Long
    <DatabaseField("nTaxGroups2"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nTaxGroups2 As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `mogneighborscores` (`mog1`, `mog2`, `score`, `nTaxGroupsBoth`, `nTaxGroups1`, `nTaxGroups2`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `mogneighborscores` (`mog1`, `mog2`, `score`, `nTaxGroupsBoth`, `nTaxGroups1`, `nTaxGroups2`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `mogneighborscores` WHERE `mog1`='{0}' and `mog2`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `mogneighborscores` SET `mog1`='{0}', `mog2`='{1}', `score`='{2}', `nTaxGroupsBoth`='{3}', `nTaxGroups1`='{4}', `nTaxGroups2`='{5}' WHERE `mog1`='{6}' and `mog2`='{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, mog1, mog2)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, mog1, mog2, score, nTaxGroupsBoth, nTaxGroups1, nTaxGroups2)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, mog1, mog2, score, nTaxGroupsBoth, nTaxGroups1, nTaxGroups2)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, mog1, mog2, score, nTaxGroupsBoth, nTaxGroups1, nTaxGroups2, mog1, mog2)
    End Function
#End Region
End Class


End Namespace

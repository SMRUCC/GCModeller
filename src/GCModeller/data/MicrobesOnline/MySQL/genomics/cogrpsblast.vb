#Region "Microsoft.VisualBasic::d1a884278d35c6ab7b5bfa69215d3ac5, GCModeller\data\MicrobesOnline\MySQL\genomics\cogrpsblast.vb"

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

    '   Total Lines: 83
    '    Code Lines: 45
    ' Comment Lines: 31
    '   Blank Lines: 7
    '     File Size: 5.53 KB


    ' Class cogrpsblast
    ' 
    '     Properties: alignLength, evalue, gap, identity, locusId
    '                 mismatch, qBegin, qEnd, sBegin, score
    '                 sEnd, subject, version
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
''' DROP TABLE IF EXISTS `cogrpsblast`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `cogrpsblast` (
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `version` int(2) unsigned NOT NULL DEFAULT '1',
'''   `subject` varchar(20) NOT NULL DEFAULT '',
'''   `identity` float unsigned NOT NULL DEFAULT '0',
'''   `alignLength` int(10) unsigned NOT NULL DEFAULT '0',
'''   `mismatch` int(10) unsigned NOT NULL DEFAULT '0',
'''   `gap` int(10) unsigned NOT NULL DEFAULT '0',
'''   `qBegin` int(10) unsigned NOT NULL DEFAULT '0',
'''   `qEnd` int(10) unsigned NOT NULL DEFAULT '0',
'''   `sBegin` int(10) unsigned NOT NULL DEFAULT '0',
'''   `sEnd` int(10) unsigned NOT NULL DEFAULT '0',
'''   `evalue` double NOT NULL DEFAULT '0',
'''   `score` float NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`locusId`,`version`,`subject`,`qBegin`,`qEnd`,`sBegin`,`sEnd`),
'''   KEY `locusId` (`locusId`,`version`),
'''   KEY `subject` (`subject`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("cogrpsblast")>
Public Class cogrpsblast: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("subject"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property subject As String
    <DatabaseField("identity"), NotNull, DataType(MySqlDbType.Double)> Public Property identity As Double
    <DatabaseField("alignLength"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property alignLength As Long
    <DatabaseField("mismatch"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property mismatch As Long
    <DatabaseField("gap"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property gap As Long
    <DatabaseField("qBegin"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property qBegin As Long
    <DatabaseField("qEnd"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property qEnd As Long
    <DatabaseField("sBegin"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property sBegin As Long
    <DatabaseField("sEnd"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property sEnd As Long
    <DatabaseField("evalue"), NotNull, DataType(MySqlDbType.Double)> Public Property evalue As Double
    <DatabaseField("score"), NotNull, DataType(MySqlDbType.Double)> Public Property score As Double
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `cogrpsblast` (`locusId`, `version`, `subject`, `identity`, `alignLength`, `mismatch`, `gap`, `qBegin`, `qEnd`, `sBegin`, `sEnd`, `evalue`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `cogrpsblast` (`locusId`, `version`, `subject`, `identity`, `alignLength`, `mismatch`, `gap`, `qBegin`, `qEnd`, `sBegin`, `sEnd`, `evalue`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `cogrpsblast` WHERE `locusId`='{0}' and `version`='{1}' and `subject`='{2}' and `qBegin`='{3}' and `qEnd`='{4}' and `sBegin`='{5}' and `sEnd`='{6}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `cogrpsblast` SET `locusId`='{0}', `version`='{1}', `subject`='{2}', `identity`='{3}', `alignLength`='{4}', `mismatch`='{5}', `gap`='{6}', `qBegin`='{7}', `qEnd`='{8}', `sBegin`='{9}', `sEnd`='{10}', `evalue`='{11}', `score`='{12}' WHERE `locusId`='{13}' and `version`='{14}' and `subject`='{15}' and `qBegin`='{16}' and `qEnd`='{17}' and `sBegin`='{18}' and `sEnd`='{19}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId, version, subject, qBegin, qEnd, sBegin, sEnd)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locusId, version, subject, identity, alignLength, mismatch, gap, qBegin, qEnd, sBegin, sEnd, evalue, score)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locusId, version, subject, identity, alignLength, mismatch, gap, qBegin, qEnd, sBegin, sEnd, evalue, score)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locusId, version, subject, identity, alignLength, mismatch, gap, qBegin, qEnd, sBegin, sEnd, evalue, score, locusId, version, subject, qBegin, qEnd, sBegin, sEnd)
    End Function
#End Region
End Class


End Namespace

#Region "Microsoft.VisualBasic::4eb92d77c13be9d29ebc6648b4c457c9, GCModeller\data\MicrobesOnline\MySQL\genomics\locus2pdb.vb"

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

    '   Total Lines: 87
    '    Code Lines: 47
    ' Comment Lines: 33
    '   Blank Lines: 7
    '     File Size: 6.04 KB


    ' Class locus2pdb
    ' 
    '     Properties: alignLength, evalue, evalueDisp, gap, identity
    '                 locusId, mismatch, pdbBegin, pdbChain, pdbEnd
    '                 pdbId, score, seqBegin, seqEnd, version
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
''' DROP TABLE IF EXISTS `locus2pdb`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `locus2pdb` (
'''   `pdbId` varchar(6) NOT NULL DEFAULT '',
'''   `pdbChain` char(1) NOT NULL DEFAULT '',
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `version` int(2) unsigned NOT NULL DEFAULT '0',
'''   `seqBegin` int(5) unsigned NOT NULL DEFAULT '0',
'''   `seqEnd` int(5) unsigned NOT NULL DEFAULT '0',
'''   `pdbBegin` int(5) unsigned NOT NULL DEFAULT '0',
'''   `pdbEnd` int(5) unsigned NOT NULL DEFAULT '0',
'''   `identity` decimal(5,2) unsigned NOT NULL DEFAULT '0.00',
'''   `alignLength` int(5) unsigned NOT NULL DEFAULT '0',
'''   `mismatch` int(5) unsigned NOT NULL DEFAULT '0',
'''   `gap` int(5) unsigned NOT NULL DEFAULT '0',
'''   `evalue` double NOT NULL DEFAULT '0',
'''   `evalueDisp` text NOT NULL,
'''   `score` decimal(10,2) unsigned NOT NULL DEFAULT '0.00',
'''   PRIMARY KEY (`pdbId`,`pdbChain`,`locusId`,`version`,`seqBegin`,`seqEnd`),
'''   KEY `locusId` (`locusId`,`version`),
'''   KEY `pdbId` (`pdbId`,`pdbChain`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("locus2pdb")>
Public Class locus2pdb: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pdbId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6")> Public Property pdbId As String
    <DatabaseField("pdbChain"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property pdbChain As String
    <DatabaseField("locusId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("seqBegin"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "5")> Public Property seqBegin As Long
    <DatabaseField("seqEnd"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "5")> Public Property seqEnd As Long
    <DatabaseField("pdbBegin"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property pdbBegin As Long
    <DatabaseField("pdbEnd"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property pdbEnd As Long
    <DatabaseField("identity"), NotNull, DataType(MySqlDbType.Decimal)> Public Property identity As Decimal
    <DatabaseField("alignLength"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property alignLength As Long
    <DatabaseField("mismatch"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property mismatch As Long
    <DatabaseField("gap"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property gap As Long
    <DatabaseField("evalue"), NotNull, DataType(MySqlDbType.Double)> Public Property evalue As Double
    <DatabaseField("evalueDisp"), NotNull, DataType(MySqlDbType.Text)> Public Property evalueDisp As String
    <DatabaseField("score"), NotNull, DataType(MySqlDbType.Decimal)> Public Property score As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `locus2pdb` (`pdbId`, `pdbChain`, `locusId`, `version`, `seqBegin`, `seqEnd`, `pdbBegin`, `pdbEnd`, `identity`, `alignLength`, `mismatch`, `gap`, `evalue`, `evalueDisp`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `locus2pdb` (`pdbId`, `pdbChain`, `locusId`, `version`, `seqBegin`, `seqEnd`, `pdbBegin`, `pdbEnd`, `identity`, `alignLength`, `mismatch`, `gap`, `evalue`, `evalueDisp`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `locus2pdb` WHERE `pdbId`='{0}' and `pdbChain`='{1}' and `locusId`='{2}' and `version`='{3}' and `seqBegin`='{4}' and `seqEnd`='{5}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `locus2pdb` SET `pdbId`='{0}', `pdbChain`='{1}', `locusId`='{2}', `version`='{3}', `seqBegin`='{4}', `seqEnd`='{5}', `pdbBegin`='{6}', `pdbEnd`='{7}', `identity`='{8}', `alignLength`='{9}', `mismatch`='{10}', `gap`='{11}', `evalue`='{12}', `evalueDisp`='{13}', `score`='{14}' WHERE `pdbId`='{15}' and `pdbChain`='{16}' and `locusId`='{17}' and `version`='{18}' and `seqBegin`='{19}' and `seqEnd`='{20}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pdbId, pdbChain, locusId, version, seqBegin, seqEnd)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdbId, pdbChain, locusId, version, seqBegin, seqEnd, pdbBegin, pdbEnd, identity, alignLength, mismatch, gap, evalue, evalueDisp, score)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdbId, pdbChain, locusId, version, seqBegin, seqEnd, pdbBegin, pdbEnd, identity, alignLength, mismatch, gap, evalue, evalueDisp, score)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pdbId, pdbChain, locusId, version, seqBegin, seqEnd, pdbBegin, pdbEnd, identity, alignLength, mismatch, gap, evalue, evalueDisp, score, pdbId, pdbChain, locusId, version, seqBegin, seqEnd)
    End Function
#End Region
End Class


End Namespace

#Region "Microsoft.VisualBasic::0606059fcf3e972aebbd8bb17a85669f, Bio.Repository\test\Module1.vb"

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

'   Total Lines: 60
'    Code Lines: 42 (70.00%)
' Comment Lines: 3 (5.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 15 (25.00%)
'     File Size: 1.68 KB


' Module Module1
' 
'     Sub: loadIndexTest, Main, read1, readTest, write1
'          writeTest
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.KEGG.Metabolism

Module Module1

    Sub Main()
        Call loadIndexTest()
        ' Call write1()
        ' Call read1()
        ' Call readTest()
        Call writeTest()
    End Sub

    Sub loadIndexTest()
        Dim pool As New List(Of FtpIndex)
        Dim qgram As New QGramIndex(6)

        For Each asm In FtpIndex.LoadIndex("D:\datapool\assembly_summary_genbank.txt").Take(1000000)
            Call pool.Add(asm)
            Call qgram.AddString(asm.organism_name)
        Next

        Pause()
    End Sub

    Sub write1()
        Dim list = ReactionClass.ScanRepository("E:\biodeep\biodeepdb_v3\KEGG\reaction_class").ToArray

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\reaction_class.repo".Open
            Call ReactionClassPack.WriteKeggDb(list, file)
        End Using
    End Sub

    Sub read1()
        Dim list As ReactionClass()

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\reaction_class.repo".Open
            list = ReactionClassPack.ReadKeggDb(file)
        End Using

        Pause()
    End Sub

    Sub writeTest()
        Dim list = CompoundRepository.ScanRepository("E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd").ToArray

        For Each cpd In list
            cpd.KCF = Nothing
        Next

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd.repo".Open
            Call KEGGCompoundPack.WriteKeggDb(list, file)
        End Using
    End Sub

    Sub readTest()
        Dim list As Compound()

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd.repo".Open(IO.FileMode.Open, doClear:=False, [readOnly]:=True)
            list = KEGGCompoundPack.ReadKeggDb(file)
        End Using

        Pause()
    End Sub
End Module

#Region "Microsoft.VisualBasic::b8801b3849e515a7efe572a950355ca2, GCModeller\analysis\Metagenome\Metagenome\SILVA\SILVABuild.vb"

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

    '   Total Lines: 31
    '    Code Lines: 20
    ' Comment Lines: 7
    '   Blank Lines: 4
    '     File Size: 1.06 KB


    ' Module SILVABuild
    ' 
    '     Function: SILVABacteria
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module SILVABuild

    ''' <summary>
    ''' 从SILVA序列库之中筛选出细菌或者古生菌的16s序列
    ''' </summary>
    ''' <param name="silva"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function SILVABacteria(silva As IEnumerable(Of FastaSeq)) As IEnumerable(Of FastaSeq)
        Dim title$
        Dim header As NamedValue(Of String)

        For Each seq As FastaSeq In silva
            title = seq.Title
            header = title.GetTagValue(" ", trim:=True)

            If InStr(header.Value, "Bacteria;", CompareMethod.Text) > 0 Then
                Yield seq
            ElseIf InStr(header.Value, "Archaea;", CompareMethod.Text) > 0 Then
                Yield seq
            Else
                ' 不是细菌或者古生菌的16S序列
                ' 跳过
            End If
        Next
    End Function
End Module

#Region "Microsoft.VisualBasic::05f27729909fedbae107245ad9b29a38, LocalBLAST\Pipeline\Extensions.vb"

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

'     Module Extensions
' 
'         Function: SkipHitNotFound
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Pipeline

    <HideModuleName> Public Module Extensions

        ''' <summary>
        ''' 在反序列化之前，使用这个过滤器过滤掉一些无效的hits来节省反序列的处理时间
        ''' </summary>
        ''' <returns></returns>
        Public Function SkipHitNotFound() As NamedValue(Of Func(Of String, Boolean))
            Return New NamedValue(Of Func(Of String, Boolean)) With {
                .Name = "hit_name",
                .Value = Function(colVal)
                             ' 将所有的HITS_NOT_FOUND的行都跳过
                             ' 这样子可以节省比较多的内存
                             Return colVal = "HITS_NOT_FOUND"
                         End Function
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="getAnnotationTerm">通过这个函数指针来获取得到KO或者GO注释信息</param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function UniProtProteinExports(source As IEnumerable(Of entry), getAnnotationTerm As Func(Of entry, String)) As IEnumerable(Of FastaSeq)
            Dim term As Value(Of String) = ""
            Dim i As i32 = 0
            Dim headers$()
            Dim seq$
            Dim fa As FastaSeq

            For Each prot As entry In source.Where(Function(g) Not g.sequence Is Nothing)
                Dim KO As dbReference = prot.KO

                If KO Is Nothing Then
                    Continue For
                End If

                If (term = getAnnotationTerm(prot)).StringEmpty Then
                    Continue For
                End If

                seq = prot.ProteinSequence
                headers = {term, prot.accessions.First & " " & prot.proteinFullName, prot.organism.scientificName}
                fa = New FastaSeq With {
                    .SequenceData = seq,
                    .Headers = headers
                }

                Yield fa

                If ++i Mod 100 = 0 Then
                    Console.Write(i)
                    Console.Write(vbTab)
                End If
            Next
        End Function
    End Module
End Namespace

#Region "Microsoft.VisualBasic::f6690d41c98ef26561b00ddda0ed7931, GCModeller\engine\IO\GCMarkupLanguage\v2\Xml\Genome\replicon.vb"

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

    '   Total Lines: 77
    '    Code Lines: 51
    ' Comment Lines: 11
    '   Blank Lines: 15
    '     File Size: 2.45 KB


    '     Class replicon
    ' 
    '         Properties: genomeName, isPlasmid, operons, RNAs
    ' 
    '         Function: GetGeneList, GetGeneNumbers, ToString
    ' 
    '         Sub: RemoveByIdList
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace v2

    ''' <summary>
    ''' 复制子
    ''' </summary>
    Public Class replicon

        ''' <summary>
        ''' 当前的这个复制子对象是否是质粒基因组？
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property isPlasmid As Boolean
        <XmlAttribute> Public Property genomeName As String

        <XmlElement>
        Public Property operons As TranscriptUnit()

        ''' <summary>
        ''' 除了mRNA的其他的一切非蛋白编码RNA
        ''' </summary>
        ''' <returns></returns>
        Public Property RNAs As XmlList(Of RNA)

        Public Function GetGeneNumbers() As Integer
            Return Aggregate TU As TranscriptUnit
                   In operons
                   Into Sum(TU.numOfGenes)
        End Function

        Public Iterator Function GetGeneList() As IEnumerable(Of gene)
            For Each operon As TranscriptUnit In operons
                For Each gene As gene In operon.genes.AsEnumerable
                    Yield gene
                Next
            Next
        End Function

        Public Sub RemoveByIdList(deleted As Index(Of String))
            Dim newList As New List(Of TranscriptUnit)

            For Each item As TranscriptUnit In operons
                item.genes = item.genes _
                    .AsEnumerable _
                    .Where(Function(g)
                               Return Not g.locus_tag Like deleted
                           End Function) _
                    .ToArray

                If item.numOfGenes > 0 Then
                    newList.Add(item)
                End If
            Next

            RNAs.items = RNAs.items _
                .Where(Function(RNA)
                           Return Not RNA.gene Like deleted
                       End Function) _
                .ToArray

            operons = newList.ToArray
        End Sub

        Public Overrides Function ToString() As String
            Dim type$ = "Genome" Or "Plasmid genome".When(isPlasmid)
            Dim strVal$ = $"[{type}] {genomeName}"

            Return strVal
        End Function

    End Class
End Namespace

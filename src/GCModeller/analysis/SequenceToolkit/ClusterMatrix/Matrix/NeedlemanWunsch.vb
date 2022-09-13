#Region "Microsoft.VisualBasic::a693b533d7f29ca250d49a67caf8c8f8, GCModeller\analysis\SequenceToolkit\ClusterMatrix\Matrix\NeedlemanWunsch.vb"

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

    '   Total Lines: 74
    '    Code Lines: 63
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 2.73 KB


    ' Module Matrix
    ' 
    '     Function: [As], __needlemanWunsch, NeedlemanWunsch
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Matrix

    <Extension> Private Function [As](Of T, V)(o As T) As V
        Return DirectCast(CObj(o), V)
    End Function

    <Extension>
    Public Function NeedlemanWunsch(locis As IEnumerable(Of FastaSeq), Optional ByRef out As StreamWriter = Nothing) As DataSet()
        Dim buffer As FastaSeq() = locis.ToArray
        Dim LQuery = From fa As SeqValue(Of FastaSeq)
                     In buffer _
                         .SeqIterator _
                         .AsParallel
                     Select index = fa.i,
                         vector = (+fa).__needlemanWunsch(buffer)
                     Order By index Ascending
        Dim tuples = LQuery.Select(Function(v) v.vector).ToArray
        Dim result As DataSet() = tuples _
            .Select(Function(d) d.vector) _
            .ToArray

        If Not out Is Nothing Then
            For Each query In tuples
                Call out.WriteLine(query.title)
                Call out.WriteLine()

                For Each target In query.log
                    Call out.WriteLine(target.Name)
                    Call target.Value.Print(dev:=out)
                Next

                Call out.WriteLine("---------------------------------------------------------------------")
                Call out.WriteLine()
            Next
        End If

        Return result
    End Function

    <Extension>
    Private Function __needlemanWunsch(query As FastaSeq, buffer As FastaSeq()) As (vector As DataSet, log As List(Of NamedCollection(Of GlobalAlign(Of Char))), title$)
        Dim dev As New List(Of NamedCollection(Of GlobalAlign(Of Char)))
        Dim score#
        Dim vector As New Dictionary(Of String, Double)

        For Each target As FastaSeq In buffer
            dev += New NamedCollection(Of GlobalAlign(Of Char)) With {
                .Name = target.Title,
                .Value = RunNeedlemanWunsch _
                    .RunAlign(query, target, score) _
                    .ToArray
            }
            vector.Add(target.Title, score)
        Next

        Call query.Title.__DEBUG_ECHO

        Return (vector:=New DataSet With {
            .ID = query.Title,
            .Properties = vector
        }, Log:=dev,
        title:=query.Title)
    End Function
End Module

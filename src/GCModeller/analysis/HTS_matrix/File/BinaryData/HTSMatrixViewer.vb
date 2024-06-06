#Region "Microsoft.VisualBasic::503cd86643b0da81cf68ce5c41051b1a, analysis\HTS_matrix\File\BinaryData\HTSMatrixViewer.vb"

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

    '   Total Lines: 86
    '    Code Lines: 65 (75.58%)
    ' Comment Lines: 4 (4.65%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 17 (19.77%)
    '     File Size: 2.70 KB


    ' Class HTSMatrixViewer
    ' 
    '     Properties: FeatureIDs, SampleIDs
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: (+2 Overloads) GetGeneExpression, GetSampleOrdinal
    ' 
    '     Sub: SetNewGeneIDs, SetNewSampleIDs
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Class HTSMatrixViewer : Inherits MatrixViewer

    ReadOnly matrix As Matrix
    ReadOnly sampleIndex As Index(Of String)
    ReadOnly geneIndex As Index(Of String)

    ''' <summary>
    ''' [width(sample_size), height(geneset_size)]
    ''' </summary>
    ReadOnly dims As Size

    Public Overrides ReadOnly Property SampleIDs As IEnumerable(Of String)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return matrix.sampleID
        End Get
    End Property

    Public Overrides ReadOnly Property FeatureIDs As IEnumerable(Of String)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return matrix.rownames
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(matrix As Matrix)
        Me.matrix = matrix
        Me.sampleIndex = matrix.sampleID.Indexing
        Me.geneIndex = matrix.rownames.Indexing
        Me.dims = New Size(sampleIndex.Count, geneIndex.Count)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetSampleOrdinal(sampleID As String) As Integer
        Return sampleIndex.IndexOf(sampleID)
    End Function

    Public Overrides Function GetGeneExpression(geneID As String) As Double()
        Dim i As Integer = geneIndex.IndexOf(geneID)

        If i < 0 Then
            Return New Double(dims.Width - 1) {}
        Else
            Return matrix.gene(i).experiments
        End If
    End Function

    Public Overrides Function GetGeneExpression(geneID() As String, sampleOrdinal As Integer) As Double()
        Dim v As Double() = New Double(geneID.Length - 1) {}

        If sampleOrdinal < 0 Then
            Return v
        End If

        For i As Integer = 0 To geneID.Length - 1
            Dim rowId As Integer = geneIndex.IndexOf(geneID(i))

            If rowId < 0 Then
                ' v(i) = 0.0
            Else
                v(i) = matrix.gene(i).experiments()(sampleOrdinal)
            End If
        Next

        Return v
    End Function

    Public Overrides Sub SetNewGeneIDs(geneIDs() As String)
        Me.geneIndex.Clear()
        Me.geneIndex.Add(geneIDs).ToArray

        For i As Integer = 0 To matrix.expression.Length - 1
            matrix.expression(i).geneID = geneIDs(i)
        Next
    End Sub

    Public Overrides Sub SetNewSampleIDs(sampleIDs() As String)
        Me.sampleIndex.Clear()
        Me.sampleIndex.Add(sampleIDs).ToArray
    End Sub
End Class

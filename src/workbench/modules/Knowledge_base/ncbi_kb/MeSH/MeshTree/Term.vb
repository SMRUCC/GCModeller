﻿#Region "Microsoft.VisualBasic::c55380da933bef359db17510b173f8aa, mzkit\src\metadb\Massbank\Public\NCBI\MeSH\RDF.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 13
'    Code Lines: 9
' Comment Lines: 0
'   Blank Lines: 4
'     File Size: 292 B


'     Class Term
' 
'         Properties: term, tree
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace MeSH.Tree

    ''' <summary>
    ''' the mesh tree
    ''' </summary>
    Public Class Term : Inherits Synonym

        ''' <summary>
        ''' the mesh term name
        ''' </summary>
        ''' <returns></returns>
        Public Property term As String

        ''' <summary>
        ''' the tree path
        ''' </summary>
        ''' <returns></returns>
        Public Property tree As String()
        Public Property description As String

        Public ReadOnly Iterator Property category As IEnumerable(Of MeshCategory)
            Get
                If isSimpleTree() Then
                    Yield Reader.ParseCategory(tree.First)
                Else
                    For Each tree As String In Me.tree
                        Yield Reader.ParseCategory(tree)
                    Next
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            If isSimpleTree() Then
                Return $"[{tree.JoinBy("->")}] {term}"
            End If

            Return $"[{accessionID}] {term} ({description})"
        End Function

        Private Function isSimpleTree() As Boolean
            Return accessionID.StringEmpty AndAlso tree.All(Function(ti) ti.IndexOf("."c) = -1)
        End Function

    End Class
End Namespace
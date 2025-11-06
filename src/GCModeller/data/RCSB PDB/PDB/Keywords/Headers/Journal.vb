#Region "Microsoft.VisualBasic::41fa03c7e9381ed49b559db860d230ff, data\RCSB PDB\PDB\Keywords\Headers\Journal.vb"

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

    '   Total Lines: 76
    '    Code Lines: 64 (84.21%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (15.79%)
    '     File Size: 2.86 KB


    '     Class Journal
    ' 
    '         Properties: author, doi, EDIT, Keyword, pmid
    '                     PUBL, ref, refn, title
    ' 
    '         Function: Append, ToString
    ' 
    '         Sub: Flush
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace Keywords

    Public Class Journal : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_JRNL
            End Get
        End Property

        Dim cache As New List(Of NamedValue(Of String))

        Public Property author As String()
        Public Property title As String
        Public Property ref As String
        Public Property refn As String
        Public Property pmid As String
        Public Property doi As String
        Public Property PUBL As String
        Public Property EDIT As String

        Public Overrides Function ToString() As String
            Return title
        End Function

        Friend Shared Function Append(ByRef jrnl As Journal, str As String) As Journal
            If jrnl Is Nothing Then
                jrnl = New Journal
            End If
            jrnl.cache.Add(str.Trim.GetTagValue(" ", trim:=True, failureNoName:=False))
            Return jrnl
        End Function

        Friend Overrides Sub Flush()
            Dim cache = Me.cache _
                .GroupBy(Function(a) a.Name) _
                .Select(Function(a) (a.Key, a _
                    .Select(Function(t)
                                Dim numprefix = t.Value.Match("\d+\s+")

                                If numprefix.Length > 0 AndAlso InStr(t.Value, numprefix) = 1 Then
                                    Return t.Value.Substring(numprefix.Length).Trim
                                End If

                                Return t.Value
                            End Function) _
                    .JoinBy(" "))) _
                .ToArray

            For Each tuple As (name$, value$) In cache
                Select Case tuple.name
                    Case "AUTH" : author = tuple.value.Split(","c)
                    Case "TITL" : title = tuple.value
                    Case "REF" : ref = tuple.value
                    Case "REFN" : refn = tuple.value
                    Case "PMID" : pmid = tuple.value
                    Case "DOI" : doi = tuple.value
                    Case "PUBL" : PUBL = tuple.value
                    Case "EDIT" : EDIT = tuple.value

                    Case Else
                        If tuple.name.IsPattern("AUTH\d+") Then
                            author = author _
                                .JoinIterates(tuple.value.Split(","c)) _
                                .ToArray
                        Else
                            Throw New NotImplementedException("journal data: " & tuple.name)
                        End If
                End Select
            Next
        End Sub
    End Class
End Namespace

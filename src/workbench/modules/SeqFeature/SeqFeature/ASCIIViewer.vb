#Region "Microsoft.VisualBasic::3ca4ec493bdfb4a0d1b62d91da242d32, modules\SeqFeature\SeqFeature\ASCIIViewer.vb"

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

    '   Total Lines: 104
    '    Code Lines: 65
    ' Comment Lines: 22
    '   Blank Lines: 17
    '     File Size: 3.41 KB


    ' Module ASCIIViewer
    ' 
    '     Sub: DisplayOn
    ' 
    ' Class Site
    ' 
    '     Properties: Left, Name
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 只适合显示短序列上面的Feature
''' </summary>
Public Module ASCIIViewer

    '                           ProtK 
    'Ch_lo_Pn1.3_Pn2_ProtK_Therm| 
    '           Glu_ProtK_Staph|| 
    '            AspGluN_ProtK||| 
    '  ArgC_Clost_Therm_Tryps|||| 
    '        Glu_ProtK_Staph||||| 
    '               AspGluN|||||| 

    '                     ||||||| 
    '                     SERVELAT
    '                 1   --------   8

    <Extension>
    Public Sub DisplayOn(sites As IEnumerable(Of Site), seq$, Optional dev As TextWriter = Nothing, Optional deli$ = ", ")
        ' 先按照位点的位置进行分组
        Dim groups = sites _
            .GroupBy(Function(site) site.Left) _
            .OrderByDescending(Function(g) g.Key) _
            .ToArray
        Dim labels = groups _
            .Select(Function(g)
                        Return g.Select(Function(site) site.Name) _
                                .OrderBy(Function(s) s) _
                                .Distinct _
                                .JoinBy(deli)
                    End Function) _
            .ToArray
        Dim lens%() = labels.Select(AddressOf Len).ToArray
        Dim lefts%() = groups.Select(Function(g) g.Key).ToArray
        Dim seqLen% = seq.Length
        Dim maxOffset% = lens.Max + seqLen

        With dev Or App.StdOut
            For i As Integer = 0 To labels.Length - 1
                Dim labeList$ = labels(i)
                Dim left% = lefts(i)
                Dim labelLength = lens(i)
                Dim offset = maxOffset - labeList.Length - seqLen + left

                Call .Write(New String(" "c, offset + 1))
                Call .Write(labeList)

                If i > 0 Then
                    For j As Integer = i - 1 To 0 Step -1
                        Dim delta% = lefts(j) - left

                        Call .Write(New String(" "c, delta - 1) & "|")
                        Call left.SetValue(lefts(j))
                    Next
                End If

                Call .WriteLine()
            Next

            Dim l As New List(Of Char)

            maxOffset = maxOffset - seq.Length + 1

            With lefts.Indexing
                For j As Integer = 1 To seq.Length
                    If .IndexOf(x:=j) > -1 Then
                        l += "|"c
                    Else
                        l += " "c
                    End If
                Next
            End With

            Call .WriteLine()
            Call .WriteLine(New String(" "c, maxOffset) & l.CharString)
            Call .WriteLine(New String(" "c, maxOffset) & seq)
            Call .WriteLine(New String(" "c, maxOffset - 4) & $"1   {New String("-"c, seq.Length)}   {seq.Length}")

            Call .Flush()
        End With
    End Sub
End Module

Public Class Site

    ''' <summary>
    ''' 位点的名称
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    ''' <summary>
    ''' 这个位点在序列上面的位置
    ''' </summary>
    ''' <returns></returns>
    Public Property Left As Integer

    Public Overrides Function ToString() As String
        Return $"{Name} @ {Left}"
    End Function
End Class

#Region "Microsoft.VisualBasic::425e4276e10f296402a6ed2e0943eeb8, RDotNet.Extensions.Bioinformatics\Declares\CRAN\VennDiagram\VennDiagram\Extensions.vb"

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
    '         Function: ParseName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace VennDiagram.ModelAPI

    Public Module Extensions

        ''' <summary>
        ''' 尝试着从一个字符串集合中猜测出可能的名称
        ''' </summary>
        ''' <param name="source">基因号列表</param>
        ''' <returns>猜测出基因号的物种前缀，例如XC_1184 -> XC_</returns>
        ''' <remarks></remarks>
        <Extension> Public Function ParseName(source As Generic.IEnumerable(Of String), Serial As Integer) As String
            Dim LCollection = (From s As String In source.AsParallel Where Not String.IsNullOrEmpty(s) Select s Distinct).ToArray
            Dim Name As List(Of Char) = New List(Of Char)
            For i As Integer = 0 To (From s As String In LCollection.AsParallel Select Len(s)).Min - 1
                Dim p As Integer = i
                Dim LQuery = (From s As String In LCollection Select s(p) Distinct).ToArray '

                If LQuery.Length = 1 Then
                    Name += LQuery.First
                Else
                    Exit For
                End If
            Next

            If Name.Count > 0 Then
                Return New String(Name.ToArray)
            Else
                Return "Serial_" & Serial
            End If
        End Function
    End Module
End Namespace

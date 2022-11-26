#Region "Microsoft.VisualBasic::8d1058a3f5f71ab13762d9658016374f, GCModeller\models\BioCyc\Reader\FeatureElement.vb"

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

    '   Total Lines: 118
    '    Code Lines: 95
    ' Comment Lines: 1
    '   Blank Lines: 22
    '     File Size: 3.94 KB


    ' Class FeatureElement
    ' 
    '     Properties: attributes, uniqueId
    ' 
    '     Function: ParseBuffer, removeBreakLines, ToString
    ' 
    ' Class ValueString
    ' 
    '     Properties: attributes, value
    ' 
    '     Function: getAttributes, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions

Public Class FeatureElement : Implements IReadOnlyId

    Public ReadOnly Property uniqueId As String Implements IReadOnlyId.Identity
        Get
            Return attributes("UNIQUE-ID").First.value
        End Get
    End Property

    Public Property attributes As Dictionary(Of String, ValueString())

    Default Public ReadOnly Property getValue(key As String) As ValueString()
        Get
            Return attributes.TryGetValue(key)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return uniqueId
    End Function

    Private Shared Function removeBreakLines(buffer As String()) As IEnumerable(Of String)
        Dim newBuf As New List(Of String)

        For i As Integer = 0 To buffer.Length - 1
            If buffer(i).StartsWith("/") Then
                newBuf(newBuf.Count - 1) &= buffer(i).Trim("/"c, " "c)
            Else
                newBuf.Add(buffer(i))
            End If
        Next

        Return newBuf
    End Function

    Friend Shared Function ParseBuffer(buffer As String()) As FeatureElement
        Dim attrs As New List(Of NamedValue(Of ValueString))
        Dim innerAttrs As New List(Of NamedValue(Of String))
        Dim temp As NamedValue(Of String) = Nothing

        buffer = removeBreakLines(buffer).ToArray

        For Each line As String In buffer
            If line.StartsWith("^") Then
                ' is attribute value of the previous string value
                Call innerAttrs.Add(line.Trim("^").GetTagValue(" - "))
            Else
                If Not temp.IsEmpty Then
                    Call attrs.Add(New NamedValue(Of ValueString) With {
                        .Name = temp.Name,
                        .Value = New ValueString With {
                            .value = temp.Value,
                            .attributes = innerAttrs.PopAll
                        }
                    })
                End If

                temp = line.GetTagValue(" - ")
            End If
        Next

        Call attrs.Add(New NamedValue(Of ValueString) With {
            .Name = temp.Name,
            .Value = New ValueString With {
                .value = temp.Value,
                .attributes = innerAttrs.PopAll
            }
        })

        Return New FeatureElement With {
            .attributes = attrs _
                .GroupBy(Function(s) s.Name) _
                .ToDictionary(Function(s) s.Key,
                              Function(s)
                                  Return (From a As NamedValue(Of ValueString) In s Select a.Value).ToArray
                              End Function)
        }
    End Function

End Class

Public Class ValueString

    Public Property value As String
    Public Property attributes As NamedValue(Of String)()

    Default Public ReadOnly Property getValue(key As String) As String
        Get
            Return attributes _
                .Where(Function(a) a.Name = key) _
                .FirstOrDefault _
                .Value
        End Get
    End Property

    Public Function getAttributes(key As String) As String()
        Return (From attr As NamedValue(Of String)
                In attributes
                Where attr.Name = key
                Select attr.Value).ToArray
    End Function

    Public Overrides Function ToString() As String
        Return value
    End Function

    Public Shared Operator &(val As ValueString, str As String) As ValueString
        val.value &= str
        Return val
    End Operator

End Class

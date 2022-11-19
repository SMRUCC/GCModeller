#Region "Microsoft.VisualBasic::0bd2fb03a271c13479e51c1a91d1c63e, GCModeller\core\Bio.Assembly\ComponentModel\ValuePair.vb"

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

    '   Total Lines: 81
    '    Code Lines: 62
    ' Comment Lines: 5
    '   Blank Lines: 14
    '     File Size: 2.76 KB


    '     Module KeyValuePairExtensions
    ' 
    '         Function: CreateObject, Distinct, Equals, Format_Prints, GetKey
    '                   GetValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace ComponentModel

    ''' <summary>
    ''' String type key value pair.(两个字符串构成的键值对)
    ''' </summary>
    ''' <remarks></remarks>
    '''
    Public Module KeyValuePairExtensions

        Public Function Format_Prints(data As IEnumerable(Of KeyValuePair)) As String
            If data Is Nothing Then
                Return ""
            End If

            Dim sBuilder As StringBuilder = New StringBuilder(New String("-"c, 120) & vbCrLf, capacity:=2048)
            Dim max As Integer = (From item In data Select Len(item.Key)).Max

            For Each item As KeyValuePair In data
                Dim s = String.Format("  {0} {1} ==> {2}", item.Key, New String(" "c, max - Len(item.Key) + 2), item.Value)
                Call sBuilder.AppendLine(s)
            Next

            Return sBuilder.ToString
        End Function

        Public Function CreateObject(key As String, value As String) As KeyValuePair
            Return New KeyValuePair With {
                .Key = key,
                .Value = value
            }
        End Function

        Public Function Equals(x As KeyValuePair, obj As KeyValuePair, Optional strict As Boolean = True) As Boolean
            If strict Then
                Return String.Equals(obj.Key, x.Key) AndAlso String.Equals(obj.Value, x.Value)
            Else
                Return String.Equals(obj.Key, x.Key, StringComparison.OrdinalIgnoreCase) AndAlso
                       String.Equals(obj.Value, x.Value, StringComparison.OrdinalIgnoreCase)
            End If
        End Function

        Public Function Distinct(Collection As KeyValuePair()) As KeyValuePair()
            Dim lst = (From obj As KeyValuePair
                       In Collection
                       Select obj
                       Order By obj.Key Ascending).AsList
            For i As Integer = 0 To lst.Count - 1
                If i >= lst.Count Then
                    Exit For
                End If
                Dim item = lst(i)

                For j As Integer = i + 1 To lst.Count - 1
                    If j >= lst.Count Then
                        Exit For
                    End If
                    If item.Equals(lst(j)) Then
                        Call lst.RemoveAt(j)
                        j -= 1
                    End If
                Next
            Next

            Return lst.ToArray
        End Function

#Region "Linq lambda"

        Public Function GetKey(k As KeyValuePair) As String
            Return k.Key
        End Function

        Public Function GetValue(k As KeyValuePair) As String
            Return k.Value
        End Function
#End Region
    End Module
End Namespace

#Region "Microsoft.VisualBasic::c90f8edbcc11402058fc0b5bea081078, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\ObjectBase.vb"

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

    '   Total Lines: 82
    '    Code Lines: 0
    ' Comment Lines: 69
    '   Blank Lines: 13
    '     File Size: 3.15 KB


    ' 
    ' /********************************************************************************/

#End Region

'Imports System.Text
'Imports System.Text.RegularExpressions
'Imports Microsoft.VisualBasic

'Namespace Assembly.MetaCyc.File

'    ''' <summary>
'    ''' Base class object in frs
'    ''' </summary>
'    ''' <remarks></remarks>
'    Public Class ObjectBase

'        Public Property TextLine As String()

'        Public Function ContactAdditionalAttribute() As ObjectBase
'            Dim List As List(Of String) = New List(Of String)
'            Dim LastBuilder As StringBuilder = New StringBuilder(TextLine.First, 1024)

'            For Each strValue As String In TextLine.Skip(1)
'                If strValue.First = "^"c Then
'                    Call LastBuilder.Append(String.Format(" [{0}]", strValue))
'                Else
'                    Call List.Add(LastBuilder.ToString)
'                    Call LastBuilder.Clear()
'                    Call LastBuilder.Append(strValue)
'                End If
'            Next
'            Call List.Add(LastBuilder.ToString)

'            Return New ObjectBase With {.TextLine = List.ToArray}
'        End Function

'        Private Function ContactArray() As ObjectBase
'            Dim NewArrayList As List(Of String) = New List(Of String)
'            Dim sBuilder As StringBuilder = New StringBuilder(1024)

'            For Each strLine As String In Me.TextLine
'                If strLine.First = "/"c Then
'                    strLine = Mid(strLine, 2)
'                    Call sBuilder.Append(strLine & " ")
'                Else
'                    If sBuilder.Length = 0 Then
'                        Call NewArrayList.Add(strLine)
'                    Else
'                        NewArrayList(NewArrayList.Count - 1) = NewArrayList.Last & " " & sBuilder.ToString.Trim.Replace("  ", " ")
'                        Call sBuilder.Clear()
'                    End If
'                End If
'            Next

'            Me.TextLine = NewArrayList.ToArray
'            Return Me
'        End Function

'        Protected Friend Shared Function CreateDictionary([Object] As ObjectBase) As MetaCyc.File.DataFiles.Slots.Object
'            Dim LQuery = (From strData As String
'                          In [Object].ContactArray.TextLine.AsParallel
'                          Let Tokens As String() = strData.Split
'                          Let PropertyName As String = Tokens.First
'                          Let Value As String = Mid(strData, Len(PropertyName) + 4)
'                          Let Item = New KeyValuePair(Of String, String)(PropertyName, Value)
'                          Select Item
'                          Order By Item.Key Ascending).AsList
'            Return LQuery
'        End Function

'        ''' <summary>
'        ''' 会保留断行
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Overrides Function ToString() As String
'            Dim sBuilder As StringBuilder = New StringBuilder(768)

'            For Each e As String In TextLine
'                sBuilder.AppendLine(e)
'            Next

'            Return sBuilder.ToString
'        End Function
'    End Class
'End Namespace

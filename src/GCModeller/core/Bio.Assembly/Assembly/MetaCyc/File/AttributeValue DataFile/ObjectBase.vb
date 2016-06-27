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
'                          Order By Item.Key Ascending).ToList
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
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.ShoalShell.HTML

Namespace SPM.Nodes

    ''' <summary>
    ''' 一个方法的元数据
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EntryPointMeta : Implements HTML.IWikiHandle

        <XmlAttribute> Public Property Name As String
        Public Property Description As String
        <XmlElement> Public Property Parameters As NamedValue(Of String)()
        Public Property ReturnedType As String

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Description) Then
                Return Name
            Else
                Return $"{Name}:  {Description}"
            End If
        End Function

        Public Function Match(keyword As String) As String Implements IWikiHandle.Match
            Dim Head As String = String.Format("[{0}]", keyword)

            If InStr(Name, keyword, CompareMethod.Text) > 0 Then
                Head = "Function Entry:  " & Name.ToLower.Replace(keyword.ToLower, Head)
            ElseIf InStr(Description, keyword, CompareMethod.Text) > 0 Then
                Head = Description.ToLower.Replace(keyword.ToLower, Head)
            ElseIf InStr(ReturnedType, keyword, CompareMethod.Text) > 0 Then
                Head = "Function Return Type:  " & ReturnedType.ToLower.Replace(keyword.ToLower, Head)
            Else
                Dim n = MatchParameters(keyword)

                If n.IsNullOrEmpty Then
                    Return ""   '没有匹配上任何数据
                Else
                    Head = String.Join(vbCrLf, (From item In n Select String.Format("{0}  {1}  {2}", item.Name, item.Item1, item.Item2).ToLower.Replace(keyword.ToLower, Head)))
                End If
            End If

            Return ">>>>  " & Head & vbCrLf & vbCrLf & GenerateDescription()
        End Function

        Public Function MatchParameters(keyword As String) As NamedTuple(Of String)()
            If Parameters.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim LQuery = (From item In Parameters
                          Where InStr(item.Name, keyword, CompareMethod.Text) > 0 OrElse
                              InStr(item.Value, keyword, CompareMethod.Text) > 0 OrElse
                              InStr(item.Description, keyword, CompareMethod.Text) > 0
                          Select item).ToArray
            Return LQuery
        End Function

        Public Function GenerateDescription() As String Implements IWikiHandle.GenerateDescription
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Call sBuilder.AppendLine("Name:        " & Name)
            Call sBuilder.AppendLine("Description: " & If(String.IsNullOrEmpty(Description), "This function have no description data defined.", Description))
            Call sBuilder.AppendLine("Return:      " & ReturnedType)

            If Not Parameters.IsNullOrEmpty Then
                Dim Max As Integer = (From item In Parameters Select Len(item.Name)).ToArray.Max

                Call sBuilder.AppendLine(vbCrLf & String.Format("Function have {0} parameters:", Parameters.Length))
                Call sBuilder.AppendLine(String.Format("-Name-{0}------Type--------------", New String("-"c, Max)))

                For Each p In Parameters
                    Call sBuilder.AppendLine(String.Format(" {0}  {1} {2}  {3}", p.Name, New String(" "c, 6 + Max - Len(p.Name)), p.Value, If(Not String.IsNullOrEmpty(p.Description), "// " & p.Description, "")))
                Next
            Else
                Call sBuilder.AppendLine("This function doesn't required of the parameters.")
            End If

            Return sBuilder.ToString
        End Function
    End Class
End Namespace
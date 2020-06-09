Imports System.Text

Module Interpolation

    ''' <summary>
    ''' 在模板之中可能还会存在html碎片的插值
    ''' 在这里进行模板的html碎片的加载
    ''' 
    ''' 模板的插值格式为${relpath}
    ''' </summary>
    ''' <param name="templateUrl">the file path of the template</param>
    ''' <returns></returns>
    Public Function Interpolate(templateUrl As String) As String
        Dim dir As String = templateUrl.ParentPath
        Dim template As New StringBuilder(templateUrl.ReadAllText)
        Dim fragmentRefs As String() = template.ToString.Matches("[$]\{.+?\}", RegexICSng).ToArray
        Dim url As String
        Dim part As String

        For Each ref As String In fragmentRefs
            url = dir & "/" & ref.GetStackValue("{", "}")
            part = Interpolate(templateUrl:=url)
            template.Replace(ref, part)
        Next

        Return template.ToString
    End Function
End Module

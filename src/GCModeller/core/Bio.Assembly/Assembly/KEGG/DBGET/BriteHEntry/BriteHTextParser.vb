Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Module BriteHTextParser

        Friend ReadOnly ClassLevels As Char() = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data$">文本内容或者文件的路径</param>
        ''' <returns></returns>
        Public Function Load(data$) As BriteHText
            Dim lines As String() = LinqAPI.Exec(Of String) <=
 _
                From s As String
                In data.Replace("<b>", "").Replace("</b>", "").LineTokens
                Where Not String.IsNullOrEmpty(s) AndAlso (Array.IndexOf(ClassLevels, s.First) > -1 AndAlso Len(s) > 1)
                Select s

            Return Load(lines, data(1))
        End Function

        Public Function Load(lines$(), Optional depth$ = "Z"c) As BriteHText
            Dim classes As New List(Of BriteHText)
            Dim p As Integer = 0
            Dim root As New BriteHText With {
                .ClassLabel = "/",
                .Level = -1,
                .Degree = depth
            }

            Do While p < lines.Length - 1
                Call classes.Add(LoadData(lines, p, level:=0, parent:=root))
            Loop

            root.CategoryItems = classes

            Return root
        End Function

        ''' <summary>
        ''' 递归加载层次数据
        ''' </summary>
        ''' <param name="strLines"></param>
        ''' <param name="p"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Private Function LoadData(strLines As String(), ByRef p As Integer, level As Integer, parent As BriteHText) As BriteHText
            Dim Category As New BriteHText With {
                .Level = level,
                .ClassLabel = Mid(strLines(p), 2).Trim,
                .Parent = parent
            }

            p += 1

            If p > strLines.Length - 1 Then
                Return Category
            End If

            If strLines(p).First > Category.CategoryLevel Then
                Dim subCategory As New List(Of BriteHText)

                Do While strLines(p).First > Category.CategoryLevel
                    Call subCategory.Add(
                        LoadData(strLines, p, level + 1, parent:=Category))

                    If p > strLines.Length - 1 Then
                        Exit Do
                    End If
                Loop

                Category.CategoryItems = subCategory.ToArray
            End If

            Return Category
        End Function
    End Module
End Namespace
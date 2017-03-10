Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG

    Public Module Extensions

        ''' <summary>
        ''' 这个主要是应用于ID mapping操作的拓展函数
        ''' </summary>
        ''' <param name="text$">
        ''' Example as the text data in the kegg drugs or kegg disease object:
        ''' 
        ''' ```
        ''' E2A-PBX1 (translocation) [HSA:6929 5087] [KO:K09063 K09355]
        ''' ```
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function GetIDpairedList(text$) As Dictionary(Of String, String())
            Dim ids$() = Regex _
                .Matches(text, "\[.+?\]", RegexICSng) _
                .ToArray(Function(s) s.GetStackValue("[", "]"))

            ' 可能还会存在多重数据，所以在这里不能够直接生成字典
            Dim table As New Dictionary(Of String, List(Of String))

            For Each id As String In ids
                Dim k = id.GetTagValue(":")

                If Not table.ContainsKey(k.Name) Then
                    table.Add(
                        k.Name,
                        New List(Of String))
                End If

                Call table(k.Name).AddRange(k.Value.Split)
            Next

            Dim out As Dictionary(Of String, String()) =
                table.ToDictionary(
                Function(k) k.Key,
                Function(k) k.Value.ToArray)

            Return out
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="tag$"></param>
        ''' <param name="list$"></param>
        ''' <returns>
        ''' Example as:
        ''' 
        ''' ```
        ''' HSA:6929 5087
        ''' ```
        ''' </returns>
        <Extension>
        Public Function IDlistStrings(tag$, list$()) As String
            Return $"{tag}:{list.JoinBy(" ")}"
        End Function
    End Module
End Namespace
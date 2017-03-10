Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

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
            Dim table As Dictionary(Of String, String()) = ids _
                .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                .ToDictionary(Function(k) k.Name,
                              Function(v) v.Value.StringSplit("\s+"))
            Return table
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
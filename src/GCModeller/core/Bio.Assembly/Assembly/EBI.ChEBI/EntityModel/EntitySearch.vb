Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Levenshtein

Namespace Assembly.EBI.ChEBI

    Public Module EntitySearch

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <param name="name$">模糊查找</param>
        ''' <param name="formula$">直接使用字符串相等</param>
        ''' <returns></returns>
        <Extension>
        Public Function SearchByNameAndFormula(chebi As Dictionary(Of Long, ChEBIEntity),
                                               name$,
                                               formula$,
                                               Optional parallel As Boolean = True) As ChEBIEntity

            Dim source As IEnumerable(Of ChEBIEntity)
            Dim nameString As New LevenshteinString(name.ToLower)

            If parallel Then
                source = chebi.Values.AsParallel
            Else
                source = chebi.Values
            End If

            Dim LQuery = LinqAPI.DefaultFirst(Of ChEBIEntity) <=
 _
                From metabolite As ChEBIEntity
                In source
                Where metabolite.Formulae.data.TextEquals(formula)
                Let match = nameString Like LCase(metabolite.chebiAsciiName)
                Where Not match Is Nothing AndAlso match.MatchSimilarity >= 0.75
                Select metabolite

            Return LQuery
        End Function
    End Module
End Namespace
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Levenshtein
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML

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
                                               Optional parallel As Boolean = True) As IEnumerable(Of ChEBIEntity)

            Dim source As IEnumerable(Of ChEBIEntity)
            Dim nameString As New LevenshteinString(name.ToLower)

            If parallel Then
                source = chebi.Values.AsParallel
            Else
                source = chebi.Values
            End If

            Dim LQuery = From metabolite As ChEBIEntity
                         In source
                         Where (Not metabolite.Formulae Is Nothing AndAlso
                             metabolite.Formulae.data.TextEquals(formula))
                         Let match = nameString Like LCase(metabolite.chebiAsciiName)
                         Where Not match Is Nothing AndAlso
                             match.MatchSimilarity >= 0.75
                         Select metabolite

            Return LQuery
        End Function

        <Extension>
        Public Function SearchByNameEqualsAny(chebi As Dictionary(Of Long, ChEBIEntity), name$) As ChEBIEntity
            For Each metabolite As ChEBIEntity In chebi.Values
                If metabolite.NameEqualsAny(name) Then
                    Return metabolite
                End If
            Next

            Return Nothing
        End Function

        <Extension> Public Function NameEqualsAny(chebi As ChEBIEntity, name$) As Boolean
            If chebi.chebiAsciiName.TextEquals(name) Then
                Return True
            Else
                For Each s As Synonyms In chebi.Synonyms.SafeQuery
                    If s.data.TextEquals(name) Then
                        Return True
                    End If
                Next
                For Each s As Synonyms In chebi.IupacNames.SafeQuery
                    If s.data.TextEquals(name) Then
                        Return True
                    End If
                Next
            End If

            Return False
        End Function
    End Module
End Namespace
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Levenshtein
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML

Namespace Assembly.EBI.ChEBI

    Public Module EntitySearch

        ''' <summary>
        ''' 返回的结果之中的名称为chebi的数字主编号
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <param name="name$">模糊查找</param>
        ''' <param name="formula$">直接使用字符串相等</param>
        ''' <returns></returns>
        <Extension>
        Public Function SearchByNameAndFormula(chebi As Dictionary(Of Long, ChEBIEntity),
                                               name$,
                                               formula$,
                                               Optional parallel As Boolean = True) As IEnumerable(Of NamedCollection(Of DistResult))

            Dim source As IEnumerable(Of ChEBIEntity)
            Dim nameString As New LevenshteinString(name.ToLower)
            Dim matchFormula = From metabolite As ChEBIEntity
                               In chebi.Values
                               Where (Not metabolite.Formulae Is Nothing AndAlso
                                   metabolite.Formulae.data.TextEquals(formula))

            If parallel Then
                source = matchFormula.AsParallel
            Else
                source = matchFormula
            End If

            Dim LQuery = From metabolite As ChEBIEntity
                         In source
                         Let matches = metabolite _
                             .EnumerateNames _
                             .Select(Function(s) nameString Like LCase(s)) _
                             .Where(Function(m) Not m Is Nothing) _
                             .ToArray
                         Where Not matches.IsNullOrEmpty
                         Select New NamedCollection(Of DistResult) With {
                             .Name = metabolite.chebiId.Split(":"c).Last,
                             .Value = matches
                         }

            Return LQuery
        End Function

        ''' <summary>
        ''' 进行<see cref="ChEBIEntity.chebiAsciiName"/>和他的同义词的精确匹配
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <param name="name$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SearchByNameEqualsAny(chebi As Dictionary(Of Long, ChEBIEntity), name$) As ChEBIEntity
            For Each metabolite As ChEBIEntity In chebi.Values
                If metabolite.NameEqualsAny(name) Then
                    Return metabolite
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' 枚举出这个chebi化合物之中的所有的化合物名称
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <returns></returns>
        <Extension> Public Function EnumerateNames(chebi As ChEBIEntity) As String()
            Dim list As New List(Of String)

            list += chebi.chebiAsciiName
            list += chebi.Synonyms.SafeQuery.Select(Function(s) s.data)
            list += chebi.IupacNames.SafeQuery.Select(Function(s) s.data)

            Return list
        End Function

        ''' <summary>
        ''' 进行不区分大小写的字符串精确匹配
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <param name="name$"></param>
        ''' <returns></returns>
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
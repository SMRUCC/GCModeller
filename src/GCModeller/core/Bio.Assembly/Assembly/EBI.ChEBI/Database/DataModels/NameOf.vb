Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv
Imports Microsoft.VisualBasic.Text.Similarity

Namespace Assembly.EBI.ChEBI

    ''' <summary>
    ''' 使用这个模块进行chebi的编号的匹配
    ''' </summary>
    Public Class [NameOf]

        ''' <summary>
        ''' 键名都是小写的
        ''' </summary>
        Dim names As Dictionary(Of String, Tables.Names())
        Dim chebiXrefs As Dictionary(Of String, Tables.Accession())
        ' Dim DbXrefs As

        Sub New(table As TSVTables)
            Dim accIDs = table.GetDatabaseAccessions
            Dim names = table.GetNames
            Dim chemicals = table.GetChemicalData

            Me.names = names _
                .GroupBy(Function(name) name.NAME.ToLower) _
                .ToDictionary(Function(k) k.Key,
                              Function(g) g.ToArray)
            Me.chebiXrefs = accIDs _
                .GroupBy(Function(acc) acc.COMPOUND_ID) _
                .ToDictionary(Function(k) k.Key,
                              Function(g) g.ToArray)
        End Sub

        Public Function MatchByName(name$, Optional fuzzy As Boolean = False) As Tables.Accession()
            Dim getByNameKey =
                Function(nameKey$)
                    Dim names As Tables.Names() = Me.names(nameKey)
                    Dim out As Tables.Accession() = names _
                        .Where(Function(id) chebiXrefs.ContainsKey(id.COMPOUND_ID)) _
                        .Select(Function(id) chebiXrefs(id.COMPOUND_ID)) _
                        .IteratesALL _
                        .ToArray
                    Return out
                End Function

            name = name.ToLower

            If Me.names.ContainsKey(name) Then
                Return getByNameKey(nameKey:=name)
            Else
                If fuzzy Then
                    Dim matches = From nameValue As String
                                  In names.Keys.AsParallel
                                  Let d = Evaluations.LevenshteinEvaluate(name, nameValue)
                                  Select nameValue, d
                                  Order By d Descending
                    name = matches.First.nameValue
                    Return getByNameKey(nameKey:=name)
                Else
                    Return {}
                End If
            End If
        End Function

        Public Function MatchByID(ID$) As Tables.Accession()
            Throw New NotImplementedException
        End Function

#Region "由于有同分异构体之类的存在，所以即使化学式或者分子质量相同，也会匹配出几种不同的化合物，所以这两个方法应该是优先级别最低的"
        Public Function MatchByFormula(formula$) As Tables.Accession()
            Throw New NotImplementedException
        End Function

        Public Function MatchByMass(mass#, Optional deltaPPM# = 1) As Tables.Accession()
            Throw New NotImplementedException
        End Function
#End Region
    End Class
End Namespace
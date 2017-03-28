Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Similarity
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv

Namespace Assembly.EBI.ChEBI

    ''' <summary>
    ''' 使用这个模块进行chebi的编号的匹配，
    ''' (由于有同分异构体之类的存在，所以即使化学式或者分子质量相同，也会匹配出几种不同的化合物，
    ''' 所以<see cref="[NameOf].MatchByFormula"/>以及<see cref="[NameOf].MatchByMass(Double, Double)"/>
    ''' 这两个方法应该是优先级别最低的)
    ''' </summary>
    Public Class [NameOf]

        ''' <summary>
        ''' 键名都是小写的
        ''' </summary>
        Dim names As Dictionary(Of String, Tables.Names())
        ''' <summary>
        ''' ``chebi -> xrefs``
        ''' </summary>
        Dim chebiXrefs As Dictionary(Of String, Tables.Accession())
        Dim DbXrefs As Dictionary(Of AccessionTypes, Tables.Accession())
        ''' <summary>
        ''' 因为存在同分异构体，所以这里是一对多的关系
        ''' </summary>
        Dim formulas As Dictionary(Of String, Tables.ChemicalData())
        Dim masses As DoubleTagged(Of Tables.ChemicalData())()

        Sub New(table As TSVTables)
            Dim accIDs = table.GetDatabaseAccessions
            Dim names = table.GetNames
            Dim chemicals = table.GetChemicalData
            Dim accessionTypes As Dictionary(Of String, AccessionTypes) =
                Enums(Of AccessionTypes) _
                .ToDictionary(Function(t) t.Description)

            Me.formulas = chemicals _
                .Where(Function(c) c.TYPE = "FORMULA") _
                .GroupBy(Function(x) x.CHEMICAL_DATA) _
                .ToDictionary(Function(x) x.Key,
                              Function(g) g.ToArray)
            Me.masses = chemicals _
                .Where(Function(c) c.TYPE = "MASS") _
                .GroupBy(Function(x) Val(x.CHEMICAL_DATA)) _
                .Select(Function(t)
                            Return New DoubleTagged(Of Tables.ChemicalData()) With {
                                .Tag = t.Key,
                                .value = t.ToArray
                            }
                        End Function) _
                .OrderBy(Function(x) x.Tag) _
                .ToArray
            Me.names = names _
                .GroupBy(Function(name) name.NAME.ToLower) _
                .ToDictionary(Function(k) k.Key,
                              Function(g) g.ToArray)
            Me.chebiXrefs = accIDs _
                .GroupBy(Function(acc) acc.COMPOUND_ID) _
                .ToDictionary(Function(k) k.Key,
                              Function(g) g.ToArray)
            Me.DbXrefs = accIDs _
                .Select(Function(id)
                            Return New Tuple(Of AccessionTypes, Tables.Accession)(accessionTypes(id.TYPE), id)
                        End Function) _
                .GroupBy(Function(t) t.Item1) _
                .ToDictionary(Function(k) k.Key,
                              Function(g) g.Select(
                              Function(id) id.Item2).ToArray)
        End Sub

        ''' <summary>
        ''' 2. 直接匹配名称(使用默认的<paramref name="fuzzy"/>=False参数)也比较精确
        ''' </summary>
        ''' <param name="name$"></param>
        ''' <param name="fuzzy"></param>
        ''' <returns></returns>
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

        ''' <summary>
        ''' 1. 最精确的方法
        ''' </summary>
        ''' <param name="ID$"></param>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Public Function MatchByID(ID$, type As AccessionTypes) As Tables.Accession()
            Dim list As Tables.Accession() = DbXrefs(type)
            For Each accID As Tables.Accession In list
                If ID.TextEquals(accID.ACCESSION_NUMBER) Then
                    Dim chebi = accID.COMPOUND_ID
                    Dim result = chebiXrefs(chebi)
                    Return result
                End If
            Next
            Return {}
        End Function

        Public Function MatchByIDs(IDs$(), type As AccessionTypes) As Tables.Accession()
            Return IDs _
                .Select(Function(id) MatchByID(id, type)) _
                .IteratesALL _
                .Distinct() _
                .ToArray
        End Function

#Region "由于有同分异构体之类的存在，所以即使化学式或者分子质量相同，也会匹配出几种不同的化合物，所以这两个方法应该是优先级别最低的"

        ''' <summary>
        ''' 3. 会出现一系列的同分异构体或者其他的具备有相同原子组成的化合物
        ''' </summary>
        ''' <param name="formula$"></param>
        ''' <returns></returns>
        Public Function MatchByFormula(formula$) As Tables.Accession()
            If formulas.ContainsKey(formula) Then
                Dim ids = formulas(formula).Select(Function(f) f.COMPOUND_ID).Distinct.ToArray
                Dim accIDs As Tables.Accession() = ids _
                    .Where(AddressOf chebiXrefs.ContainsKey) _
                    .Select(Function(id) chebiXrefs(id)) _
                    .IteratesALL _
                    .ToArray
                Return accIDs
            Else
                Return {}
            End If
        End Function

        ''' <summary>
        ''' 4. 精准度最低
        ''' </summary>
        ''' <param name="mass#"></param>
        ''' <param name="deltaPPM#"></param>
        ''' <returns></returns>
        Public Function MatchByMass(mass#, Optional deltaPPM# = 1) As Tables.Accession()
            Dim enter As Boolean = True
            Dim result As New List(Of Tables.ChemicalData)

            For Each massGroup As DoubleTagged(Of Tables.ChemicalData()) In masses
                If Math.Abs(massGroup.Tag - mass) <= deltaPPM Then
                    enter = False
                    result += massGroup.value
                Else
                    ' 已经被设置过了，由于是经过升序排序了的，所以后面肯定不会满足要求了
                    If Not enter Then
                        Exit For
                    End If
                End If
            Next

            Dim ids As String() = result _
                .Select(Function(x) x.COMPOUND_ID) _
                .Distinct _
                .ToArray
            Dim accIDs As Tables.Accession() = ids _
                .Where(AddressOf chebiXrefs.ContainsKey) _
                .Select(Function(id) chebiXrefs(id)) _
                .IteratesALL _
                .ToArray

            Return accIDs
        End Function
#End Region
    End Class
End Namespace
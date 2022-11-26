#Region "Microsoft.VisualBasic::267ebb2355baf2a4d793ccf644d9e3cb, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\Database\DataModels\NameOf.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 323
    '    Code Lines: 241
    ' Comment Lines: 55
    '   Blank Lines: 27
    '     File Size: 13.85 KB


    '     Class [NameOf]
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __valueTable, ContainsChEBIid, FromDataDirectory, GetChEBINamesByID, GetChebiXrefs
    '                   GetChemicalDatas, GetInCHIData, MatchByFormula, MatchByID, MatchByIDs
    '                   MatchByMass, MatchByName, ToKEGG
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Similarity
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv
Imports stdNum = System.Math

Namespace Assembly.ELIXIR.EBI.ChEBI

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
        Dim chebiXrefs As Dictionary(Of String, Dictionary(Of AccessionTypes, Tables.Accession))
        Dim DbXrefs As Dictionary(Of AccessionTypes, Tables.Accession())
        ''' <summary>
        ''' 因为存在同分异构体，所以这里是一对多的关系
        ''' </summary>
        Dim formulas As Dictionary(Of String, Tables.ChemicalData())
        Dim masses As DoubleTagged(Of Tables.ChemicalData())()
        Dim chebiNames As Dictionary(Of String, Tables.Names())
        Dim chebiChemicalDatas As Dictionary(Of String, Dictionary(Of String, Tables.ChemicalData))
        Dim chebiInChI As Dictionary(Of String, Tables.InChI())

        Sub New(table As TSVTables)
            Dim accIDs = table.GetDatabaseAccessions
            Dim names = table.GetNames
            Dim chemicals = table.GetChemicalData
            Dim accessionTypes As Dictionary(Of String, AccessionTypes) =
                Enums(Of AccessionTypes) _
                .ToDictionary(Function(t) t.Description)

            Me.chebiInChI = table.GetInChI _
                .GroupBy(Function(m) m.CHEBI_ID) _
                .ToDictionary(Function(c) c.Key,
                              Function(g)
                                  Return g.ToArray
                              End Function)
            Me.formulas = chemicals _
                .Where(Function(c) c.TYPE = "FORMULA") _
                .GroupBy(Function(x) x.CHEMICAL_DATA) _
                .ToDictionary(Function(x) x.Key,
                              Function(g)
                                  Return g.ToArray
                              End Function)
            Me.masses = chemicals _
                .Where(Function(c) c.TYPE = "MASS") _
                .GroupBy(Function(x) Val(x.CHEMICAL_DATA)) _
                .Select(Function(t)
                            Return New DoubleTagged(Of Tables.ChemicalData()) With {
                                .Tag = t.Key,
                                .Value = t.ToArray
                            }
                        End Function) _
                .OrderBy(Function(x) x.Tag) _
                .ToArray
            Me.chebiChemicalDatas = chemicals _
                .GroupBy(Function(c) c.COMPOUND_ID) _
                .ToDictionary(Function(id) id.Key,
                              AddressOf __valueTable)
            Me.names = names _
                .GroupBy(Function(name) name.NAME.ToLower) _
                .ToDictionary(Function(k) k.Key,
                              Function(g) g.ToArray)
            Me.chebiNames = names _
                .GroupBy(Function(name) name.COMPOUND_ID) _
                .ToDictionary(Function(ID) ID.Key,
                              Function(nameList) nameList.ToArray)
            Me.chebiXrefs = accIDs _
                .GroupBy(Function(acc) acc.COMPOUND_ID) _
                .ToDictionary(Function(k) k.Key,
                              Function(g)
                                  Return g _
                                      .GroupBy(Function(db) db.TYPE) _
                                      .ToDictionary(Function(db) accessionTypes(db.Key),
                                                    Function(db) db.First)
                              End Function)
            Me.DbXrefs = accIDs _
                .Select(Function(id)
                            Return New Tuple(Of AccessionTypes, Tables.Accession)(accessionTypes(id.TYPE), id)
                        End Function) _
                .GroupBy(Function(t) t.Item1) _
                .ToDictionary(Function(k) k.Key,
                              Function(g)
                                  Return g.Select(Function(id) id.Item2).ToArray
                              End Function)
        End Sub

        Public Function GetInCHIData(chebiID$) As Tables.InChI()
            If chebiInChI.ContainsKey(chebiID) Then
                Return chebiInChI(chebiID)
            Else
                Return Nothing
            End If
        End Function

        Private Shared Function __valueTable(data As IGrouping(Of String, Tables.ChemicalData)) As Dictionary(Of String, Tables.ChemicalData)
            Dim g = data _
                .ToArray _
                .GroupBy(Function(x) x.TYPE) _
                .ToArray
            Dim out = g.ToDictionary(Function(t) t.Key, Function(t) t.First)
            Return out
        End Function

        Public Function ToKEGG(chebiID$) As String()
            Dim xrefs = GetChebiXrefs(chebiID)

            If xrefs Is Nothing Then
                Return Nothing
            Else
                With xrefs
                    Return {
                        .TryGetValue(AccessionTypes.KEGG_Compound),
                        .TryGetValue(AccessionTypes.KEGG_Drug),
                        .TryGetValue(AccessionTypes.KEGG_Glycan)
                    }.Where(Function(id) Not id Is Nothing) _
                     .Select(Function(x)
                                 Return x.ACCESSION_NUMBER
                             End Function) _
                     .ToArray
                End With
            End If
        End Function

        Public Function GetChebiXrefs(chebiID$) As Dictionary(Of AccessionTypes, Tables.Accession)
            If chebiXrefs.ContainsKey(chebiID) Then
                Return chebiXrefs(chebiID)
            Else
                Return Nothing
            End If
        End Function

        Public Function GetChemicalDatas(chebiID$) As Dictionary(Of String, Tables.ChemicalData)
            If chebiChemicalDatas.ContainsKey(chebiID) Then
                Return chebiChemicalDatas(chebiID)
            Else
                Return Nothing
            End If
        End Function

        Public Shared Function FromDataDirectory(DIR$) As [NameOf]
            Dim tsv As New TSVTables(DIR)
            Dim names As New [NameOf](tsv)
            Return names
        End Function

        ''' <summary>
        ''' 目标<paramref name="chebiID"/>是否存在于这个名称mapping数据表之中
        ''' </summary>
        ''' <param name="chebiID$">
        ''' ChEBI代谢物数据库编号，这里要求是纯数字的，不带有``CHEBI``前缀
        ''' </param>
        ''' <returns></returns>
        Public Function ContainsChEBIid(chebiID$) As Boolean
            Return chebiXrefs.ContainsKey(chebiID)
        End Function

        ''' <summary>
        ''' 通过chebi编号从names数据之中查找得到名称列表
        ''' </summary>
        ''' <param name="chebi_ID$">单纯的数值格式的ChEBI编号</param>
        ''' <returns></returns>
        Public Function GetChEBINamesByID(chebi_ID$) As String()
            If chebiNames.ContainsKey(chebi_ID) Then
                Return chebiNames(chebi_ID) _
                    .Where(Function(name) name.TYPE = "SYNONYM" And name.SOURCE = "ChEBI") _
                    .Select(Function(name) name.NAME) _
                    .ToArray
            Else
                Return {}
            End If
        End Function

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
                        .Select(Function(id) chebiXrefs(id.COMPOUND_ID).Values) _
                        .IteratesALL _
                        .ToArray
                    Return out
                End Function

            If name.StringEmpty Then
                Return {}
            Else
                name = name.ToLower
            End If

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
        ''' <returns>
        ''' 这个函数会返回所有和目标ID能够关联上的编号列表，假若只想要得到chebi编号，
        ''' 则可以通过<paramref name="chebi"/>函数参数来获得
        ''' </returns>
        Public Function MatchByID(ID$, type As AccessionTypes, Optional ByRef chebi$ = Nothing) As Tables.Accession()
            Dim list As Tables.Accession() = DbXrefs(type)

            If ID.StringEmpty Then
                ' 空的编号字符串，则肯定没有结果
                Return {}
            End If

            For Each accID As Tables.Accession In list
                If ID.TextEquals(accID.ACCESSION_NUMBER) Then
                    With accID
                        Dim result = chebiXrefs(.COMPOUND_ID)
                        chebi = .COMPOUND_ID
                        Return result _
                            .Values _
                            .ToArray
                    End With
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
                    .Select(Function(id) chebiXrefs(id).Values) _
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
                If stdNum.Abs(massGroup.Tag - mass) <= deltaPPM Then
                    enter = False
                    result += massGroup.Value
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
                .Select(Function(id) chebiXrefs(id).Values) _
                .IteratesALL _
                .ToArray

            Return accIDs
        End Function
#End Region
    End Class
End Namespace

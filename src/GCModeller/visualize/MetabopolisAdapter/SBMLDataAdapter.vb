#Region "MetabopolisAdapter: SBML data source"

' ============================================================================
' SBML 数据源适配器
' ----------------------------------------------------------------------------
' 把 SBML 文件（Level 3）的代谢网络转换成 Metabopolis 的 MetabolicNetwork 模型：
'   * 用 SBML 库的 Level3.XmlFile(Of Reaction).LoadDocument 读入文档；
'   * 代谢物取 listOfSpecies，反应取 listOfReactions，方向由 reversible 决定；
'   * 类别取 compartment —— BioCyc 导出的 SBML 文件没有 groups 分组信息，
'     因此改用「反应涉及的 species 所在的 compartment」作为类别；
'     跨膜运输反应会把两个 compartment 拼成 "c+p" 这样的类别名；
'   * 为容错，任何被反应引用但未在 listOfSpecies 中出现的物种都会补一个占位代谢物，
'     避免因 id 转义差异导致整张网络结构断裂。
' ============================================================================

Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic.Linq
Imports Metabopolis.Model
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Model.SBML.Level3
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' SBML 适配器选项。
''' </summary>
Public Class SBMLAdapterOptions

    ''' <summary>最多转换的反应数量；0 表示不限制。</summary>
    Public Property MaxReactions As Integer = 0

    ''' <summary>是否跳过既无反应物又无产物的反应。</summary>
    Public Property SkipEmptyReactions As Boolean = True

    ''' <summary>无法判定 compartment 时的兜底类别编号。</summary>
    Public Property FallbackCompartment As String = "UNASSIGNED"

End Class

''' <summary>
''' SBML 文件 -&gt; Metabopolis 网络模型。
''' </summary>
Public Module SBMLDataAdapter

    ''' <summary>
    ''' 从 SBML 文件加载代谢网络。
    ''' </summary>
    Public Function Load(sbmlFile As String, Optional options As SBMLAdapterOptions = Nothing) As MetabolicNetwork
        If String.IsNullOrEmpty(sbmlFile) Then
            Throw New ArgumentNullException(NameOf(sbmlFile))
        End If

        If Not File.Exists(sbmlFile) Then
            Throw New FileNotFoundException($"SBML file not found: {sbmlFile}", sbmlFile)
        End If

        Dim opts As SBMLAdapterOptions = If(options, New SBMLAdapterOptions())
        Dim xml As String = File.ReadAllText(sbmlFile)
        Dim document As XmlFile(Of Reaction) = XmlFile(Of Reaction).LoadDocument(xml)

        If document Is Nothing OrElse document.model Is Nothing Then
            Throw New InvalidOperationException($"unable to parse SBML document: {sbmlFile}")
        End If

        Dim model As Model(Of Reaction) = document.model
        Dim compartmentNames As New Dictionary(Of String, String)(StringComparer.Ordinal)

        For Each item As compartment In model.listOfCompartments.SafeQuery
            If item Is Nothing OrElse String.IsNullOrEmpty(item.id) Then
                Continue For
            End If

            compartmentNames(item.id) = If(String.IsNullOrEmpty(item.name), item.id, item.name)
        Next

        ' ---- 代谢物 ----
        Dim compounds As New List(Of MetabolicCompound)()
        Dim compartmentOfSpecies As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim seen As New HashSet(Of String)(StringComparer.Ordinal)

        For Each specie As species In model.listOfSpecies.SafeQuery
            If specie Is Nothing OrElse String.IsNullOrEmpty(specie.id) OrElse Not seen.Add(specie.id) Then
                Continue For
            End If

            Dim compartment As String = specie.compartmentId

            If String.IsNullOrEmpty(compartment) Then
                compartment = InferCompartment(specie.id, compartmentNames)
            End If

            compartmentOfSpecies(specie.id) = compartment

            Dim xrefs As DBLink() = Nothing

            Try
                xrefs = specie.db_xrefs
            Catch ex As Exception
                xrefs = Nothing
            End Try

            compounds.Add(New MetabolicCompound With {
                .id = specie.id,
                .name = If(String.IsNullOrEmpty(specie.name), specie.id, specie.name),
                .xref = xrefs
            })
        Next

        ' ---- 反应 ----
        Dim reactions As New List(Of MetabolicReaction)()
        Dim categoryAssignment As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim errors As Integer = 0

        For Each rxn As Reaction In model.listOfReactions?.reactions.SafeQuery
            If rxn Is Nothing OrElse String.IsNullOrEmpty(rxn.id) Then
                Continue For
            End If

            Try
                Dim converted As MetabolicReaction = ConvertReaction(rxn)

                If converted Is Nothing Then
                    Continue For
                End If

                Dim hasLeft As Boolean = converted.left IsNot Nothing AndAlso converted.left.Length > 0
                Dim hasRight As Boolean = converted.right IsNot Nothing AndAlso converted.right.Length > 0

                If opts.SkipEmptyReactions AndAlso Not hasLeft AndAlso Not hasRight Then
                    Continue For
                End If

                reactions.Add(converted)
                categoryAssignment(converted.id) = CategoryOf(rxn, compartmentOfSpecies, opts.FallbackCompartment)

                ' 补齐被引用但未在 listOfSpecies 中声明的物种
                For Each reference As CompoundSpecieReference In converted.left.Concat(converted.right.SafeQuery)
                    If reference Is Nothing OrElse String.IsNullOrEmpty(reference.ID) Then
                        Continue For
                    End If

                    If seen.Add(reference.ID) Then
                        Dim compartment As String = InferCompartment(reference.ID, compartmentNames)
                        compartmentOfSpecies(reference.ID) = compartment
                        compounds.Add(New MetabolicCompound With {
                            .id = reference.ID,
                            .name = reference.ID
                        })
                    End If
                Next
            Catch ex As Exception
                errors += 1

                If errors <= 5 Then
                    Console.Error.WriteLine($"[sbml] {ex.Message}")
                End If
            End Try

            If opts.MaxReactions > 0 AndAlso reactions.Count >= opts.MaxReactions Then
                Exit For
            End If
        Next

        If errors > 5 Then
            Console.Error.WriteLine($"[sbml] skipped {errors} reactions due to conversion errors")
        End If

        Return MetabolicNetworkBuilder.Build(
            id:=Path.GetFileNameWithoutExtension(sbmlFile),
            name:=If(String.IsNullOrEmpty(model.name), Path.GetFileNameWithoutExtension(sbmlFile), model.name),
            source:=sbmlFile,
            compounds:=compounds,
            reactions:=reactions,
            categoryOf:=Function(rxn)
                            Dim hit As String = Nothing

                            If categoryAssignment.TryGetValue(rxn.id, hit) Then
                                Return hit
                            Else
                                Return Nothing
                            End If
                        End Function,
            categoryName:=Function(id) ResolveCategoryName(id, compartmentNames),
            fallbackCategory:=opts.FallbackCompartment)
    End Function

    ''' <summary>把 SBML 反应转换为内部标准反应模型。</summary>
    Public Function ConvertReaction(rxn As Reaction) As MetabolicReaction
        If rxn Is Nothing OrElse String.IsNullOrEmpty(rxn.id) Then
            Return Nothing
        End If

        Return New MetabolicReaction With {
            .id = rxn.id,
            .name = If(String.IsNullOrEmpty(rxn.name), rxn.id, rxn.name),
            .is_reversible = rxn.reversible,
            .is_spontaneous = False,
            .left = ToReferences(rxn.listOfReactants),
            .right = ToReferences(rxn.listOfProducts)
        }
    End Function

    Private Function ToReferences(list As List(Of SpeciesReference)) As CompoundSpecieReference()
        If list Is Nothing OrElse list.Count = 0 Then
            Return New CompoundSpecieReference() {}
        End If

        Dim result As New List(Of CompoundSpecieReference)(list.Count)

        For Each reference As SpeciesReference In list
            If reference Is Nothing OrElse String.IsNullOrEmpty(reference.species) Then
                Continue For
            End If

            result.Add(New CompoundSpecieReference(reference.stoichiometry, reference.species))
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' 反应所属类别：由反应涉及的物种所在 compartment 决定；
    ''' 跨 compartment 的反应把两个编号拼起来（例如 <c>c+p</c>）。
    ''' </summary>
    Private Function CategoryOf(rxn As Reaction,
                                compartmentOfSpecies As Dictionary(Of String, String),
                                fallback As String) As String

        Dim compartments As New List(Of String)()

        If Not String.IsNullOrEmpty(rxn.compartment) Then
            compartments.Add(rxn.compartment)
        End If

        For Each reference As SpeciesReference In rxn.listOfReactants.SafeQuery
            AppendCompartment(compartments, reference, compartmentOfSpecies)
        Next

        For Each reference As SpeciesReference In rxn.listOfProducts.SafeQuery
            AppendCompartment(compartments, reference, compartmentOfSpecies)
        Next

        Dim distinct As String() = compartments _
            .Where(Function(c) Not String.IsNullOrEmpty(c)) _
            .Distinct(StringComparer.Ordinal) _
            .OrderBy(Function(c) c, StringComparer.Ordinal) _
            .ToArray

        If distinct.Length = 0 Then
            Return fallback
        End If

        Return String.Join("+", distinct)
    End Function

    Private Sub AppendCompartment(compartments As List(Of String),
                                  reference As SpeciesReference,
                                  compartmentOfSpecies As Dictionary(Of String, String))

        If reference Is Nothing OrElse String.IsNullOrEmpty(reference.species) Then
            Return
        End If

        Dim compartment As String = Nothing

        If compartmentOfSpecies.TryGetValue(reference.species, compartment) Then
            compartments.Add(compartment)
        End If
    End Sub

    ''' <summary>
    ''' 由物种编号的后缀推断 compartment（例如 <c>LYS_c</c> -&gt; <c>c</c>）。
    ''' </summary>
    Private Function InferCompartment(speciesId As String, names As Dictionary(Of String, String)) As String
        If String.IsNullOrEmpty(speciesId) Then
            Return Nothing
        End If

        Dim index As Integer = speciesId.LastIndexOf("_"c)

        If index < 0 OrElse index = speciesId.Length - 1 Then
            Return Nothing
        End If

        Dim candidate As String = speciesId.Substring(index + 1)

        If names.ContainsKey(candidate) Then
            Return candidate
        End If

        Return candidate
    End Function

    ''' <summary>
    ''' 把类别编号（可能是 <c>c+p</c> 这样的组合）翻译成可读名称。
    ''' </summary>
    Private Function ResolveCategoryName(categoryId As String, names As Dictionary(Of String, String)) As String
        If String.IsNullOrEmpty(categoryId) Then
            Return Nothing
        End If

        Dim parts As String() = categoryId.Split("+"c)
        Dim display As New List(Of String)(parts.Length)

        For Each part As String In parts
            Dim hit As String = Nothing

            If names.TryGetValue(part, hit) Then
                display.Add(hit)
            Else
                display.Add(part)
            End If
        Next

        Return String.Join(" + ", display)
    End Function

End Module

#End Region

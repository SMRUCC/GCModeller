#Region "MetabopolisAdapter: BioCyc data source"

' ============================================================================
' BioCyc 数据源适配器
' ----------------------------------------------------------------------------
' 把 BioCyc PGDB（.dat 平面文件）的代谢网络转换成 Metabopolis 的
' MetabolicNetwork 模型：
'   * 代谢物 / 反应实体复用现有框架的
'     SMRUCC.genomics.MetabolicModel.{MetabolicCompound, MetabolicReaction}，
'     转换范式与 models\BioCyc\test\BioCycMetabolicConvertor.vb 保持一致；
'   * 反应方向一律取 reactions.equation（已按 REACTION-DIRECTION 归正后的
'     Reactants / Products），不要直接读 left/right 这两个原始槽位；
'   * 类别取反应的 IN-PATHWAY（首条通路视为主通路），
'     通路显示名从 pathways.dat 的 COMMON-NAME 解析。
' ============================================================================

Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic.Linq
Imports Metabopolis.Model
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' BioCyc 适配器选项。
''' </summary>
Public Class BioCycAdapterOptions

    ''' <summary>最多转换的反应数量；0 表示不限制。</summary>
    Public Property MaxReactions As Integer = 0

    ''' <summary>最多转换的代谢物数量；0 表示不限制。</summary>
    Public Property MaxCompounds As Integer = 0

    ''' <summary>是否跳过自发反应。</summary>
    Public Property SkipSpontaneous As Boolean = False

    ''' <summary>是否跳过没有 IN-PATHWAY 的反应（不归入任何通路）。</summary>
    Public Property SkipUnassigned As Boolean = False

End Class

''' <summary>
''' BioCyc PGDB -&gt; Metabopolis 网络模型。
''' </summary>
Public Module BioCycDataAdapter

    ''' <summary>
    ''' 从 BioCyc PGDB 目录加载代谢网络。
    ''' </summary>
    ''' <param name="workspaceDirectory">
    ''' PGDB 根目录（例如 <c>F:\ecoli\29.0</c>）或者直接指向 <c>data</c> 子目录。
    ''' </param>
    Public Function Load(workspaceDirectory As String, Optional options As BioCycAdapterOptions = Nothing) As MetabolicNetwork
        If String.IsNullOrEmpty(workspaceDirectory) Then
            Throw New ArgumentNullException(NameOf(workspaceDirectory))
        End If

        Dim root As String = ResolveDataDirectory(workspaceDirectory)
        Dim opts As BioCycAdapterOptions = If(options, New BioCycAdapterOptions())
        Dim name As String = New DirectoryInfo(root).Parent?.Name

        If String.IsNullOrEmpty(name) Then
            name = New DirectoryInfo(root).Name
        End If

        Dim ws As Workspace = Workspace.Open(root)

        If ws Is Nothing Then
            Throw New InvalidOperationException($"unable to open BioCyc PGDB at '{root}'")
        End If

        ' ---- 通路显示名 ----
        Dim pathwayNames As New Dictionary(Of String, String)(StringComparer.Ordinal)

        For Each pathway As pathways In ws.pathways.AsEnumerable
            If pathway Is Nothing OrElse String.IsNullOrEmpty(pathway.uniqueId) Then
                Continue For
            End If

            pathwayNames(pathway.uniqueId) = If(String.IsNullOrEmpty(pathway.commonName), pathway.uniqueId, pathway.commonName)
        Next

        ' ---- 代谢物 ----
        Dim compounds As New List(Of MetabolicCompound)()

        For Each cpd As compounds In ws.compounds.AsEnumerable
            Dim converted As MetabolicCompound = ConvertCompound(cpd)

            If converted IsNot Nothing Then
                compounds.Add(converted)
            End If

            If opts.MaxCompounds > 0 AndAlso compounds.Count >= opts.MaxCompounds Then
                Exit For
            End If
        Next

        ' ---- 反应 + 类别归属 ----
        Dim reactions As New List(Of MetabolicReaction)()
        Dim categoryOf As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim errors As Integer = 0

        For Each rxn As reactions In ws.reactions.AsEnumerable
            Dim primary As String = PrimaryPathway(rxn)

            If opts.SkipUnassigned AndAlso String.IsNullOrEmpty(primary) Then
                Continue For
            End If

            If opts.SkipSpontaneous AndAlso SafeBool(rxn.spontaneous) Then
                Continue For
            End If

            Try
                Dim converted As MetabolicReaction = ConvertReaction(rxn)

                If converted Is Nothing Then
                    Continue For
                End If

                reactions.Add(converted)
                categoryOf(converted.id) = primary
            Catch ex As Exception
                errors += 1

                If errors <= 5 Then
                    Console.Error.WriteLine($"[biocyc] {ex.Message}")
                End If
            End Try

            If opts.MaxReactions > 0 AndAlso reactions.Count >= opts.MaxReactions Then
                Exit For
            End If
        Next

        If errors > 5 Then
            Console.Error.WriteLine($"[biocyc] skipped {errors} reactions due to conversion errors")
        End If

        Return MetabolicNetworkBuilder.Build(
            id:=name,
            name:=name,
            source:=root,
            compounds:=compounds,
            reactions:=reactions,
            categoryOf:=Function(rxn)
                            Dim hit As String = Nothing

                            If categoryOf.TryGetValue(rxn.id, hit) Then
                                Return hit
                            Else
                                Return Nothing
                            End If
                        End Function,
            categoryName:=Function(id)
                             Dim display As String = Nothing

                             If pathwayNames.TryGetValue(id, display) Then
                                 Return display
                             Else
                                 Return Nothing
                             End If
                         End Function,
            fallbackCategory:="UNASSIGNED")
    End Function

    ''' <summary>
    ''' 解析 PGDB 数据目录（兼容直接传入 <c>data</c> 目录与传入上一级目录）。
    ''' </summary>
    Private Function ResolveDataDirectory(directory As String) As String
        If File.Exists(Path.Combine(directory, "reactions.dat")) Then
            Return directory
        End If

        Dim nested As String = Path.Combine(directory, "data")

        If File.Exists(Path.Combine(nested, "reactions.dat")) Then
            Return nested
        End If

        Return directory
    End Function

    ''' <summary>取反应的主通路（IN-PATHWAY 的第一条）。</summary>
    Private Function PrimaryPathway(rxn As reactions) As String
        Dim members As String() = rxn?.inPathway

        If members Is Nothing OrElse members.Length = 0 Then
            Return Nothing
        End If

        For Each item As String In members
            If Not String.IsNullOrEmpty(item) Then
                Return item
            End If
        Next

        Return Nothing
    End Function

    ''' <summary>把 BioCyc 化合物转换为内部标准代谢物模型。</summary>
    Public Function ConvertCompound(cpd As compounds) As MetabolicCompound
        If cpd Is Nothing OrElse String.IsNullOrEmpty(cpd.uniqueId) Then
            Return Nothing
        End If

        Return New MetabolicCompound With {
            .id = cpd.uniqueId,
            .name = If(String.IsNullOrEmpty(cpd.commonName), cpd.uniqueId, cpd.commonName),
            .synonym = cpd.synonyms,
            .formula = compounds.FormulaString(cpd),
            .moleculeWeight = cpd.molecularWeight,
            .xref = compounds.GetDbLinks(cpd).ToArray,
            .smiles = cpd.SMILES
        }
    End Function

    ''' <summary>
    ''' 把 BioCyc 反应转换为内部标准反应模型（left/right 已按 REACTION-DIRECTION 归正）。
    ''' </summary>
    Public Function ConvertReaction(rxn As reactions) As MetabolicReaction
        If rxn Is Nothing OrElse String.IsNullOrEmpty(rxn.uniqueId) Then
            Return Nothing
        End If

        Dim equation As Equation = rxn.equation

        If equation Is Nothing Then
            Return Nothing
        End If

        Dim result As New MetabolicReaction With {
            .id = rxn.uniqueId,
            .name = If(String.IsNullOrEmpty(rxn.commonName), If(String.IsNullOrEmpty(rxn.systematicName), rxn.uniqueId, rxn.systematicName), rxn.commonName),
            .description = rxn.comment,
            .is_reversible = (rxn.reactionDirection = ReactionDirections.Reversible),
            .is_spontaneous = SafeBool(rxn.spontaneous),
            .left = equation.Reactants,
            .right = equation.Products
        }

        If rxn.ec_number IsNot Nothing Then
            result.ECNumbers = rxn.ec_number.Select(Function(ec) ec.ToString()).ToArray
        End If

        If Not Double.IsNaN(rxn.gibbs0) AndAlso Not Double.IsInfinity(rxn.gibbs0) Then
            result.gibbs = rxn.gibbs0
        End If

        Return result
    End Function

    ''' <summary>
    ''' 防御式布尔转换：部分槽位在缺失时是空串，反射绑定可能给出非预期类型。
    ''' </summary>
    Private Function SafeBool(value As Object) As Boolean
        If value Is Nothing Then
            Return False
        End If

        Try
            If TypeOf value Is Boolean Then
                Return CBool(value)
            End If

            Return Boolean.Parse(value.ToString())
        Catch ex As Exception
            Return False
        End Try
    End Function

End Module

#End Region

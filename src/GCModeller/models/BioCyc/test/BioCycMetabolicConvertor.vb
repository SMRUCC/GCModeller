' ============================================================================
' BioCycMetabolicConvertor.vb — BioCyc PGDB → GCModeller 内部代谢模型 转换器
' ----------------------------------------------------------------------------
' 仿照 core\Bio.Assembly\MetabolicModel\KEGGConvertor.vb 的写法，把 BioCyc 的
' compounds / reactions 转换成 SMRUCC.genomics.MetabolicModel 下的
' MetabolicCompound / MetabolicReaction，用于演示"任意数据源 → 内部标准模型 →
' MetabolicAdapter"的完整通路。
'
' 方向处理：BioCyc 的 REACTION-DIRECTION 归正发生在 rxn.equation 属性里，这里直接
' 取 equation.Reactants / equation.Products 作为内部模型的 left / right，转换得到的
' MetabolicReaction 因此已经是"已归正"的形态。
' ============================================================================

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism
Imports SMRUCC.genomics.MetabolicModel

Public Module BioCycMetabolicConvertor

    ''' <summary>
    ''' 把 BioCyc 的 compounds.dat 条目转换为内部代谢化合物模型。
    ''' </summary>
    ''' <param name="cpd">BioCyc 化合物对象。</param>
    ''' <returns>内部标准模型对象；输入为 Nothing 时返回 Nothing。</returns>
    Public Function ConvertCompound(cpd As compounds) As MetabolicCompound
        If cpd Is Nothing Then Return Nothing

        Return New MetabolicCompound With {
            .id = cpd.uniqueId,
            .name = If(cpd.commonName, cpd.uniqueId),
            .synonym = cpd.synonyms,
            .formula = compounds.FormulaString(cpd),
            .moleculeWeight = cpd.molecularWeight,
            .xref = compounds.GetDbLinks(cpd).ToArray,
            .smiles = cpd.SMILES
        }
    End Function

    ''' <summary>
    ''' 把 BioCyc 的 reactions.dat 条目转换为内部代谢反应模型。
    ''' </summary>
    ''' <param name="rxn">BioCyc 反应对象。</param>
    ''' <returns>内部标准模型对象（left/right 已按 REACTION-DIRECTION 归正）。</returns>
    Public Function ConvertReaction(rxn As reactions) As MetabolicReaction
        If rxn Is Nothing Then Return Nothing

        Dim eq As Equation = rxn.equation
        Dim m As New MetabolicReaction()
        Dim stepName As String = "id"

        Try
            stepName = "id" : m.id = rxn.uniqueId
            stepName = "name"
            m.name = rxn.commonName
            If String.IsNullOrEmpty(m.name) Then m.name = rxn.systematicName
            If String.IsNullOrEmpty(m.name) Then m.name = rxn.uniqueId
            stepName = "description" : m.description = rxn.comment
            stepName = "ECNumbers"
            m.ECNumbers = If(rxn.ec_number Is Nothing,
                             Nothing,
                             rxn.ec_number.Select(Function(ec) ec.ToString()).ToArray)
            stepName = "is_reversible" : m.is_reversible = (rxn.reactionDirection = ReactionDirections.Reversible)
            stepName = "is_spontaneous" : m.is_spontaneous = SafeBool(rxn.spontaneous)
            stepName = "gibbs" : m.gibbs = If(Double.IsNaN(rxn.gibbs0) OrElse Double.IsInfinity(rxn.gibbs0), 0, rxn.gibbs0)
            stepName = "left" : m.left = eq.Reactants
            stepName = "right" : m.right = eq.Products
        Catch ex As Exception
            Throw New Exception($"{rxn.uniqueId} @ {stepName}: {ex.Message}", ex)
        End Try

        Return m
    End Function

    ''' <summary>
    ''' 防御式布尔转换：BioCyc 的部分布尔槽位（如 SPONTANEOUS?）在缺失时是空串，
    ''' 反射绑定可能给出非预期的运行时类型，这里统一兜底为 False，不让单条脏数据中断整库转换。
    ''' </summary>
    Private Function SafeBool(value As Object) As Boolean
        If value Is Nothing Then Return False

        Try
            If TypeOf value Is Boolean Then Return CBool(value)
            Return Boolean.Parse(value.ToString())
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 批量转换整库的化合物。
    ''' </summary>
    Public Function GetCompounds(biocyc As Workspace) As List(Of MetabolicCompound)
        Return biocyc.compounds _
            .AsEnumerable _
            .Select(Function(c) ConvertCompound(c)) _
            .Where(Function(c) c IsNot Nothing) _
            .ToList
    End Function

    ''' <summary>
    ''' 批量转换整库的反应。单条脏数据只跳过并计数（前几条会打印原因），不让装配中断。
    ''' </summary>
    Public Function GetReactions(biocyc As Workspace) As List(Of MetabolicReaction)
        Dim list As New List(Of MetabolicReaction)()
        Dim errors As Integer = 0

        For Each rxn As reactions In biocyc.reactions.AsEnumerable
            Try
                Dim converted As MetabolicReaction = ConvertReaction(rxn)
                If converted IsNot Nothing Then list.Add(converted)
            Catch ex As Exception
                errors += 1
                If errors <= 5 Then
                    Console.Error.WriteLine($"[convert] {ex.Message}")
                End If
            End Try
        Next

        If errors > 5 Then
            Console.Error.WriteLine($"[convert] ... 共跳过 {errors} 条无法转换的反应")
        End If

        Return list
    End Function

End Module

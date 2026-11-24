Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

''' <summary>
''' 负责把 BioCyc 数据库之中的 CCO 区室编号映射为虚拟细胞模型之中的区室定义。
''' 
''' 旧版本的编译器仅仅使用一个硬编码的字符串表将 ``CCO-OUT`` 映射为 ``Extracellular``，
''' 其余的区室全部归并到细胞之内，导致周质空间以及细胞膜等关键结构信息丢失。
''' 本编译器会为每一个出现过的区室建立完整的区室定义，并将映射关系写入模型文件。
''' </summary>
Public Class CompartmentCompiler

    ''' <summary>
    ''' 细胞之内(细胞质基质)的区室编号，通常为细胞的分类学编号
    ''' </summary>
    Public ReadOnly Property cytosol As String

    ReadOnly m_compartments As New Dictionary(Of String, Compartment)
    ReadOnly m_alias As New Dictionary(Of String, String)

    Sub New(cellular_id As String)
        Me.cytosol = If(cellular_id.StringEmpty(, True), "Intracellular", cellular_id)

        Call register("CCO-CYTOSOL", Me.cytosol, CompartmentKind.intracellular, volume:=2.3, pH:=7.6)
        Call register("CCO-IN", Me.cytosol, CompartmentKind.intracellular, volume:=2.3, pH:=7.6)
        Call register("CCI-CYTOSOL", Me.cytosol, CompartmentKind.intracellular, volume:=2.3, pH:=7.6)
        Call register("CCI-CYTOSOL-GN", Me.cytosol, CompartmentKind.intracellular, volume:=2.3, pH:=7.6)

        Call register("CCO-PERIPLASM", "Periplasm", CompartmentKind.periplasm, volume:=0.3, pH:=7.0)
        Call register("CCO-PERI-B", "Periplasm", CompartmentKind.periplasm, volume:=0.3, pH:=7.0)
        Call register("CCO-PERI-BAC", "Periplasm", CompartmentKind.periplasm, volume:=0.3, pH:=7.0)
        Call register("CCO-PERI-BAC-GN", "Periplasm", CompartmentKind.periplasm, volume:=0.3, pH:=7.0)
        Call register("CCO-PERI-BAC-PERI-BAC", "Periplasm", CompartmentKind.periplasm, volume:=0.3, pH:=7.0)
        Call register("CCO-MIDDLE", "Periplasm", CompartmentKind.periplasm, volume:=0.3, pH:=7.0)

        Call register("CCO-CYTOPLASMIC-MEMBRANE", "InnerMembrane", CompartmentKind.innerMembrane, volume:=0, pH:=7.0)
        Call register("CCO-INNER-MEMBRANE", "InnerMembrane", CompartmentKind.innerMembrane, volume:=0, pH:=7.0)
        Call register("CCO-MEMBRANE", "InnerMembrane", CompartmentKind.innerMembrane, volume:=0, pH:=7.0)
        Call register("CCO-PM-BAC-NEG", "InnerMembrane", CompartmentKind.innerMembrane, volume:=0, pH:=7.0)
        Call register("CCO-PM-BAC-NEG-GN", "InnerMembrane", CompartmentKind.innerMembrane, volume:=0, pH:=7.0)
        Call register("CCO-PM-BAC-ACT", "InnerMembrane", CompartmentKind.innerMembrane, volume:=0, pH:=7.0)

        Call register("CCO-OUTER-MEMBRANE", "OuterMembrane", CompartmentKind.outerMembrane, volume:=0, pH:=7.0)
        Call register("CCO-OUTER-MEM", "OuterMembrane", CompartmentKind.outerMembrane, volume:=0, pH:=7.0)

        Call register("CCO-EXTRACELLULAR", "Extracellular", CompartmentKind.extracellular, volume:=0, pH:=7.0)
        Call register("CCO-OUT", "Extracellular", CompartmentKind.extracellular, volume:=0, pH:=7.0)
    End Sub

    Private Sub register(id As String, compartmentId As String, kind As CompartmentKind, volume As Double, pH As Double)
        Call m_alias.Add(id, compartmentId)

        If Not m_compartments.ContainsKey(compartmentId) Then
            Call m_compartments.Add(compartmentId, New Compartment With {
                .id = compartmentId,
                .name = compartmentId,
                .kind = kind,
                .volume = volume,
                .pH = pH
            })
        End If
    End Sub

    ''' <summary>
    ''' 模型文件之中所定义的全部区室
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Compartments As Compartment()
        Get
            Return m_compartments.Values.ToArray
        End Get
    End Property

    ''' <summary>
    ''' 将BioCyc的区室编号映射为模型文件之中的区室编号。
    ''' 
    ''' 对于未知的区室编号，会按照编号的前缀推断出区室的类型，并且动态注册一个新的区室定义，
    ''' 从而避免丢失数据
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function MapCompartId(id As String) As String
        If id.StringEmpty(, True) Then
            Return cytosol
        End If

        Dim key As String = id.Trim()
        Dim mapped As String = Nothing

        If m_alias.TryGetValue(key, value:=mapped) Then
            Return mapped
        End If

        ' 对于数据文件之中出现的新的区室编号，按照关键字推断区室的类型
        Dim kind As CompartmentKind = ResolveKind(key)
        Dim newId As String = normalize(key, kind)

        Call m_alias.Add(key, newId)

        If Not m_compartments.ContainsKey(newId) Then
            Call m_compartments.Add(newId, New Compartment With {
                .id = newId,
                .name = key,
                .kind = kind,
                .pH = 7.0
            })
        End If

        Return newId
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function MapCompartId(id As String()) As String()
        Return id.SafeQuery.Select(AddressOf MapCompartId).Distinct.ToArray
    End Function

    Private Shared Function ResolveKind(id As String) As CompartmentKind
        Dim text As String = id.ToUpperInvariant

        If text.Contains("EXTRACELLULAR") OrElse text.EndsWith("-OUT") Then
            Return CompartmentKind.extracellular
        ElseIf text.Contains("PERIPLASM") OrElse text.Contains("PERI-BAC") OrElse text.EndsWith("-MIDDLE") Then
            Return CompartmentKind.periplasm
        ElseIf text.Contains("OUTER-MEM") Then
            Return CompartmentKind.outerMembrane
        ElseIf text.Contains("MEMBRANE") OrElse text.Contains("PM-BAC") Then
            Return CompartmentKind.innerMembrane
        Else
            Return CompartmentKind.intracellular
        End If
    End Function

    Private Shared Function normalize(id As String, kind As CompartmentKind) As String
        Select Case kind
            Case CompartmentKind.extracellular
                Return "Extracellular"
            Case CompartmentKind.periplasm
                Return "Periplasm"
            Case CompartmentKind.outerMembrane
                Return "OuterMembrane"
            Case CompartmentKind.innerMembrane
                Return "InnerMembrane"
            Case Else
                Return id.Replace("CCO-", "").Replace("CCI-", "")
        End Select
    End Function

End Class

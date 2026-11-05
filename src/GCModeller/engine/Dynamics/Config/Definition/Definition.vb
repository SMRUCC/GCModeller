#Region "Microsoft.VisualBasic::ea998afaf6bd88a0e584baba06e6af04, engine\BootstrapLoader\Definition\Definition.vb"

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

'   Total Lines: 269
'    Code Lines: 191 (71.00%)
' Comment Lines: 50 (18.59%)
'    - Xml Docs: 92.00%
' 
'   Blank Lines: 28 (10.41%)
'     File Size: 9.91 KB


'     Enum GeneralCompound
' 
'         Compound, DNA, Protein, RNA
' 
'  
' 
' 
' 
'     Class Transmembrane
' 
'         Properties: passive
' 
'     Class Definition
' 
'         Properties: ADP, AminoAcid, Ammonia, ATP, CultureMedium
'                     GDP, GenericCompounds, GTP, NucleicAcid, Oxygen
'                     PI, PPI, status, transmembrane, Water
' 
'         Function: GenericEnumerator, GetInfinitySource, initMassValue, KEGG, MetaCyc
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf
Imports Microsoft.VisualBasic.Linq

Namespace Definitions

    ''' <summary>
    ''' The definition for the mass environment for run the simulation, use for context mapping across database
    ''' </summary>
    ''' <remarks>
    ''' 因为物质编号可能会来自于不同的数据库，所以会需要使用这个对象将一些关键的物质映射为计算引擎所能够被识别的对象
    ''' </remarks>
    ''' 
    <ClassName("define")>
    Public Class Definition : Implements Enumeration(Of String)

#Region "Object maps"

        ' 当初主要是使用这种固定的映射来处理一些特定的模板事件

#Region "转录和翻译"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property ATP As String
        Public Property ADP As String
        Public Property PPI As String
        Public Property PI As String

        Public Property GTP As String
        Public Property GDP As String

#End Region

#Region "模板或者说无限来源的物质"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property Water As String
        Public Property Oxygen As String
        Public Property Ammonia As String
#End Region

        ''' <summary>
        ''' nucleotide metabolite id mapping for construct of the RNA molecule in the transcription process.
        ''' </summary>
        ''' <returns></returns>
        Public Property NucleicAcid As NucleicAcid
        ''' <summary>
        ''' amino acid metabolite id mapping for construct of the polypeptide in translation process
        ''' </summary>
        ''' <returns></returns>
        Public Property AminoAcid As AminoAcid

        ''' <summary>
        ''' define the id mapping to some general compounds
        ''' </summary>
        ''' <returns></returns>
        Public Property GenericCompounds As Dictionary(Of String, GeneralCompound)
#End Region

#Region "培养基与胞外环境"

        ''' <summary>
        ''' 默认培养基配方。
        ''' 
        ''' 一个组分由「角色(role) + 数据库化合物编号(id) + 默认浓度(mM)」构成，
        ''' 因此同一个配方可以在 MetaCyc / BioCyc / KEGG 之间切换而无需改动调用代码。
        ''' 模拟程序在未指定培养基文件的时候，会按本表初始化胞外环境，并且只保留
        ''' 模型之中真实存在的化合物编号。
        ''' </summary>
        ''' <returns></returns>
        Public Property CultureMediumComponents As MediumComponent()

        ''' <summary>
        ''' 矿物离子映射：<c>[离子符号 => 化合物编号]</c>
        ''' 
        ''' 常用键：K、Na、Mg、Ca、Cl、Fe2、Fe3、Zn、Mn、Cu、Co、Ni、Mo、H、OH
        ''' </summary>
        ''' <returns></returns>
        Public Property Ions As Dictionary(Of String, String)

        ''' <summary>
        ''' 氧化还原载体与酶辅因子映射：<c>[辅因子名称 => 化合物编号]</c>
        ''' 
        ''' 常用键：NAD、NADH、NADP、NADPH、FAD、FADH2、CO-A、ACETYL-COA、
        ''' THIAMINE-PP、PYRIDOXAL-P、COBALAMIN
        ''' </summary>
        ''' <returns></returns>
        Public Property Cofactors As Dictionary(Of String, String)

        ''' <summary>
        ''' 分泌产物 / 废物映射：<c>[产物名称 => 化合物编号]</c>
        ''' 
        ''' 常用键：CO2、HCO3、LACTATE、ETHANOL、FORMATE、ACETATE、H2、H2S、NH3
        ''' </summary>
        ''' <returns></returns>
        Public Property WasteProducts As Dictionary(Of String, String)

        ''' <summary>
        ''' 按离子符号读取化合物编号（不存在时返回 Nothing）
        ''' </summary>
        ''' <param name="symbol"></param>
        ''' <returns></returns>
        Public Function GetIon(symbol As String) As String
            Return lookup(Ions, symbol)
        End Function

        ''' <summary>
        ''' 按辅因子名称读取化合物编号（不存在时返回 Nothing）
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function GetCofactor(name As String) As String
            Return lookup(Cofactors, name)
        End Function

        ''' <summary>
        ''' 按产物名称读取化合物编号（不存在时返回 Nothing）
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function GetWasteProduct(name As String) As String
            Return lookup(WasteProducts, name)
        End Function

        Private Shared Function lookup(table As Dictionary(Of String, String), key As String) As String
            Dim value As String = Nothing

            If table Is Nothing OrElse key.StringEmpty(, True) Then
                Return Nothing
            End If

            If table.TryGetValue(key, value) Then
                Return value
            Else
                Return Nothing
            End If
        End Function

#End Region

        Public Property transmembrane As Transmembrane

        ''' <summary>
        ''' the compartment id of the Culture medium
        ''' </summary>
        ''' <returns></returns>
        Public Property CultureMedium As String = "Extracellular"

        ''' <summary>
        ''' define of the cell initial status.
        ''' </summary>
        ''' <returns>
        ''' [cell_id => [metabolite => content data]]
        ''' </returns>
        Public Property context As CellContext()

        ''' <summary>
        ''' load from ini file
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <summary>
        ''' 从 ini 文件加载上下文映射定义
        ''' </summary>
        ''' <param name="file">
        ''' ini 文件路径。为空或者文件不存在的时候返回 <c>Nothing</c>，
        ''' 由调用方决定使用哪一个数据库的内置映射作为回退
        ''' </param>
        ''' <returns></returns>
        Public Shared Function LoadIni(file As String) As Definition
            If file.StringEmpty(, True) Then
                Return Nothing
            End If
            If Not file.FileExists(True) Then
                Return Nothing
            End If

            Return ClassMapper.LoadIni(Of Definition)(file)
        End Function

        ''' <summary>
        ''' Save to ini file
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function SaveIni(file As String) As Boolean
            Return ClassMapper.WriteClass(Me, file)
        End Function

        ''' <summary>
        ''' 无限来源的物质：这些物质在模拟过程之中不会被消耗殆尽
        ''' （水、氧气、二氧化碳以及氨/铵）。
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetInfinitySource() As Index(Of String)
            Return {
                Water,
                Oxygen,
                GetWasteProduct("CO2"),
                If(Ammonia, GetWasteProduct("NH3"))
            }.Where(Function(ref) Not ref.StringEmpty(, True)).Indexing
        End Function

        ''' <summary>
        ''' 枚举定义之中所引用的全部化合物编号（自动去重）
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GenericEnumerator() As IEnumerator(Of String) Implements Enumeration(Of String).GenericEnumerator
            Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each id As String In enumerateAll()
                If Not id.StringEmpty(, True) AndAlso seen.Add(id) Then
                    Yield id
                End If
            Next
        End Function

        Private Iterator Function enumerateAll() As IEnumerable(Of String)
            Yield ADP
            Yield ATP
            Yield PPI
            Yield PI
            Yield GTP
            Yield GDP

            Yield Water
            Yield Oxygen
            Yield Ammonia

            If Not GenericCompounds Is Nothing Then
                For Each key As String In GenericCompounds.Keys
                    Yield key
                Next
            End If

            If Not CultureMediumComponents Is Nothing Then
                For Each component As MediumComponent In CultureMediumComponents
                    If Not component Is Nothing Then
                        Yield component.id
                    End If
                Next
            End If

            If Not Ions Is Nothing Then
                For Each id As String In Ions.Values
                    Yield id
                Next
            End If

            If Not Cofactors Is Nothing Then
                For Each id As String In Cofactors.Values
                    Yield id
                Next
            End If

            If Not WasteProducts Is Nothing Then
                For Each id As String In WasteProducts.Values
                    Yield id
                Next
            End If

            If Not context Is Nothing Then
                For Each key As String In context.Keys
                    Yield key
                Next
            End If

            If Not NucleicAcid Is Nothing Then
                Yield NucleicAcid.A
                Yield NucleicAcid.C
                Yield NucleicAcid.G
                Yield NucleicAcid.U
                Yield NucleicAcid.T
            End If

            If Not AminoAcid Is Nothing Then
                For Each aa As String In AminoAcid.AsEnumerable
                    Yield aa
                Next
            End If
        End Function
    End Class
End Namespace

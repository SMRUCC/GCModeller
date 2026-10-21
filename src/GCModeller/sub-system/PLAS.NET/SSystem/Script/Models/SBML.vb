#Region "Microsoft.VisualBasic::f16775f7c4e457aa48df89b7daf384d1, sub-system\PLAS.NET\SSystem\Script\Models\SBML.vb"

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

    '   Total Lines: 166
    '    Code Lines: 109 (65.66%)
    ' Comment Lines: 31 (18.67%)
    '    - Xml Docs: 87.10%
    ' 
    '   Blank Lines: 26 (15.66%)
    '     File Size: 6.28 KB


    '     Class SBML
    ' 
    '         Properties: AutoFixError
    ' 
    '         Function: __contact, __generateSystem, __where, Compile, CompileImpl
    '                   GenerateFunction, PreCompile
    ' 
    '         Sub: __strip, __stripNumber
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels
Imports SMRUCC.genomics.Analysis.SSystem.Runner
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace Script

    ''' <summary>
    ''' SBML模型编译器
    ''' </summary>
    ''' <remarks>
    ''' 本编译器同时支持 SBML Level2 与 Level3 格式的文档：
    ''' Level2 走 <see cref="SMRUCC.genomics.Model.SBML.Level2.XmlFile"/>，
    ''' Level3 走 <see cref="SMRUCC.genomics.Model.SBML.Level3.XmlFile(Of SMRUCC.genomics.Model.SBML.Level3.Reaction)"/>，
    ''' 两种文档都会被规整化为同一个 <see cref="SbmlNetwork"/> 网络视图之后再做转换。
    ''' </remarks>
    Public Class SBML : Inherits Compiler(Of Model)

        ''' <summary>
        ''' The file path of the source sbml document.(输入的SBML文档的文件路径)
        ''' </summary>
        ''' <returns></returns>
        Public Property SBMLPath As String

        ''' <summary>
        ''' The loaded sbml network model.(已经加载完成的SBML网络视图)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Network As SbmlNetwork
            Get
                Return sbmlNetwork
            End Get
        End Property

        ''' <summary>
        ''' 当物种缺失初始量属性的时候所使用的默认初始量
        ''' </summary>
        ''' <returns></returns>
        Public Property InitialAmountFallback As Double = 1.0R

        ''' <summary>
        ''' 对缺失初始量的物种附加的确定性抖动幅度（相对比例）。
        ''' 由于幂律的生成项与消耗项会互相抵消，当所有的初始量完全一致的时候
        ''' 整个系统会恒等于零，故这里缺省会附加一个确定性的抖动量。
        ''' </summary>
        ''' <returns></returns>
        Public Property InitialJitter As Double = 0.01R

        Dim sbmlNetwork As SbmlNetwork

        ''' <summary>
        ''' 是否自动修复模型之中的错误
        ''' </summary>
        ''' <returns></returns>
        Public Property AutoFixError As Boolean = False

        Protected Overrides Function PreCompile(args As CommandLine) As Integer
            m_compiledModel = New Model
            sbmlNetwork = SBMLFileLoader.Load(SBMLPath)

            Return 0
        End Function

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer
            Call __generateSystem(m_compiledModel)

            m_compiledModel.Title = sbmlNetwork.ModelName
            m_compiledModel.FinalTime = 100

            Return 0
        End Function

        ''' <summary>
        ''' Generate the equation group of the target modelling system.(生成目标模型系统的方程组)
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这里使用<see cref="SbmlReactionIndex"/>反向索引，避免了对
        ''' 代谢物列表 x 反应列表的 O(n*m) 全线性扫描
        ''' </remarks>
        Private Function __generateSystem(model As Script.Model) As Boolean
            Dim index As New SbmlReactionIndex(sbmlNetwork)
            Dim reactions As New List(Of SEquation)
            Dim referenced As New HashSet(Of String)
            Dim metabolites As New List(Of var)

            ' 求解标准要求：方程 PST 之中每一行的 expression 由所有反应的反应物构成，
            ' 故这里直接使用反应物的集合作为被引用的符号集合
            For Each rxn As SbmlReaction In sbmlNetwork.Reactions
                For Each id As String In rxn.Reactants.SafeQuery
                    If Not String.IsNullOrEmpty(id) Then
                        Call referenced.Add(id)
                    End If
                Next
            Next

            For Each m As SbmlSpecies In sbmlNetwork.Species
                If Not index.HasReaction(m.Id) Then
                    Continue For
                End If

                Dim expression As String = GenerateFunction(m.Id, index)

                If String.IsNullOrEmpty(expression) Then
                    Continue For
                End If

                Call reactions.Add(New SEquation(m.Id, expression))
            Next

            For Each m As SbmlSpecies In sbmlNetwork.Species
                ' 除了被引用的代谢物之外，方程所指向的目标代谢物自身也必须要有初始量，
                ' 否则 ODE 求解器会因为缺失状态变量而把这些方程给丢掉
                If Not (referenced.Contains(m.Id) OrElse index.HasReaction(m.Id)) Then
                    Continue For
                End If

                Call metabolites.Add(New var With {
                    .Id = m.Id,
                    .title = m.Name,
                    .Value = __initialValue(m)
                })
            Next

            model.sEquations = reactions.ToArray
            model.Vars = metabolites.ToArray

            Return True
        End Function

        ''' <summary>
        ''' 计算目标代谢物的初始量，缺失初始量属性的时候使用配置对象所给定的默认值
        ''' </summary>
        ''' <param name="metabolite"></param>
        ''' <returns></returns>
        Private Function __initialValue(metabolite As SbmlSpecies) As Double
            If metabolite.HasInitialAmount AndAlso metabolite.InitialAmount > 0 Then
                Return metabolite.InitialAmount
            End If

            Return InitialAmountFallback * (1 + __deterministicJitter(metabolite.Id))
        End Function

        ''' <summary>
        ''' 依据代谢物的编号计算出一个确定性的抖动因子，取值区间为 [-InitialJitter, +InitialJitter]
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Private Function __deterministicJitter(id As String) As Double
            If InitialJitter <= 0 Then
                Return 0
            End If

            ' djb2 hash，每一步都取模以避免 VB 的整数溢出检查抛出异常
            Dim hash As Long = 5381

            For Each c As Char In id
                hash = ((hash << 5) + hash + AscW(c)) Mod 2147483647L
            Next

            Dim ratio As Double = (hash Mod 2001L) / 1000.0 - 1.0

            Return InitialJitter * ratio
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Metabolite"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' + 在计算消耗的部分的时候，S系统方程中当前的底物是作为反应物而被消耗掉的
        ''' + 在计算生成的部分的时候，S系统方程之中是消耗来源的反应物而生成的当前底物
        ''' 
        ''' 故而两个方向都是取值来源为反应物
        ''' </remarks>
        Private Function GenerateFunction(Metabolite As String, index As SbmlReactionIndex) As String
            Dim produce As String = __contact(index.GetProduce(Metabolite))
            Dim consume As String = __contact(index.GetConsume(Metabolite)) ' 在计算消耗的部分的时候，S系统方程中目标是作为反应物而被消耗掉的
            Dim eq As String = ""

            If Not String.IsNullOrEmpty(produce) Then
                eq = "0.5*" & produce
            End If
            If Not String.IsNullOrEmpty(consume) Then
                eq = eq & "-0.5*" & consume
            End If

            Return eq
        End Function

        Private Function __contact(reactions As SbmlReaction()) As String
            If reactions Is Nothing OrElse reactions.Length = 0 Then
                Return ""
            End If

            Return reactions _
                .Select(Function(rxn) __contact(rxn.Reactants)) _
                .Where(Function(s) Not String.IsNullOrEmpty(s)) _
                .JoinBy("*")
        End Function

        Private Function __contact(mlst As IEnumerable(Of String)) As String
            If mlst Is Nothing Then
                Return ""
            End If

            Dim ids As String() = mlst _
                .Where(Function(id) Not String.IsNullOrEmpty(id)) _
                .ToArray

            If ids.Length = 0 Then
                Return ""
            ElseIf ids.Length = 1 Then
                Return ids(0) & "^0.5"
            Else
                Return ids.Select(Function(id) id & "^0.5").JoinBy("*")
            End If
        End Function

        Public Overloads Shared Function Compile(path As String,
                                                 Optional AutoFix As Boolean = False,
                                                 Optional options As RunOptions = Nothing) As Script.Model

            If options Is Nothing Then
                options = New RunOptions()
            End If

            Using compiler As New SBML With {
                .SBMLPath = path,
                .AutoFixError = AutoFix,
                .InitialAmountFallback = options.InitialAmountFallback,
                .InitialJitter = options.InitialJitter
            }
                Call compiler.Compile()
                Call compiler.m_compiledModel.WriteScript(path & ".plas")

                Return compiler.Return
            End Using
        End Function

        Public Shared Widening Operator CType(path As String) As SBML
            Return New SBML With {
                .SBMLPath = path
            }
        End Operator
    End Class
End Namespace

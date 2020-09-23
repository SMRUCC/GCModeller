#Region "Microsoft.VisualBasic::84a691f17062d6f38d68293b7915042a, sub-system\FBA\FBA_DP\rFBA\old\Engine.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::3bc4783eafa8a30f568c43e9525c4b77, sub-system\FBA\FBA_DP\rFBA\old\Engine.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    '     Class Engine
'    ' 
'    '         Constructor: (+1 Overloads) Sub New
'    ' 
'    '         Function: __innerTicks, GetFBADataPackage, Initialize, Load
'    ' 
'    '         Sub: ApplyFBAConstraint, RegulationNetwork_TICK
'    ' 
'    '     Class rFBAlpModel
'    ' 
'    '         Properties: FluxObjects
'    ' 
'    '         Constructor: (+1 Overloads) Sub New
'    '         Function: ApplyFluxConstraint, GetFluxColumnIds, getMatrix
'    '         Class FluxMap
'    ' 
'    '             Properties: Boundaries, FluxName, Handle, MappingGeneId, Reversible
'    ' 
'    '             Function: ApplyConstraint
'    ' 
'    '             Sub: Assign
'    ' 
'    ' 
'    ' 
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports System.Xml.Serialization
'Imports Microsoft.VisualBasic.ComponentModel
'Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
'Imports Microsoft.VisualBasic
'Imports SMRUCC.genomics.Analysis.FBA_DP
'Imports SMRUCC.genomics.Model.SBML.FLuxBalanceModel
'Imports SMRUCC.genomics.Analysis.FBA_DP.Models
'Imports SMRUCC.genomics.Model.SBML
'Imports Microsoft.VisualBasic.Data.csv.IO
'Imports Microsoft.VisualBasic.Data.csv

'Namespace rFBA

'    ''' <summary>
'    ''' 包括一个数学迭代计算引擎和一个FBA计算引擎，每迭代计算一次，则计算一次FBA
'    ''' </summary>
'    ''' <remarks></remarks>
'    Public Class Engine : Inherits SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.IterationMathEngine(Of rFBA.NetworkModel)

'        Dim rFBAlpModel As rFBA.rFBAlpModel, FBAlpSolver As FBAlpRSolver
'        ''' <summary>
'        ''' 使用基因号来表示一个酶促代谢反应的标号列表，对于非酶促代谢反应，则使用空字符串来代替
'        ''' </summary>
'        ''' <remarks></remarks>
'        Dim FBAColumns As String()
'        ''' <summary>
'        ''' {RTime, FluxValueCollection{ObjectiveFunction, FluxValues}}()
'        ''' </summary>
'        ''' <remarks></remarks>
'        Dim FBADataPackages As List(Of KeyValuePair(Of Integer, lpOUT)) =
'            New List(Of KeyValuePair(Of Integer, lpOUT))
'        Dim RegulationNetworkDataPackage As rFBA.DataAdapter

'        Sub New(ModelFile As String, rBin As String)
'            Call MyBase.New(ModelFile.LoadXml(Of rFBA.NetworkModel))

'            Dim DataModel = Me._innerDataModel
'            Dim SBMLData = Level2.XmlFile.Load(FileIO.FileSystem.GetParentPath(ModelFile) & "/" & DataModel.MetabolismHref)

'            rFBAlpModel = New rFBA.rFBAlpModel(SBMLData, DataModel.ObjectiveFunction)
'            FBAlpSolver = New FBAlpRSolver(rBin)
'            RegulationNetworkDataPackage = New rFBA.DataAdapter(Me)
'        End Sub

'        Public Shared Function Load(ModelFile As String, rBin As String) As Engine
'            Return New Engine(ModelFile, rBin)
'        End Function

'        Public Overrides Function Initialize() As Integer
'            For i As Integer = 0 To _innerDataModel.NetworkComponents.Count - 1
'                Dim Equation = _innerDataModel.NetworkComponents(i)
'                For j As Integer = 0 To Equation.Variables.Count - 1
'                    Dim Variable = Equation.Variables(j)
'                    Variable._Equation = _innerDataModel.NetworkComponents(Variable.pHandle)
'                Next
'            Next
'            Return 0
'        End Function

'        Private Sub RegulationNetwork_TICK()
'            Dim Equations = _innerDataModel.NetworkComponents
'            For i As Integer = 0 To Equations.Count - 1
'                Dim Equation = Equations(i)
'                Call Equation.Evaluate()
'            Next
'        End Sub

'        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
'            Call RegulationNetwork_TICK()
'            Call MyBase.__runDataAdapter()
'            Call ApplyFBAConstraint()
'            Dim Values = FBAlpSolver.RSolving(rFBAlpModel)
'            Call FBADataPackages.Add(New KeyValuePair(Of Integer, lpOUT)(MyBase._RTime, Values))
'            Return 0
'        End Function

'        ''' <summary>
'        ''' 将FBA模型计算数据转换为一个Excel文件用于保存
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Function GetFBADataPackage() As IO.File
'            Dim CsvData As New IO.File
'            Call CsvData.Add(New String() {"RTime", "ObjectiveFunction"})
'            Call CsvData.First.AddRange(rFBAlpModel.GetFluxColumnIds)
'            Dim LQuery = (From dataLine In Me.FBADataPackages.AsParallel
'                          Let CreateMethod = Function() As RowObject
'                                                 Dim Row As New RowObject
'                                                 Call Row.Add(dataLine.Key)
'                                                 '    Call Row.Add(dataLine.Value.Key)
'                                                 '     Call Row.AddRange(dataLine.Value.Value)
'                                                 Return Row
'                                             End Function
'                          Let NewRowData = CreateMethod()
'                          Select NewRowData
'                          Order By NewRowData.First Ascending).ToArray
'            Call CsvData.AppendRange(LQuery)

'            Return CsvData
'        End Function

'        ''' <summary>
'        ''' 将相应的反应对象的流量上限修改为所代表酶分子的基因的表达量
'        ''' </summary>
'        ''' <remarks></remarks>
'        Private Sub ApplyFBAConstraint()
'            Dim LQuery = (From Gene In MyBase._DynamicsExprs Select rFBAlpModel.ApplyFluxConstraint(Gene.Identifier, Gene.Value)).ToArray
'        End Sub
'    End Class

'    Public Class rFBAlpModel : Inherits Models.SBML
'#Region "Data Cached"
'        ''' <summary>
'        ''' S matrix in FBA model.(FBA模型中的S矩阵)
'        ''' </summary>
'        ''' <remarks></remarks>
'        Dim MAT_S As Double()()
'        ''' <summary>
'        ''' {GeneId, {Reversible, UpperBound}}()，与Columns中的元素的位置一致，并且假设上限和下限在可逆反应中是一致的
'        ''' </summary>
'        ''' <remarks></remarks>
'        Dim FluxConstraints As KeyValuePair(Of String, KeyValuePairObject(Of Boolean, Double))()
'#End Region

'        <XmlElement> Public Property FluxObjects As FluxMap()

'        Public Class FluxMap : Implements IAddressOf

'            <XmlAttribute> Public Property FluxName As String
'            <XmlAttribute> Public Property Reversible As Boolean
'            <XmlElement> Public Property Boundaries As KeyValuePairObject(Of Double, Double)

'            <XmlAttribute> Public Property Handle As Integer Implements IAddressOf.Address
'            ''' <summary>
'            ''' 催化当前这个代谢反应所涉及到的基因
'            ''' </summary>
'            ''' <value></value>
'            ''' <returns></returns>
'            ''' <remarks></remarks>
'            <XmlAttribute> Public Property MappingGeneId As String()

'            Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
'                Me.Handle = address
'            End Sub

'            Protected Friend Function ApplyConstraint(Kernel As rFBA.Engine)
'                Throw New NotImplementedException
'            End Function
'        End Class

'        Public Function GetFluxColumnIds() As String()
'            Dim LQuery = (From item In FluxObjects Select item.FluxName).ToArray
'            Return LQuery
'        End Function

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="SBMl"></param>
'        ''' <param name="objectiveFunction">UniqueId list for the target metabolism reactions.(代谢反应对象的UniqueId列表)</param>
'        ''' <remarks></remarks>
'        Sub New(SBMl As Level2.XmlFile, objectiveFunction As String())
'            Call MyBase.New(SBMl, objectiveFunction, True)
'            MAT_S = MyBase.getMatrix
'        End Sub

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="Id">目标基因的基因号</param>
'        ''' <param name="UpperBound">流量上限，即该基因的当前所计算出来的表达量</param>
'        ''' <remarks></remarks>
'        Public Function ApplyFluxConstraint(Id As String, UpperBound As Double) As Integer
'            Dim idx = MyBase.fluxColumns.IndexOf(Id)
'            FluxConstraints(idx).Value.Value = UpperBound
'            Return 0
'        End Function

'        ''' <summary>
'        ''' 网络结构不会有变化
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Protected Overrides Function getMatrix() As Double()()
'            Return MAT_S
'        End Function
'    End Class
'End Namespace

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic

Namespace rFBA

    ''' <summary>
    ''' 包括一个数学迭代计算引擎和一个FBA计算引擎，每迭代计算一次，则计算一次FBA
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Engine : Inherits LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.IterationMathEngine(Of FBA.rFBA.NetworkModel)

        Dim rFBAlpModel As FBA.rFBA.rFBAlpModel, FBAlpSolver As FBA.FBAlpRSolver
        ''' <summary>
        ''' 使用基因号来表示一个酶促代谢反应的标号列表，对于非酶促代谢反应，则使用空字符串来代替
        ''' </summary>
        ''' <remarks></remarks>
        Dim FBAColumns As String()
        ''' <summary>
        ''' {RTime, FluxValueCollection{ObjectiveFunction, FluxValues}}()
        ''' </summary>
        ''' <remarks></remarks>
        Dim FBADataPackages As List(Of KeyValuePair(Of Integer, lpOUT)) =
            New List(Of KeyValuePair(Of Integer, lpOUT))
        Dim RegulationNetworkDataPackage As rFBA.DataAdapter

        Sub New(ModelFile As String, rBin As String)
            Call MyBase.New(ModelFile.LoadXml(Of rFBA.NetworkModel))

            Dim DataModel = Me._innerDataModel
            Dim SBMLData = LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile.Load(FileIO.FileSystem.GetParentPath(ModelFile) & "/" & DataModel.MetabolismHref)

            rFBAlpModel = New FBA.rFBA.rFBAlpModel(SBMLData, DataModel.ObjectiveFunction)
            FBAlpSolver = New FBA.FBAlpRSolver(rBin)
            RegulationNetworkDataPackage = New rFBA.DataAdapter(Me)
        End Sub

        Public Shared Function Load(ModelFile As String, rBin As String) As Engine
            Return New Engine(ModelFile, rBin)
        End Function

        Public Overrides Function Initialize() As Integer
            For i As Integer = 0 To _innerDataModel.NetworkComponents.Count - 1
                Dim Equation = _innerDataModel.NetworkComponents(i)
                For j As Integer = 0 To Equation.Variables.Count - 1
                    Dim Variable = Equation.Variables(j)
                    Variable._Equation = _innerDataModel.NetworkComponents(Variable.pHandle)
                Next
            Next
            Return 0
        End Function

        Private Sub RegulationNetwork_TICK()
            Dim Equations = _innerDataModel.NetworkComponents
            For i As Integer = 0 To Equations.Count - 1
                Dim Equation = Equations(i)
                Call Equation.Evaluate()
            Next
        End Sub

        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
            Call RegulationNetwork_TICK()
            Call MyBase.__runDataAdapter()
            Call ApplyFBAConstraint()
            Dim Values = FBAlpSolver.RSolving(rFBAlpModel)
            Call FBADataPackages.Add(New KeyValuePair(Of Integer, lpOUT)(MyBase._RTime, Values))
            Return 0
        End Function

        ''' <summary>
        ''' 将FBA模型计算数据转换为一个Excel文件用于保存
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFBADataPackage() As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim CsvData As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Call CsvData.Add(New String() {"RTime", "ObjectiveFunction"})
            Call CsvData.First.AddRange(rFBAlpModel.GetFluxColumnIds)
            Dim LQuery = (From dataLine In Me.FBADataPackages.AsParallel
                          Let CreateMethod = Function() As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
                                                 Dim Row As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject =
                                                     New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
                                                 Call Row.Add(dataLine.Key)
                                                 '    Call Row.Add(dataLine.Value.Key)
                                                 '     Call Row.AddRange(dataLine.Value.Value)
                                                 Return Row
                                             End Function
                          Let NewRowData = CreateMethod()
                          Select NewRowData
                          Order By NewRowData.First Ascending).ToArray
            Call CsvData.AppendRange(LQuery)

            Return CsvData
        End Function

        ''' <summary>
        ''' 将相应的反应对象的流量上限修改为所代表酶分子的基因的表达量
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ApplyFBAConstraint()
            Dim LQuery = (From Gene In MyBase._DynamicsExprs Select rFBAlpModel.ApplyFluxConstraint(Gene.Identifier, Gene.Value)).ToArray
        End Sub
    End Class

    Public Class rFBAlpModel : Inherits FBA.Models.SBML
#Region "Data Cached"
        ''' <summary>
        ''' S matrix in FBA model.(FBA模型中的S矩阵)
        ''' </summary>
        ''' <remarks></remarks>
        Dim MAT_S As Double()()
        ''' <summary>
        ''' {GeneId, {Reversible, UpperBound}}()，与Columns中的元素的位置一致，并且假设上限和下限在可逆反应中是一致的
        ''' </summary>
        ''' <remarks></remarks>
        Dim FluxConstraints As KeyValuePair(Of String, KeyValuePairObject(Of Boolean, Double))()
#End Region

        <XmlElement> Public Property FluxObjects As FluxMap()

        Public Class FluxMap : Implements IAddressHandle

            <XmlAttribute> Public Property FluxName As String
            <XmlAttribute> Public Property Reversible As Boolean
            <XmlElement> Public Property Boundaries As KeyValuePairObject(Of Double, Double)

            <XmlAttribute> Public Property Handle As Integer Implements IAddressHandle.Address
            ''' <summary>
            ''' 催化当前这个代谢反应所涉及到的基因
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            <XmlAttribute> Public Property MappingGeneId As String()

            Protected Friend Function ApplyConstraint(Kernel As rFBA.Engine)
                Throw New NotImplementedException
            End Function

#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                End If
                Me.disposedValue = True
            End Sub

            ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
            'Protected Overrides Sub Finalize()
            '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
            '    Dispose(False)
            '    MyBase.Finalize()
            'End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
#End Region
        End Class

        Public Function GetFluxColumnIds() As String()
            Dim LQuery = (From item In FluxObjects Select item.FluxName).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SBMl"></param>
        ''' <param name="objectiveFunction">UniqueId list for the target metabolism reactions.(代谢反应对象的UniqueId列表)</param>
        ''' <remarks></remarks>
        Sub New(SBMl As LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile, objectiveFunction As String())
            Call MyBase.New(SBMl, objectiveFunction, True)
            MAT_S = MyBase.getMatrix
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Id">目标基因的基因号</param>
        ''' <param name="UpperBound">流量上限，即该基因的当前所计算出来的表达量</param>
        ''' <remarks></remarks>
        Public Function ApplyFluxConstraint(Id As String, UpperBound As Double) As Integer
            Dim idx = MyBase.fluxColumns.IndexOf(Id)
            FluxConstraints(idx).Value.Value = UpperBound
            Return 0
        End Function

        ''' <summary>
        ''' 网络结构不会有变化
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function getMatrix() As Double()()
            Return MAT_S
        End Function
    End Class
End Namespace


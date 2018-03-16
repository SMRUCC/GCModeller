#Region "Microsoft.VisualBasic::3d45d17305fef14e8efe6e75a9a3f6dd, engine\GCModeller\EngineSystem\Engine\SystemResetOperator.vb"

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

    '     Class GCModeller
    ' 
    '         Sub: Reset
    '         Class SystemResetOperator
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Sub: Internal_InvokeResetOperation, InternalResetDataService
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv

Namespace EngineSystem.Engine

    Partial Class GCModeller

        Public Sub Reset()
            Call SystemResetOperator.Internal_InvokeResetOperation(Me)
        End Sub

        ''' <summary>
        ''' 为了节省创建模型的时间花销，在进行全基因组基因突变实验的时候，使用本帮助对象的方法来重置代谢物的浓度为初始浓度以缩短计算时间
        ''' </summary>
        ''' <remarks></remarks>
        Private NotInheritable Class SystemResetOperator

            Private Sub New()
            End Sub

            Public Shared Sub Internal_InvokeResetOperation(Modeller As GCModeller)
                Dim Metabolites = Modeller.KernelModule.Metabolism.Metabolites

                For i As Integer = 0 To Modeller.KernelModule.Metabolism.Metabolites.Count - 1
                    Metabolites(i).Quantity = Metabolites(i).ModelBaseElement.InitialAmount
                Next

                Modeller._RTime = 0
                Modeller.SystemActivityRecordList = New IO.File From {New String() {"#TIME", "Transcription", "Translation", "Metabolism", "SUM()"}}

                '重新设置基因突变参数
                Dim ExpressionRegulationNetwork = Modeller.KernelModule.ExpressionRegulationNetwork
                Dim Transcriptions = ExpressionRegulationNetwork._InternalEvent_Transcriptions
                For i As Integer = 0 To ExpressionRegulationNetwork._InternalEvent_Transcriptions.Count - 1
                    Transcriptions(i).CoefficientFactor = 1 '将所有基因的转录情况重置为正常状态
                Next
                '初始化基因突变设置
                Call Modeller.KernelModule.InitializeGeneObjectMutation()
                '重置数据服务
                Call InternalResetDataService(Modeller)
            End Sub

            ''' <summary>
            ''' 重置计算引擎的数据服务
            ''' </summary>
            ''' <param name="Modeller"></param>
            ''' <remarks></remarks>
            Private Shared Sub InternalResetDataService(Modeller As GCModeller)
                Modeller._DataAcquisitionService = New Services.DataAcquisition.ManageSystem(Modeller)
                Call Modeller.DataAcquisitionService.Join(Modeller.KernelModule.CreateServiceSerials)
                Call Modeller.DataAcquisitionService.Join(Modeller.KernelModule.ExpressionRegulationNetwork.CreateServiceSerials)
                Call Modeller.DataAcquisitionService.Initialize()
                Call Modeller.ConnectDataService(Modeller.GetArguments("-url"))
            End Sub
        End Class
    End Class
End Namespace

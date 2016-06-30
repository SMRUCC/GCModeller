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
                Modeller.SystemActivityRecordList = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File From {New String() {"#TIME", "Transcription", "Translation", "Metabolism", "SUM()"}}

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
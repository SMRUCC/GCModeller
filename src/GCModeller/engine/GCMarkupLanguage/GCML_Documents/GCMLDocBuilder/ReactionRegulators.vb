Imports Microsoft.VisualBasic.Terminal.stdio
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic

Namespace Builder

    Public Class ReactionRegulators : Inherits IBuilder

        Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel)
            Call MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Dim Regulators = (From regr As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Regulation
                              In MetaCyc.GetRegulations
                              Where regr.Types.IndexOf("Regulation-of-Reactions") > -1
                              Select regr).ToArray         '获取所有的ReactionRegulator
            Dim Metabolites = Model.Metabolism.Metabolites.ToArray

            For Each Reaction In Model.Metabolism.MetabolismNetwork
                Dim LQuery = (From regr In Regulators Where String.Equals(Reaction.Identifier, regr.RegulatedEntity) Select regr).ToArray '选择调控本反映的调控因子
                If Not LQuery.IsNullOrEmpty Then '可能有多个调控因子
                    'Reaction.Regulators = (From regr As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Regulation
                    '                       In LQuery
                    '                       Let Regulator = CreateRegulator(regr, Metabolites)
                    '                       Where Not Regulator Is Nothing
                    '                       Select Regulator).ToList
                End If
            Next

            Return Model
        End Function

        Private Shared Function CreateRegulator(RegulatorBaseType As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Regulation,
                                                Metabolites As GCML_Documents.XmlElements.Metabolism.Metabolite()) _
            As GCML_Documents.XmlElements.SignalTransductions.Regulator

            Dim Regulator As GCML_Documents.XmlElements.SignalTransductions.Regulator =
                New GCML_Documents.XmlElements.SignalTransductions.Regulator With
                {
                    .Identifier = RegulatorBaseType.Identifier}
            Regulator.Activation = String.Equals(RegulatorBaseType.Mode, "+")
            Regulator.PhysiologicallyRelevant = String.Equals(RegulatorBaseType.PhysiologicallyRelevant, "T")
            Regulator.Types = RegulatorBaseType.Types.ToArray

            Dim Regulators = (From met In Metabolites Where String.Equals(met.Identifier, RegulatorBaseType.Regulator) Select met).ToArray
            If Regulators.IsNullOrEmpty Then
                Call Printf("[EXCEPTION]Unknown metabolite compound: #UNIQUE-ID=::%s", RegulatorBaseType.Regulator)
                Return Nothing
            Else
                '   Regulator.pHwnd = Regulators.First.Hwnd
            End If

            Return Regulator
        End Function
    End Class
End Namespace
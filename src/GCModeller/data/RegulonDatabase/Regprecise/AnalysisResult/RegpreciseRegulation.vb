Namespace RegulonDatabase

    ''' <summary>
    ''' Write for csv file
    ''' </summary>
    ''' <remarks></remarks>
    Public Module RegulationModels

        Public Class OperonRegulation : Implements IRegulationModel

            Public Property Regulator As String Implements IRegulationModel.Regulator
            Public Property Door As String Implements IRegulationModel.Regulated
            Public Property GeneId As String()
            Public Property RegulationMode As String Implements IRegulationModel.RegulationMode

            Public Overrides Function ToString() As String
                Return RegulationModels.ToString(Me) & String.Format(" [{0}]", String.Join("; ", GeneId))
            End Function
        End Class

        Public Interface IRegulationModel
            Property Regulator As String
            Property Regulated As String
            Property RegulationMode As String
        End Interface

        Public Function ToString(RegulationModel As IRegulationModel) As String
            Dim Regulation As String = If(String.IsNullOrEmpty(RegulationModel.RegulationMode), "Regulates", RegulationModel.RegulationMode)
            Return String.Join(" ", New String() {RegulationModel.Regulator, Regulation, RegulationModel.Regulated})
        End Function

        Public Class RegpreciseRegulation : Implements IRegulationModel

            Public Property Regulator As String Implements IRegulationModel.Regulator
            Public Property Gene As String Implements IRegulationModel.Regulated
            Public Property RegulationMode As String Implements IRegulationModel.RegulationMode

            Public Overrides Function ToString() As String
                Return RegulationModels.ToString(Me)
            End Function
        End Class
    End Module
End Namespace
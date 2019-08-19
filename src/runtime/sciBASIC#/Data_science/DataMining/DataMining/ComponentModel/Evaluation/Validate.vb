Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace ComponentModel.Evaluation

    Public Structure Validate

        Dim actuals As Double()
        Dim predicts As Double()

        Public ReadOnly Property err As Double
            Get
                Dim predicts = Me.predicts

                Return actuals _
                .Select(Function(x, i) Math.Abs(x - predicts(i))) _
                .Average
            End Get
        End Property

        Public ReadOnly Property width As Integer
            Get
                Return actuals.Length
            End Get
        End Property

        Public Shared Iterator Function ROC(data As IEnumerable(Of Validate),
                                        Optional threshold As Sequence = Nothing,
                                        Optional outputLabels$() = Nothing) As IEnumerable(Of NamedCollection(Of Validation))

            Dim dataArray = data.ToArray
            Dim width = dataArray(Scan0).width

            If outputLabels.IsNullOrEmpty Then
                outputLabels = width _
                    .SeqIterator _
                    .Select(Function(i) $"output_i") _
                    .ToArray
            End If

#Disable Warning
            For i As Integer = 0 To width - 1
                Yield New NamedCollection(Of Validation) With {
                    .name = outputLabels(i),
                    .value = Validation.ROC(Of Validate)(
                        entity:=dataArray,
                        getValidate:=Function(x, cutoff) x.actuals(i) >= cutoff,
                        getPredict:=Function(x, cutoff) x.predicts(i) >= cutoff,
                        threshold:=threshold Or Validation.normalRange
                    )
                }
            Next
#Enable Warning
        End Function
    End Structure
End Namespace
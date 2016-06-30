Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace dataExprMAT

    Public Structure Experiment

        Public Property Sample As String
        Public Property Experiment As String
        Public Property Reference As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' <see cref="Experiment"/>/<see cref="Reference"/>
        ''' </summary>
        ''' <param name="expr">&lt;a/b>|&lt;c/d>|&lt;e/f>|....</param>
        ''' <returns></returns>
        Public Shared Function GetSamples(expr As String) As Experiment()
            Dim pairs As String() = expr.Split("|"c)
            Dim samples As Experiment() = pairs.ToArray(AddressOf __sampleTable)
            Return samples
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s">a/b</param>
        ''' <returns></returns>
        Private Shared Function __sampleTable(s As String) As Experiment
            Dim tokens As String() = s.Split("/"c)
            Return New Experiment With {
                .Experiment = tokens(0),
                .Reference = tokens(1),
                .Sample = s
            }
        End Function
    End Structure
End Namespace
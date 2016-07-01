Imports SMRUCC.genomics.Toolkits.RNA_Seq.GenePrediction.DocNodes

Namespace GenePrediction

    Public Class GeneMark

        Public Property Version As String '  PROKARYOTIC (Version 2.6r)
        ''' <summary>
        ''' Sequence file name
        ''' </summary>
        ''' <returns></returns>
        Public Property SeqFileName As String
        ''' <summary>
        ''' Model file name
        ''' </summary>
        ''' <returns></returns>
        Public Property Model As String
        Public Property ModelOrganism As String
        Public Property Time As String

        ''' <summary>
        ''' Predicted genes
        ''' </summary>
        ''' <returns></returns>
        Public Property PredictedGenes As lstGenes

        Public Property Sequence As String
        Public Property Length As Integer
        Public Property GCContent As Double
        Public Property WindowLength As Integer
        Public Property WindowStep As Integer
        Public Property Threshold As Double
        Public Property Matrix As String ': Heuristic, GC = 41
        Public Property Author As String ': Borodovsky Laboratory - Georgia Tech, School Of Biology, Atlanta, GA, USA
        Public Property MatrixOrder As Integer

        Public Property lstORFs As lstORF
        Public Property InterestRegions As InterestRegions
        Public Property FramShifts As FrameShifts

        Public Shared Function ParseDoc(url As String) As GeneMark
            Dim doc As String = url.GET
            Return GeneMark.Parser(doc)
        End Function

        Public Shared Function Parser(doc As String) As GeneMark
            Try
                Return __parser(doc)
            Catch ex As Exception
                ex = New Exception(doc, ex)
                Call ex.PrintException
                Call App.LogException(ex)
                Return Nothing
            End Try
        End Function

        Private Shared Function __parser(doc As String) As GeneMark
            Dim Tokens As String() = doc.lTokens
            Dim gmObj As GeneMark = __paramParser(Tokens)

            On Error Resume Next

            gmObj.FramShifts = FrameShifts.Parser(Tokens)
            gmObj.PredictedGenes = lstGenes.Parser(Tokens)
            gmObj.lstORFs = lstORF.Parser(Tokens)
            gmObj.InterestRegions = InterestRegions.Parser(Tokens)

            Return gmObj
        End Function

        Private Shared Function __paramParser(Tokens As String()) As GeneMark
            Dim params As Dictionary(Of String, String) = __getParameters(Tokens)
            Dim doc As GeneMark = New GeneMark With {
                .Version = Tokens(Scan0)
            }

            doc.SeqFileName = params("Sequence file name")
            doc.Model = params("Model file name")
            doc.ModelOrganism = params("Model organism")
            doc.Time = Tokens(Tokens.Lookup("Model organism") + 1)
            doc.Sequence = params("Sequence")
            doc.Length = Scripting.CTypeDynamic(Of Integer)(params("Sequence length"))
            doc.GCContent = Val(params("GC Content").Replace("%", ""))
            doc.WindowLength = Scripting.CTypeDynamic(Of Integer)(params("Window length"))
            doc.WindowStep = Scripting.CTypeDynamic(Of Integer)(params("Window step"))
            doc.Threshold = Scripting.CTypeDynamic(Of Double)(params("Threshold value"))
            doc.Matrix = params("Matrix")
            doc.MatrixOrder = Scripting.CTypeDynamic(Of Integer)(params("Matrix order"))
            doc.Author = params("Matrix author")

            Return doc
        End Function

        Private Shared Function __getParameters(Tokens As String()) As Dictionary(Of String, String)
            Dim LQuery = (From line As String
                      In Tokens
                          Let pair As String() = line.Split(":"c)
                          Where pair.Length > 1
                          Select pair) _
                            .ToDictionary(Function(x) x(Scan0), elementSelector:=Function(x) x(1).Trim)
            Return LQuery
        End Function
    End Class
End Namespace
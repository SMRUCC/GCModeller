Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Workflows.PromoterParser

    Public MustInherit Class PromoterParser : Implements System.IDisposable

#Region "{Gene.ID, Fasta}"

        Public Property Promoter_100 As Dictionary(Of String, FastaToken)
            Get
                Return _Promoter_100
            End Get
            Protected Set(value As Dictionary(Of String, FastaToken))
                _Promoter_100 = value
            End Set
        End Property

        Public Property Promoter_150 As Dictionary(Of String, FastaToken)
            Get
                Return _Promoter_150
            End Get
            Protected Set(value As Dictionary(Of String, FastaToken))
                _Promoter_150 = value
            End Set
        End Property
        Public Property Promoter_200 As Dictionary(Of String, FastaToken)
            Get
                Return _Promoter_200
            End Get
            Protected Set(value As Dictionary(Of String, FastaToken))
                _Promoter_200 = value
            End Set
        End Property
        Public Property Promoter_250 As Dictionary(Of String, FastaToken)
            Get
                Return _Promoter_250
            End Get
            Protected Set(value As Dictionary(Of String, FastaToken))
                _Promoter_250 = value
            End Set
        End Property
        Public Property Promoter_300 As Dictionary(Of String, FastaToken)
            Get
                Return _Promoter_300
            End Get
            Protected Set(value As Dictionary(Of String, FastaToken))
                _Promoter_300 = value
            End Set
        End Property
        Public Property Promoter_350 As Dictionary(Of String, FastaToken)
            Get
                Return _Promoter_350
            End Get
            Protected Set(value As Dictionary(Of String, FastaToken))
                _Promoter_350 = value
            End Set
        End Property
        Public Property Promoter_400 As Dictionary(Of String, FastaToken)
            Get
                Return _Promoter_400
            End Get
            Protected Set(value As Dictionary(Of String, FastaToken))
                _Promoter_400 = value
            End Set
        End Property
        Public Property Promoter_450 As Dictionary(Of String, FastaToken)
            Get
                Return _Promoter_450
            End Get
            Protected Set(value As Dictionary(Of String, FastaToken))
                _Promoter_450 = value
            End Set
        End Property
        Public Property Promoter_500 As Dictionary(Of String, FastaToken)
            Get
                Return _Promoter_500
            End Get
            Protected Set(value As Dictionary(Of String, FastaToken))
                _Promoter_500 = value
            End Set
        End Property

        Dim _Promoter_100 As Dictionary(Of String, FastaToken)
        Dim _Promoter_150 As Dictionary(Of String, FastaToken)
        Dim _Promoter_200 As Dictionary(Of String, FastaToken)
        Dim _Promoter_250 As Dictionary(Of String, FastaToken)
        Dim _Promoter_300 As Dictionary(Of String, FastaToken)
        Dim _Promoter_350 As Dictionary(Of String, FastaToken)
        Dim _Promoter_400 As Dictionary(Of String, FastaToken)
        Dim _Promoter_450 As Dictionary(Of String, FastaToken)
        Dim _Promoter_500 As Dictionary(Of String, FastaToken)

#End Region

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

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

        Public Shared ReadOnly Property PrefixLength As Integer() = New Integer() {100, 150, 200, 250, 300, 350, 400, 450, 500}

        'Protected Shared Function __filledPretents(list As List(Of FastaToken),
        '                                           itr As Integer) _
        '    As List(Of FastaToken)

        '    If list.IsNullOrEmpty OrElse list.Count >= 6 Then
        '        Return list
        '    Else
        '        itr += 1

        '        For Each fa In list.ToArray
        '            If list.Count < 6 Then
        '                Dim attrs As String() = {fa.Attributes(Scan0) & "_" & itr}.Join(fa.Attributes.Skip(1).ToArray).ToArray
        '                Dim fakeFill As New FastaToken With {
        '                    .SequenceData = fa.SequenceData,
        '                    .Attributes = attrs
        '                }
        '                Call list.Add(fakeFill)
        '            Else
        '                Exit For
        '            End If
        '        Next

        '        Return __filledPretents(list, itr)
        '    End If
        'End Function

        'Public Shared Function FilledPretents(list As Generic.IEnumerable(Of FastaToken)) _
        '    As FastaFile
        '    Return __filledPretents(list.ToList, Scan0)
        'End Function

    End Class
End Namespace
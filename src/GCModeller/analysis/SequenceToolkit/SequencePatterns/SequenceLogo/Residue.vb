Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace SequenceLogo

    ''' <summary>
    ''' A drawing site in the sequence logo drawing.(所绘制的序列logo图之中的一个位点)
    ''' </summary>
    Public Class Residue : Implements IAddressHandle

        ''' <summary>
        ''' ATGC, 4 characters for nt, and aa is 20.
        ''' </summary>
        ''' <returns></returns>
        Public Property Alphabets As Alphabet()
        ''' <summary>
        ''' The total height of the letters depicts the information content Of the position, In bits.
        ''' (Bits的值是和比对的序列的数量是有关系的)
        ''' </summary>
        ''' <returns></returns>
        Public Property Bits As Double

        ''' <summary>
        ''' Position value of this residue in the motif sequence.(这个残基的位点编号)
        ''' </summary>
        ''' <returns></returns>
        Public Property Address As Integer Implements IAddressHandle.Address

        ''' <summary>
        ''' Display this site as a single alphabet, and this property is used for generates the motif string.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AsChar As Char
            Get
                Dim maxInd As Integer = Alphabets.MaxIndex
                Dim c As Char =
                    If(Alphabets.Length = 4,
                    ColorSchema.NT(maxInd),
                    ColorSchema.AA(maxInd))

                Return Motif.ResidueSite.__toChar(c, Alphabets(maxInd).RelativeFrequency)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{Address}]{AsChar} {NameOf(Bits)}:= {Bits}"
        End Function

        ''' <summary>
        ''' Hi is the uncertainty (sometimes called the Shannon entropy) of position i
        ''' 
        '''    Hi = - Σ(f(a,i) x log2(f(a,i))
        ''' 
        ''' Here, f(a,i) is the relative frequency of base or amino acid a at position i (in this residue)
        ''' 
        ''' 但是频率是零的时候怎么处理？？？
        ''' </summary>
        ''' <returns></returns>
        Public Function Hi() As Double
            Dim LQuery As IEnumerable(Of Double) =
                From alph As Alphabet
                In Alphabets
                Select alph.RelativeFrequency *
                    Math.Log(alph.RelativeFrequency, newBase:=2)
            Dim sm As Double = LQuery.Sum * -1

            Return sm
        End Function

        ''' <summary>
        ''' The information content (y-axis) of position i is given by:
        '''     Ri = log2(4) - (Hi + en)   //nt
        '''     Ri = log2(20) - (Hi + en)  //prot 
        ''' 
        ''' 4 for DNA/RNA or 20 for protein. Consequently, the maximum sequence conservation 
        ''' per site Is log2 4 = 2 bits for DNA/RNA And log2 20 ≈ 4.32 bits for proteins.
        ''' 
        ''' </summary>
        ''' <param name="rsd"></param>
        ''' <param name="En"></param>
        ''' <returns></returns>
        Public Shared Function CalculatesBits(rsd As Residue, En As Double, NtMol As Boolean) As Residue
            Dim n As Double = If(NtMol, 2, Math.Log(20, newBase:=2))
            Dim bits = n - (rsd.Hi + En)
            rsd.Bits = bits
            Return rsd
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
    End Class

    ''' <summary>
    ''' Alphabet model in the drawing motif model, nt for 4 and aa for 20
    ''' </summary>
    Public Class Alphabet : Implements IComparable

        ''' <summary>
        ''' A alphabet character which represents one residue.(可以代表本残基的字母值)
        ''' </summary>
        ''' <returns></returns>
        Public Property Alphabet As Char
        ''' <summary>
        ''' The relative alphabet frequency at this site position.
        ''' </summary>
        ''' <returns></returns>
        Public Property RelativeFrequency As Double

        ''' <summary>
        ''' Sorts for the logo drawing
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            If obj Is Nothing Then
                Return 1
            ElseIf obj.GetType <> GetType(Alphabet) Then
                Return 1
            End If

            Dim n As Double = DirectCast(obj, Alphabet).RelativeFrequency

            If RelativeFrequency > n Then
                Return 1
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' The height of letter a in column i Is given by
        '''    height = f(a,i) x R(i)
        ''' 
        ''' (该残基之中本类型的字母的绘制的高度)
        ''' </summary>
        ''' <returns></returns>
        Public Function Height(Ri As Double) As Integer
            Return CInt(Me.RelativeFrequency * Ri)
        End Function

        Public Overrides Function ToString() As String
            Return $"{Alphabet} --> {RelativeFrequency}"
        End Function
    End Class
End Namespace
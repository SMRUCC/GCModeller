
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Namespace SVM

    Friend Class SVR_Q
        Inherits Kernel

        Private ReadOnly l As Integer
        Private ReadOnly cache As Cache
        Private ReadOnly sign As SByte()
        Private ReadOnly index As Integer()
        Private next_buffer As Integer
        Private buffer As Single()()
        Private ReadOnly QD As Double()

        Public Sub New(ByVal prob As Problem, ByVal param As Parameter)
            MyBase.New(prob.Count, prob.X, param)
            l = prob.Count
            cache = New Cache(l, CLng(param.CacheSize) * (1 << 20))
            QD = New Double(2 * l - 1) {}
            sign = New SByte(2 * l - 1) {}
            index = New Integer(2 * l - 1) {}

            For k = 0 To l - 1
                sign(k) = 1
                sign(k + l) = -1
                index(k) = k
                index(k + l) = k
                QD(k) = KernelFunction(k, k)
                QD(k + l) = QD(k)
            Next

            buffer = New Single()() {New Single(2 * l - 1) {}, New Single(2 * l - 1) {}}
            next_buffer = 0
        End Sub

        Public Overrides Sub SwapIndex(ByVal i As Integer, ByVal j As Integer)
            Do
                Dim __ = sign(i)
                sign(i) = sign(j)
                sign(j) = __
            Loop While False

            Do
                Dim __ = index(i)
                index(i) = index(j)
                index(j) = __
            Loop While False

            Do
                Dim __ = QD(i)
                QD(i) = QD(j)
                QD(j) = __
            Loop While False
        End Sub

        Public Overrides Function GetQ(ByVal i As Integer, ByVal len As Integer) As Single()
            Dim data As Single() = Nothing
            Dim j As Integer, real_i = index(i)

            If cache.GetData(real_i, data, l) < l Then
                For j = 0 To l - 1
                    data(j) = CSng(KernelFunction(real_i, j))
                Next
            End If

            ' reorder and copy
            Dim buf = buffer(next_buffer)
            next_buffer = 1 - next_buffer
            Dim si = sign(i)

            For j = 0 To len - 1
                buf(j) = CSng(si) * sign(j) * data(index(j))
            Next

            Return buf
        End Function

        Public Overrides Function GetQD() As Double()
            Return QD
        End Function
    End Class
End Namespace
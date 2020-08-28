Imports System.Globalization
Imports System.Threading

Namespace SVM
    Friend Module TemporaryCulture
        Private _culture As CultureInfo

        Public Sub Start()
            _culture = Thread.CurrentThread.CurrentCulture
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture
        End Sub

        Public Sub [Stop]()
            Thread.CurrentThread.CurrentCulture = _culture
        End Sub
    End Module
End Namespace

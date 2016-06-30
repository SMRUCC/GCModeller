Imports RDotNet.Extensions.VisualBasic

Namespace VennDiagram

    Public MustInherit Class vennBase : Inherits IRToken

        Sub New()
            Requires = {"VennDiagram"}
        End Sub
    End Class
End Namespace
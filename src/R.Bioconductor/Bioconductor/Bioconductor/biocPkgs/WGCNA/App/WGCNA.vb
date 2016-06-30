
Imports RDotNET.Extensions.VisualBasic

Namespace WGCNA.App

    ''' <summary>
    ''' WGCNA脚本对象的基本结构类型
    ''' </summary>
    Public MustInherit Class WGCNA : Inherits IRScript

        Sub New()
            Requires = {"WGCNA"}
        End Sub
    End Class
End Namespace
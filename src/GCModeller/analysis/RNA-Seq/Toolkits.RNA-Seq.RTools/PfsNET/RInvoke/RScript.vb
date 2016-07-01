Imports System.Text
Imports SMRUCC.genomics.AnalysisTools.CellularNetwork.PFSNet
Imports RDotNET.Extensions.VisualBasic

Namespace PfsNET

    Public Class PfsNETScript : Inherits IRScript

        Public Property File1 As String
        Public Property File2 As String
        Public Property File3 As String

        Public Property b As String
        Public Property n As String
        Public Property t1 As String
        Public Property t2 As String

        Sub New(Optional b As String = "0.5", Optional t1 As String = "0.95", Optional t2 As String = "0.85", Optional n As String = "1000")
            Me.b = b
            Me.n = n
            Me.t1 = t1
            Me.t2 = t2
        End Sub

        Protected Overrides Function __R_script() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Call sBuilder.AppendLine(String.Format("result <- pfsnet(""{0}"",""{1}"",""{2}"",{3},{4},{5},{6})", File1.Replace("\", "/"), File2.Replace("\", "/"), File3.Replace("\", "/"), b, t1, t2, n))
            Try
                Call FileIO.FileSystem.WriteAllText("./RScript.log", sBuilder.ToString, False, Encoding.ASCII)
            Catch ex As Exception
                Call App.LogException(ex)
            End Try

            Return sBuilder.ToString
        End Function
    End Class

    Public Class PFSNetResultOut : Inherits DataStructure.PFSNetResultOut

        Public Property STD_OUTPUT As String()

        Public Overrides Function ToString() As String
            Return DataTag
        End Function
    End Class
End Namespace
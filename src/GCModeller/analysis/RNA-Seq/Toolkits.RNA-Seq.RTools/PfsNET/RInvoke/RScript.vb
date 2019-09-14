#Region "Microsoft.VisualBasic::8ef3c0a26707886a98125234f7a1a238, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\PfsNET\RInvoke\RScript.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class PfsNETScript
    ' 
    '         Properties: b, File1, File2, File3, n
    '                     t1, t2
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __R_script
    ' 
    '     Class PFSNetResultOut
    ' 
    '         Properties: STD_OUTPUT
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports SMRUCC.genomics.Analysis.PFSNet

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
            Return tag
        End Function
    End Class
End Namespace

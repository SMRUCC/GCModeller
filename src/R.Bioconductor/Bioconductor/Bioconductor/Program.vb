#Region "Microsoft.VisualBasic::8991f9ffcec7976d3cbb9673a4aa509e, ..\R.Bioconductor\Bioconductor\Bioconductor\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.R.CRAN.Bioconductor.Web
Imports RDotNET.Extensions.VisualBasic
Imports SMRUCC.R.CRAN.Bioconductor.Web.Packages
Imports System.Threading

Module Program

    Public Sub Main()
#If DEBUG Then
        Call Test.Main()
#End If

        Dim splash As New bioc
        Call RunTask(AddressOf splash.ShowDialog)
        Dim repo As Repository = Repository.LoadDefault
        Call Thread.Sleep(500)
        Call splash.Close()
        Call New InstallPackage(repo).ShowDialog()
    End Sub
End Module


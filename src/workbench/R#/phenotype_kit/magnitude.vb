#Region "Microsoft.VisualBasic::9094e5cee511831daa90325d92d93503, R#\phenotype_kit\magnitude.vb"

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


    ' Code Statistics:

    '   Total Lines: 28
    '    Code Lines: 19
    ' Comment Lines: 3
    '   Blank Lines: 6
    '     File Size: 936 B


    ' Module magnitude
    ' 
    '     Function: profiles
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

''' <summary>
''' HTS expression data simulating for analysis test
''' </summary>
<Package("magnitude", Category:=APICategories.UtilityTools)>
Module magnitude

    <ExportAPI("profiles")>
    Public Function profiles(selector As String, foldchange As Double, Optional base As list = Nothing, Optional env As Environment = Nothing) As Object
        Dim mapNames As String() = KOSelector.SelectMaps(selector)
        Dim data As Dictionary(Of String, Double)

        If base Is Nothing Then
            base = New list With {
                .slots = New Dictionary(Of String, Object)
            }
        End If

        data = base.AsGeneric(Of Double)(env)


    End Function
End Module

#Region "Microsoft.VisualBasic::755d58da5fe31057e3a88298b1aaf756, R#\metagenomics_kit\OTUTableTools.vb"

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

    '   Total Lines: 43
    '    Code Lines: 37
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.39 KB


    ' Module OTUTableTools
    ' 
    '     Function: filter, relativeAbundance
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome

<Package("OTU_table")>
Module OTUTableTools

    <ExportAPI("relative_abundance")>
    Public Function relativeAbundance(x As OTUTable()) As OTUTable()
        Dim sample_ids As String() = x _
            .Select(Function(otu) otu.Properties.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim v As Vector

        For Each name As String In sample_ids
            v = x.Select(Function(otu) otu(name)).AsVector
            v = v / v.Sum

            For i As Integer = 0 To x.Length - 1
                x(i)(name) = v(i)
            Next
        Next

        Return x
    End Function

    <ExportAPI("filter")>
    Public Function filter(x As OTUTable(), relative_abundance As Double) As OTUTable()
        Return x _
            .Where(Function(otu)
                       Return otu.Properties _
                          .Values _
                          .Any(Function(xi)
                                   Return xi > relative_abundance
                               End Function)
                   End Function) _
            .ToArray
    End Function
End Module


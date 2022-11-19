#Region "Microsoft.VisualBasic::ad6faa429f93eecd5d406f9d43fd6fdf, GCModeller\data\SABIO-RK\SBML\TargetParser.vb"

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

    '   Total Lines: 40
    '    Code Lines: 33
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.33 KB


    '     Module TargetParser
    ' 
    '         Function: getIdentifier, getIdentifiers
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Namespace SBML

    Module TargetParser

        <Extension>
        Friend Iterator Function getIdentifiers(react As SBMLReaction) As IEnumerable(Of NamedValue(Of String))
            Dim annotation = react.annotation.RDF.description

            If Not annotation.is.IsNullOrEmpty Then
                For Each ref In annotation.is
                    Yield ref.Bag _
                        .list(Scan0) _
                        .getIdentifier("is")
                Next
            End If

            If Not annotation.isVersionOf Is Nothing Then
                Yield annotation.isVersionOf _
                    .Bag _
                    .list(Scan0) _
                    .getIdentifier(NameOf(annotation.isVersionOf))
            End If
        End Function

        <Extension>
        Public Function getIdentifier(ref As li, Optional type$ = "is") As NamedValue(Of String)
            Dim tokens = ref.resource.Split("/"c).AsList

            Return New NamedValue(Of String) With {
                .Description = type,
                .Name = tokens(-2),
                .Value = tokens(-1)
            }
        End Function
    End Module
End Namespace

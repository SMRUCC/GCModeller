#Region "Microsoft.VisualBasic::af2a9917b044c54363748be43200f1f5, GCModeller\data\RegulonDatabase\Regprecise\AnalysisResult\Effectors.vb"

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

    '   Total Lines: 19
    '    Code Lines: 15
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 549 B


    '     Class Effectors
    ' 
    '         Properties: BiologicalProcess, Effector, KEGG, MetaCyc, Pathway
    '                     Regulon, TF
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Regprecise

    Public Class Effectors

        Public Property Effector As String
        Public Property TF As String
        Public Property Regulon As String
        Public Property Pathway As String
        Public Property BiologicalProcess As String
        Public Property KEGG As String
        Public Property MetaCyc As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace

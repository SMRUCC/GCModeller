#Region "Microsoft.VisualBasic::ce5bc73251f3517c24c4af32ea1596ca, data\RegulonDatabase\Regprecise\AnalysisResult\Effectors.vb"

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

    '     Class Effectors
    ' 
    '         Properties: BiologicalProcess, Effector, KEGG, MetaCyc, Pathway
    '                     Regulon, TF
    ' 
    '         Function: Fill, ToString
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

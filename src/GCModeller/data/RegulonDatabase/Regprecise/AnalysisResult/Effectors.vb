#Region "Microsoft.VisualBasic::d8cce7cbafc8cbfbfaa5498dbbd475dd, ..\GCModeller\data\RegulonDatabase\Regprecise\AnalysisResult\Effectors.vb"

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

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema
Imports Microsoft.VisualBasic.Serialization
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

        Public Function Fill(compound As ICompoundObject) As Effectors
            If String.IsNullOrEmpty(MetaCyc) Then
                MetaCyc = compound.Key
            End If
            Return Me
        End Function
    End Class
End Namespace

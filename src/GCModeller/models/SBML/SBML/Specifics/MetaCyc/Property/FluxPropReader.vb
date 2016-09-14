#Region "Microsoft.VisualBasic::d28e6f75780c5eecdbee4f1d338a9ea6, ..\GCModeller\models\SBML\SBML\Specifics\MetaCyc\Property\FluxPropReader.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.Model.SBML.Components

Namespace Specifics.MetaCyc

    Public Class FluxPropReader : Inherits ReaderBase(Of FluxProperties)

        Sub New(note As Notes)
            Call MyBase.New(note.Properties, PropertyParser.FluxKeyMaps)

            Me.BIOCYC = __getValue(FluxProperties.BIOCYC)
            Me.ConfidenceLevel = Val(__getValue(FluxProperties.ConfidenceLevel))
            Me.ECNumber = GetEcList(__getValue(FluxProperties.ECNumber))
            Me.GENE_ASSOCIATION = GetGenes(__getValue(FluxProperties.GENE_ASSOCIATION))
            Me.SUBSYSTEM = __getValue(FluxProperties.SUBSYSTEM)
        End Sub

        Public ReadOnly Property BIOCYC As String
        Public ReadOnly Property ECNumber As String()
        Public ReadOnly Property SUBSYSTEM As String
        Public ReadOnly Property GENE_ASSOCIATION As String()
        Public ReadOnly Property ConfidenceLevel As Double
    End Class
End Namespace

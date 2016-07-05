#Region "Microsoft.VisualBasic::592e63e873ce460143128d0f910a6fe4, ..\GCModeller\analysis\Xfam\Pfam\MPAlignment\Components\DomainAlignment.vb"

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

Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace ProteinDomainArchitecture.MPAlignment

    Public Class DomainAlignment
        Public Property ProteinQueryDomainDs As DomainDistribution
        Public Property ProteinSbjctDomainDs As DomainDistribution
        <XmlAttribute> Public Property Score As Double

        Public ReadOnly Property IsMatch As Boolean
            Get
                Return Score > 0
            End Get
        End Property

        Public Function FormatPlantTextOutput(QueryMaxLength As Integer, SbjctMaxLength As Integer) As String
            Dim array As String() = {
                String.Format("{0}{1}", ProteinQueryDomainDs.DomainId, New String(" ", QueryMaxLength - Len(ProteinQueryDomainDs.DomainId))),
                String.Format("{0}{1}", ProteinSbjctDomainDs.DomainId, New String(" ", SbjctMaxLength - Len(ProteinSbjctDomainDs.DomainId))),
                Score
            }
            Return String.Join(" ", array)
        End Function

        Public Overrides Function ToString() As String
            Return String.Join(vbTab, {ProteinQueryDomainDs.DomainId, ProteinSbjctDomainDs.DomainId, Score})
        End Function
    End Class
End Namespace

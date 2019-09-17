#Region "Microsoft.VisualBasic::eb9e3c3344984b179dcba808c95357c4, analysis\Motifs\CRISPR\CRT\Output\CRISPR.vb"

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

    '     Class CRISPR
    ' 
    '         Properties: ID, RepeatLocis, Right, SpacerLocis, Start
    ' 
    '         Function: ToString
    ' 
    '         Sub: Assign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel

Namespace Output

    Public Class CRISPR : Implements IAddressOf

        ''' <summary>
        ''' 每一个CRISPR位点之中的重复片段位点
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property RepeatLocis As Loci()
        ''' <summary>
        ''' 每一个CRISPR位点之中的重复片段之间的间隔序列（外源DNA片段）
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property SpacerLocis As Loci()
        <XmlAttribute> Public Property ID As Integer Implements IAddressOf.Address
        <XmlAttribute> Public Property Start As Integer

        ''' <summary>
        ''' last_left + last_length
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Right As Integer
            Get
                Return RepeatLocis.Last.Left + Len(RepeatLocis.Last.SequenceData)
            End Get
        End Property

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.ID = address
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace

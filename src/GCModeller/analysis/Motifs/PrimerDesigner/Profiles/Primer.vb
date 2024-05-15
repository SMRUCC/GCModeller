#Region "Microsoft.VisualBasic::1e3079d9ec677980db713f7b13ade094, analysis\Motifs\PrimerDesigner\Profiles\Primer.vb"

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

    '   Total Lines: 89
    '    Code Lines: 58
    ' Comment Lines: 18
    '   Blank Lines: 13
    '     File Size: 2.54 KB


    ' Class Primer
    ' 
    '     Properties: Alpha, AntisenseRestrictedSite, Beta, Forward, ForwardSequence
    '                 Gamma, GCDifference, Product, Reversed, ReversedSequence
    '                 SenseRestrictedSite, TmDifference
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Public Class Primer

    Public ReadOnly Property GCDifference As Double
        Get
            Return Math.Abs(ForwardSequence.GC - ReversedSequence.GC)
        End Get
    End Property

    Public ReadOnly Property TmDifference As Double
        Get
            Return Math.Abs(ForwardSequence.Tm - ReversedSequence.Tm)
        End Get
    End Property

    Public ReadOnly Property Product As NucleotideLocation
        Get
            Return New NucleotideLocation With {
                .Left = Reversed.right,
                .Right = Forward.left
            }
        End Get
    End Property

    Public Property Forward As NucleotideLocation
    Public Property Reversed As NucleotideLocation

    Public Property SenseRestrictedSite As String
    Public Property AntisenseRestrictedSite As String

    ''' <summary>
    ''' Fe - Fs
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Alpha As Integer
        Get
            Return Forward.Ends - Forward.Start
        End Get
    End Property

    ''' <summary>
    ''' Rs - Fe
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Beta As Integer
        Get
            Return Reversed.Start - Reversed.Ends
        End Get
    End Property

    ''' <summary>
    ''' Re - Rs
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Gamma As Integer
        Get
            Return Reversed.Ends - Reversed.Start
        End Get
    End Property

    Dim _Template As IPolymerSequenceModel
    Dim _ReversedTemplate As IPolymerSequenceModel

    Sub New(Template As NucleicAcid)
        _Template = Template
        _ReversedTemplate = Template.Complement
    End Sub

    Public ReadOnly Property ForwardSequence As NucleicAcid
        Get
            Return New NucleicAcid(SenseRestrictedSite & _Template.CutSequenceLinear(Forward.Start, Forward.Ends))
        End Get
    End Property

    Public ReadOnly Property ReversedSequence As NucleicAcid
        Get
            Return New NucleicAcid(AntisenseRestrictedSite & _ReversedTemplate.CutSequenceLinear(Reversed.Start, Reversed.Ends))
        End Get
    End Property
End Class

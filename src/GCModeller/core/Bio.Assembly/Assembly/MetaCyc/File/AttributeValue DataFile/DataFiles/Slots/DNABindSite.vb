#Region "Microsoft.VisualBasic::80a8de84eef217bd2458b64b4814c84c, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\DNABindSite.vb"

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

    '   Total Lines: 66
    '    Code Lines: 18
    ' Comment Lines: 37
    '   Blank Lines: 11
    '     File Size: 3.10 KB


    '     Class DNABindSite
    ' 
    '         Properties: AbsCenterPos, ComponentOf, InvolvedInRegulation, SiteLength, Table
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' The class describes DNA regions that are binding sites for transcription factors.
    ''' (本对象描述了一个能够与转录因子相结合的DNA片段)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DNABindSite : Inherits MetaCyc.File.DataFiles.Slots.Object

        ''' <summary>
        ''' This slot defines the position on the replicon of the center of this binding site.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property AbsCenterPos As String

        ''' <summary>
        ''' This slot links the binding site to a Regulation frame describing the regulatory 
        ''' interaction in which this binding site participates.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>本属性指向Regulation对象</remarks>
        <ExternalKey("regulations", "", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property InvolvedInRegulation As List(Of String)

        ''' <summary>
        ''' This slot defines the extent of a binding site in base pairs. If a value for this 
        ''' slot is omitted, the site length will be computed based on the DNA-Footprint-Size 
        ''' of the binding protein. Thus, a value for this slot should only be supplied here 
        ''' if the site length for a particular transcription factor is not consistent across 
        ''' all its sites.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property SiteLength As String

        <ExternalKey("transunits", "", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property ComponentOf As List(Of String)

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.dnabindsites
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As DNABindSite
        '    Dim NewObj As DNABindSite = New DNABindSite

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of DNABindSite) _
        '        (MetaCyc.File.AttributeValue.Object.Format(DNABindSites.AttributeList, e), NewObj)

        '    If NewObj.Object.ContainsKey("SITE-LENGTH") Then NewObj.SiteLength = NewObj.Object("SITE-LENGTH") Else NewObj.SiteLength = String.Empty
        '    If NewObj.Object.ContainsKey("ABS-CENTER-POS") Then NewObj.AbsCenterPos = NewObj.Object("ABS-CENTER-POS") Else NewObj.AbsCenterPos = String.Empty
        '    NewObj.InvolvedInRegulation = StringQuery(NewObj.Object, "INVOLVED-IN-REGULATION( \d+)?")

        '    Return NewObj
        'End Operator
    End Class
End Namespace

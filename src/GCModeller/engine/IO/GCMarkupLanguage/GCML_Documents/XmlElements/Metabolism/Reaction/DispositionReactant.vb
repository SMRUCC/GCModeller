#Region "Microsoft.VisualBasic::f1a7946f226f1abe9af79c271c7db726, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Metabolism\Reaction\DispositionReactant.vb"

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

    '   Total Lines: 25
    '    Code Lines: 13
    ' Comment Lines: 6
    '   Blank Lines: 6
    '     File Size: 937 B


    '     Class DispositionReactant
    ' 
    '         Properties: Enzymes, GeneralType, UPPER_BOUND
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace GCML_Documents.XmlElements.Metabolism

    Public Class DispositionReactant

        ''' <summary>
        ''' 本属性仅能够取两个值：<see cref="DispositionReactant.GENERAL_TYPE_ID_POLYPEPTIDE">多肽链</see>和<see cref="DispositionReactant.GENERAL_TYPE_ID_TRANSCRIPTS">RNA分子</see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GeneralType As String
        <Collection("Enzymes")> Public Property Enzymes As String()

        Public Const GENERAL_TYPE_ID_POLYPEPTIDE As String = "Polypeptide"
        Public Const GENERAL_TYPE_ID_TRANSCRIPTS As String = "Transcripts"

        Public Property UPPER_BOUND As Double

        Public Overrides Function ToString() As String
            Return GeneralType
        End Function
    End Class
End Namespace

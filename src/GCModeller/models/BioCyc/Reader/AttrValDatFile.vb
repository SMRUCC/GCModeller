#Region "Microsoft.VisualBasic::4a68d4d78ba45e34760f5c1119f49846, GCModeller\models\BioCyc\Reader\AttrValDatFile.vb"

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

    '   Total Lines: 88
    '    Code Lines: 31
    ' Comment Lines: 46
    '   Blank Lines: 11
    '     File Size: 3.38 KB


    ' Class AttrValDatFile
    ' 
    '     Properties: features, fileMeta
    ' 
    '     Function: loadFeatures, ParseFile, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions

''' <summary>
''' ## Attribute-Value
''' 
''' Each attribute-value file contains data for one class of objects, 
''' such as genes or proteins.  A file is divided into entries, where 
''' one entry describes one database object.
''' An entry consists Of a Set Of attribute-value pairs, which describe 
''' properties Of the Object, And relationships Of the Object To other 
''' Object. Each attribute-value pair typically resides On a Single 
''' line Of the file, although In some cases For values that are Long 
''' strings, the value will reside On multiple lines.  An attribute-value 
''' pair consists Of an attribute name, followed by the String " - " 
''' And a value, For example
''' 
''' ```
''' LEFT - NADP
''' ```
''' 
''' A value that requires more than one line Is continued by a newline 
''' followed by a /. Thus, literal slashes at the beginning Of a line 
''' must be escaped As //. A line that contains only // separates objects. 
''' Comment lines can be anywhere In the file And must begin With the 
''' following symbol
''' 
''' ```
''' #
''' ```
''' 
''' Starting In version 6.5 Of Pathway Tools, attribute-value files can 
''' also contain annotation-value pairs.  Annotations are a mechanism For 
''' attaching labeled values To specific attribute values.  For example, 
''' we might want To specify a coefficient For a reactant In a chemical 
''' reaction. An annotations refers To the attribute value that immediately 
''' precedes the annotation.  An annotation-value pair consists Of a caret 
''' symbol "^" that points upward To indicate that the annotation annotates 
''' the preceding attribute value, followed by the annotation label, 
''' followed by the String " - ", followed by a value. The same attribute 
''' name Or annotation label With different values can appear any number 
''' Of times In an Object.  An example annotation-value pair that refers 
''' To the preceding attribute-value pair Is
''' 
''' ```
''' LEFT - NADP
''' ^COEFFICIENT - 1
''' ```
''' </summary>
Public Class AttrValDatFile

    Public Property fileMeta As FileMeta
    Public Property features As FeatureElement()

    Public Overrides Function ToString() As String
        Return $"{fileMeta} ({features.Length} features)"
    End Function

    Public Shared Function ParseFile(file As StreamReader) As AttrValDatFile
        Dim line As Value(Of String) = ""
        Dim meta As FileMeta = FileMeta.readMeta(file, line)
        Dim attrVals As New AttrValDatFile With {
            .fileMeta = meta,
            .features = loadFeatures(file, line).ToArray
        }

        Return attrVals
    End Function

    Private Shared Iterator Function loadFeatures(file As StreamReader, line As Value(Of String)) As IEnumerable(Of FeatureElement)
        Dim buffer As New List(Of String) From {line.Value}

        Do While Not (line = file.ReadLine) Is Nothing
            If line.Value = "//" Then
                Yield FeatureElement.ParseBuffer(buffer.PopAll)
            Else
                Call buffer.Add(line)
            End If
        Loop
    End Function

End Class




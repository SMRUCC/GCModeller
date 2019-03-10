Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Utilities

Namespace OokLanguage

    ' 在这里定义语法高亮

#Region "Format definitions"

    ''' <summary>
    ''' Defines an editor format for the OrdinaryClassification type that has a purple background
    ''' and is underlined.
    ''' </summary>
    <Export(GetType(EditorFormatDefinition)),
    ClassificationType(ClassificationTypeNames:="ook!"),
    Name("ook!"),
    UserVisible(False),
    Order(Before:=Priority.Default)>
    Friend NotInheritable Class OokE
        Inherits ClassificationFormatDefinition

        ''' <summary>
        ''' Defines the visual format for the "ordinary" classification type
        ''' </summary>
        Public Sub New()
            Me.DisplayName = "ook!" 'human readable version of the name
            Me.ForegroundColor = Colors.BlueViolet
        End Sub

    End Class

    ''' <summary>
    ''' Defines an editor format for the OrdinaryClassification type that has a purple background
    ''' and is underlined.
    ''' </summary>
    <Export(GetType(EditorFormatDefinition)),
    ClassificationType(ClassificationTypeNames:="ook?"),
    Name("ook?"),
    UserVisible(False),
    Order(Before:=Priority.Default)>
    Friend NotInheritable Class OokQ
        Inherits ClassificationFormatDefinition

        ''' <summary>
        ''' Defines the visual format for the "ordinary" classification type
        ''' </summary>
        Public Sub New()
            Me.DisplayName = "ook?" 'human readable version of the name
            Me.ForegroundColor = Colors.Green
        End Sub

    End Class

    ''' <summary>
    ''' Defines an editor format for the OrdinaryClassification type that has a purple background
    ''' and is underlined.
    ''' </summary>
    <Export(GetType(EditorFormatDefinition)),
    ClassificationType(ClassificationTypeNames:="ook."),
    Name("ook."),
    UserVisible(False),
    Order(Before:=Priority.Default)>
    Friend NotInheritable Class OokP
        Inherits ClassificationFormatDefinition

        ''' <summary>
        ''' Defines the visual format for the "ook" classification type
        ''' </summary>
        Public Sub New()
            Me.DisplayName = "ook." 'human readable version of the name
            Me.ForegroundColor = Colors.Orange
        End Sub

    End Class
#End Region

End Namespace
